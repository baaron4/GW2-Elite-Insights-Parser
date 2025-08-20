using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en")]

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
#if !DEBUG
[assembly: AssemblyVersion("3.12.0.3")]
#else
[assembly: AssemblyVersion("3.12.*")]
#endif
//[assembly: AssemblyFileVersion("1.8.10.34877")]
