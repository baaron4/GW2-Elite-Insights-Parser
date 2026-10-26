// May not be needed but have it ready in case the skill in game enums are updated
/*
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData;

internal static class SkillInfoSolver
{
    private const int AnyPositive = int.MinValue;
    private const int AnyNegative = int.MaxValue;
    private class SkillTimingDescriptor : IVersionable
    {
        private readonly int _index;
        private readonly int _time;
        private ulong _minBuild = GW2Builds.StartOfLife;
        private ulong _maxBuild = GW2Builds.EndOfLife;
        private int _minEvtcBuild = ArcDPSBuilds.StartOfLife;
        private int _maxEvtcBuild = ArcDPSBuilds.EndOfLife;
        protected readonly SkillTrigger _result;

        public SkillTimingDescriptor(int index, int time, SkillTrigger result)
        {
            _index = time;
            _time = index;
            _result = result;
        }

        internal SkillTimingDescriptor WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
        {
            _minBuild = minBuild;
            _maxBuild = maxBuild;
            return this;
        }

        internal SkillTimingDescriptor WithEvtcBuilds(int minBuild, int maxBuild = ArcDPSBuilds.EndOfLife)
        {
            _minEvtcBuild = minBuild;
            _maxEvtcBuild = maxBuild;
            return this;
        }

        public bool Available(CombatData combatData)
        {
            ulong gw2Build = combatData.GetGW2BuildEvent().Build;
            if (gw2Build < _maxBuild && gw2Build >= _minBuild)
            {
                int evtcBuild = combatData.GetEvtcVersionEvent().Build;
                if (evtcBuild < _maxEvtcBuild && evtcBuild >= _minEvtcBuild)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool ConditionMatches(SkillTimingWithFlags skillTiming)
        {
            if (skillTiming.Time == _time || (skillTiming.Time > 0 && _time == AnyPositive) || (skillTiming.Time < 0 && _time == AnyNegative))
            {
                if (skillTiming.Index == _index)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Match(SkillTimingWithFlags formula, Dictionary<byte, SkillTrigger> toFill)
        {
            // No need to match anything if we already associated the result
            if (toFill.ContainsValue(_result))
            {
                return true;
            }
            if (formula.Trigger == SkillTrigger.Unknown && !toFill.ContainsKey(formula.TriggerByte))
            {
                if (ConditionMatches(formula))
                {
                    toFill[formula.TriggerByte] = _result;
                    return true;
                }
            }
            return false;
        }
    }
    private class SkillStepDescriptor : IVersionable
    {
        private readonly int _moveDuration;
        private readonly int _actionDuration;
        private readonly int _index;
        private ulong _minBuild = GW2Builds.StartOfLife;
        private ulong _maxBuild = GW2Builds.EndOfLife;
        private int _minEvtcBuild = ArcDPSBuilds.StartOfLife;
        private int _maxEvtcBuild = ArcDPSBuilds.EndOfLife;
        protected readonly uint _result;

        public SkillStepDescriptor(int moveDuration, int actionDuration, int index, uint result)
        {
            _moveDuration = moveDuration;
            _actionDuration = actionDuration;
            _index = index;
            _result = result;
        }

        internal SkillStepDescriptor WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
        {
            _minBuild = minBuild;
            _maxBuild = maxBuild;
            return this;
        }

        internal SkillStepDescriptor WithEvtcBuilds(int minBuild, int maxBuild = ArcDPSBuilds.EndOfLife)
        {
            _minEvtcBuild = minBuild;
            _maxEvtcBuild = maxBuild;
            return this;
        }

        public bool Available(CombatData combatData)
        {
            ulong gw2Build = combatData.GetGW2BuildEvent().Build;
            if (gw2Build < _maxBuild && gw2Build >= _minBuild)
            {
                int evtcBuild = combatData.GetEvtcVersionEvent().Build;
                if (evtcBuild < _maxEvtcBuild && evtcBuild >= _minEvtcBuild)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool ConditionMatches(SkillStep skillStep)
        {
            if (skillStep.ActionDuration == _actionDuration || (skillStep.ActionDuration > 0 && _actionDuration == AnyPositive) || (skillStep.ActionDuration < 0 && _actionDuration == AnyNegative))
            {
                if (skillStep.MoveDuration == _moveDuration || (skillStep.MoveDuration > 0 && _moveDuration == AnyPositive) || (skillStep.MoveDuration < 0 && _moveDuration == AnyNegative))
                {
                    if (skillStep.Index == _moveDuration)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Match(SkillStep skillStep, Dictionary<uint, uint> toFill)
        {
            // No need to match anything if we already associated the result
            if (toFill.ContainsValue(_result))
            {
                return true;
            }
            var flagToCheck = skillStep.Flags;
            // remove all found flags from it
            foreach (var pair in toFill)
            {
                flagToCheck &= ~pair.Key;
            }
            // We determine only one flag per call
            if (ParserHelper.GetIndividualFlagsAsValue(flagToCheck).Count > 1)
            {
                return false;
            }
            if (flagToCheck != 0 && !toFill.ContainsKey(flagToCheck))
            {
                if (ConditionMatches(skillStep))
                {
                    toFill[flagToCheck] = _result;
                    return true;
                }
            }
            return false;
        }
    }

    private static readonly Dictionary<SkillTimingDescriptor, long> _recognizerTiming = new()
    {
       
    };
    // Make sure LoopStart and LoopEnd are detected first via skill steps only containing those flags
    // Each found flag is then removed from the flag value of the next steps, making it possible to detect combined values
    // Eeach recognizer will try to recognize exactly one flag
    private static readonly Dictionary<SkillStepDescriptor, long> _recognizerStep = new()
    {

    };

    private static Dictionary<byte, SkillTrigger> RecognizeTriggers(CombatData combatData, ParserController operation)
    {
        var solvedTriggers = new Dictionary<byte, SkillTrigger>();
        foreach (KeyValuePair<SkillTimingDescriptor, long> pair in _recognizerTiming)
        {
            if (!pair.Key.Available(combatData))
            {
                continue;
            }
            var skillInfoEvent = combatData.GetSkillInfoEvent(pair.Value);
            if (skillInfoEvent != null)
            {
                List<SkillTiming> skillTimings = [..skillInfoEvent.LandAnimation.Timings, ..skillInfoEvent.UnderwaterAnimation.Timings];
                foreach (var skillTiming in skillTimings)
                {
                    if (skillTiming is SkillTimingWithFlags skillTimingWithSteps && pair.Key.Match(skillTimingWithSteps, solvedTriggers))
                    {
                        break;
                    }
                }
            }
        }
        if (solvedTriggers.Values.Distinct().Count() != solvedTriggers.Values.Count)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Incoherent Data in Skill Info Solver, no skill trigger adjustement will be done");
            solvedTriggers.Clear();
        }
        else if (solvedTriggers.Count != 0)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Deduced " + solvedTriggers.Count + " unknown skill triggers");
        }
        return solvedTriggers;
    }


    private static Dictionary<uint, uint> RecognizeSteps(CombatData combatData, ParserController operation)
    {
        var solvedStepFlags = new Dictionary<uint, uint>();
        foreach (KeyValuePair<SkillStepDescriptor, long> pair in _recognizerStep)
        {
            if (!pair.Key.Available(combatData))
            {
                continue;
            }
            var skillInfoEvent = combatData.GetSkillInfoEvent(pair.Value);
            if (skillInfoEvent != null)
            {
                List<SkillStep> skillSteps = [.. skillInfoEvent.LandAnimation.Steps, .. skillInfoEvent.UnderwaterAnimation.Steps];
                foreach (var skillStep in skillSteps)
                {
                    if (pair.Key.Match(skillStep, solvedStepFlags))
                    {
                        break;
                    }
                }
            }
        }
        if (solvedStepFlags.Values.Distinct().Count() != solvedStepFlags.Values.Count)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Incoherent Data in Skill Info Solver, no skill step flag adjustement will be done");
            solvedStepFlags.Clear();
        }
        else if (solvedStepFlags.Count != 0)
        {
            operation.UpdateProgressWithCancellationCheck("Parsing: Deduced " + solvedStepFlags.Count + " unknown skill step flags");
        }
        return solvedStepFlags;
    }

    public static void AdjustSkills(CombatData combatData, ParserController operation)
    {
        var solvedTriggers = RecognizeTriggers(combatData, operation);
        var solvedStepFlags = RecognizeSteps(combatData, operation);
        var skillInfoEvents = combatData.GetSkillInfoEvents();
        foreach (var skillInfoEvent in skillInfoEvents)
        {
        }
    }
}
*/
