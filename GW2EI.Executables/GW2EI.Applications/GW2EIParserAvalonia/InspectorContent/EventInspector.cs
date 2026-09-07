using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.InspectorContent;

public static class EventInspector
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();
    private static readonly ConcurrentDictionary<Type, FieldInfo[]> FieldCache = new();
    private const int MaxDepth = 12;
    private const int MaxCollectionItems = 100;
    private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static IReadOnlyList<EventPropertyModel> Inspect(object? instance)
    {
        if (instance == null)
        {
            return [];
        }

        return GetMembers(instance, depth: 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
    }

    private static List<EventPropertyModel> GetMembers(object instance, int depth, HashSet<object> visited)
    {
        var result = new List<EventPropertyModel>();

        if (depth >= MaxDepth)
        {
            return result;
        }

        var type = instance.GetType();
        var trackReference = !(type.IsValueType || IsSimpleType(type));

        if (trackReference && !visited.Add(instance))
        {
            return result;
        }

        try
        {
            AddProperties(result, instance, type, depth, visited);
            AddFields(result, instance, type, depth, visited);

            result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name));
        }
        finally
        {
            if (trackReference)
            {
                visited.Remove(instance);
            }
        }

        return result;
    }

    private static void AddProperties(List<EventPropertyModel> result, object instance, Type type, int depth, HashSet<object> visited)
    {
        foreach (var property in GetAllProperties(type))
        {
            object? value;

            try
            {
                value = property.GetValue(instance);
            }
            catch (Exception ex)
            {
                result.Add(new EventPropertyModel(property.Name, $"<getter threw {GetExceptionName(ex)}>"));
                continue;
            }

            result.Add(CreateNode(property.Name, property.PropertyType, value, depth, visited));
        }
    }

    private static void AddFields(List<EventPropertyModel> result, object instance, Type type, int depth, HashSet<object> visited)
    {
        var existingNames = new HashSet<string>(result.Select(x => x.Name), StringComparer.Ordinal);

        foreach (var field in GetAllFields(type))
        {
            if (!existingNames.Add(field.Name))
            {
                continue;
            }

            object? value;

            try
            {
                value = field.GetValue(instance);
            }
            catch (Exception ex)
            {
                result.Add(new EventPropertyModel(field.Name, $"<unable to read: {GetExceptionName(ex)}>"));
                continue;
            }

            result.Add(CreateNode(field.Name, field.FieldType, value, depth, visited));
        }
    }

    private static EventPropertyModel CreateNode(string name, Type declaredType, object? value, int depth, HashSet<object> visited)
    {
        var node = new EventPropertyModel(name, FormatValue(value));

        if (value == null || depth >= MaxDepth || IsSimpleType(value.GetType() ?? declaredType))
        {
            return node;
        }

        if (value is IDictionary dictionary)
        {
            AddDictionaryChildren(node, dictionary, depth, visited);
        }
        else if (value is IEnumerable enumerable && value is not string)
        {
            AddCollectionChildren(node, enumerable, depth, visited);
        }
        else
        {
            AddObjectChildren(node, value, depth, visited);
        }

        return node;
    }

    private static void AddObjectChildren(EventPropertyModel node, object value, int depth, HashSet<object> visited)
    {
        foreach (var child in GetMembers(value, depth + 1, visited))
        {
            node.Children.Add(child);
        }
    }

    private static void AddCollectionChildren(EventPropertyModel node, IEnumerable enumerable, int depth, HashSet<object> visited)
    {
        if (!visited.Add(enumerable))
        {
            node.Children.Add(new EventPropertyModel("<recursive reference>", "..."));
            return;
        }

        try
        {
            var index = 0;

            foreach (var item in enumerable)
            {
                if (index >= MaxCollectionItems)
                {
                    node.Children.Add(new EventPropertyModel("...", $"Only the first {MaxCollectionItems:N0} items are shown."));
                    break;
                }

                var itemType = item?.GetType() ?? typeof(object);
                var child = CreateNode($"[{index}]", itemType, item, depth + 1, visited);
                node.Children.Add(child);
                index++;
            }
        }
        catch (Exception ex)
        {
            node.Children.Add(new EventPropertyModel("<enumeration failed>", GetExceptionName(ex)));
        }
        finally
        {
            visited.Remove(enumerable);
        }
    }

    private static void AddDictionaryChildren(EventPropertyModel node, IDictionary dictionary, int depth, HashSet<object> visited)
    {
        if (!visited.Add(dictionary))
        {
            node.Children.Add(new EventPropertyModel("<recursive reference>", "..."));
            return;
        }

        try
        {
            var index = 0;

            foreach (DictionaryEntry entry in dictionary)
            {
                if (index >= MaxCollectionItems)
                {
                    node.Children.Add(new EventPropertyModel("...", $"Only the first {MaxCollectionItems:N0} entries are shown."));
                    break;
                }

                var entryNode = new EventPropertyModel($"[{FormatValue(entry.Key)}]", FormatValue(entry.Value));

                if (entry.Value != null && !IsSimpleType(entry.Value.GetType()) && depth + 1 < MaxDepth)
                {
                    if (entry.Value is IDictionary childDictionary)
                    {
                        AddDictionaryChildren(entryNode, childDictionary, depth + 1, visited);
                    }
                    else if (entry.Value is IEnumerable enumerable && entry.Value is not string)
                    {
                        AddCollectionChildren(entryNode, enumerable, depth + 1, visited);
                    }
                    else
                    {
                        AddObjectChildren(entryNode, entry.Value, depth + 1, visited);
                    }
                }

                node.Children.Add(entryNode);
                index++;
            }
        }
        catch (Exception ex)
        {
            node.Children.Add(new EventPropertyModel("<enumeration failed>", GetExceptionName(ex)));
        }
        finally
        {
            visited.Remove(dictionary);
        }
    }

    private static PropertyInfo[] GetAllProperties(Type type)
    {
        return PropertyCache.GetOrAdd(type, static type =>
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<PropertyInfo>();

            foreach (var currentType in GetTypeHierarchy(type))
            {
                foreach (var property in currentType.GetProperties(MemberFlags))
                {
                    if (property.GetIndexParameters().Length > 0 || property.GetMethod == null || !seen.Add(property.Name))
                    {
                        continue;
                    }

                    result.Add(property);
                }
            }

            return result.OrderBy(property => property.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        });
    }

    private static FieldInfo[] GetAllFields(Type type)
    {
        return FieldCache.GetOrAdd(type, static type =>
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<FieldInfo>();

            foreach (var currentType in GetTypeHierarchy(type))
            {
                foreach (var field in currentType.GetFields(MemberFlags))
                {
                    if (field.IsStatic || field.IsDefined(typeof(CompilerGeneratedAttribute)) || !seen.Add(field.Name))
                    {
                        continue;
                    }

                    result.Add(field);
                }
            }

            return result.OrderBy(field => field.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        });
    }

    private static IEnumerable<Type> GetTypeHierarchy(Type type)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            yield return current;
        }
    }

    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(TimeSpan)
               || type == typeof(Guid);
    }

    private static string FormatValue(object? value)
    {
        if (value == null)
        {
            return "null";
        }

        if (value is string text)
        {
            return text;
        }

        if (value is bool boolean)
        {
            return boolean ? "true" : "false";
        }

        if (value is Enum enumValue)
        {
            return enumValue.ToString();
        }

        if (value is char character)
        {
            return $"'{character}'";
        }

        if (value is IEnumerable && value is not string)
        {
            return string.Empty;
        }

        try
        {
            return value.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            return $"<ToString threw {GetExceptionName(ex)}>";
        }
    }

    private static string GetExceptionName(Exception exception)
    {
        if (exception is TargetInvocationException && exception.InnerException != null)
        {
            exception = exception.InnerException;
        }

        return exception.GetType().Name;
    }
}
