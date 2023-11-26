using System;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EIData.BuffSimulators;

namespace GW2EIEvtcParser.ParsedData
{
    public class BuffExtensionEvent : AbstractBuffApplyEvent
    {
        public long OldDuration => NewDuration - ExtendedDuration;
        public long ExtendedDuration { get; protected set; }
        private long OriginalExtendedDuration { get; set; }
        public long NewDuration { get; protected set; }
        private long OriginalNewDuration { get; set; }
        private bool _sourceFinderRan = false;

        internal BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            NewDuration = evtcItem.OverstackValue;
            OriginalNewDuration = NewDuration;
            ExtendedDuration = Math.Max(evtcItem.Value, 0);
            OriginalExtendedDuration = ExtendedDuration;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
            if (!_sourceFinderRan && By == ParserHelper._unknownAgent)
            {
                _sourceFinderRan = true;
                if (ExtendedDuration > 1)
                {
                    By = log.Buffs.TryFindSrc(To, Time, ExtendedDuration, log, BuffID, BuffInstance);
                }
            }
        }

        internal void OffsetNewDuration(IReadOnlyList<AbstractBuffEvent> events, int evtcVersion)
        {
            long activeTime = 0;
            long previousTime = long.MinValue;
            long originalStackDuration = 0;
            for (int i = 0; i < events.Count; i++) {
                AbstractBuffEvent cur = events[i];
                if (i == 0)
                {
                    if (cur is BuffApplyEvent bae)
                    {
                        originalStackDuration = bae.OriginalAppliedDuration;
                        if (bae.Initial)
                        {
                            activeTime += bae.OriginalAppliedDuration - bae.AppliedDuration;
                        }
                    } 
                    else
                    {
                        throw new InvalidOperationException("OffsetNewDuration first element should be buff apply");
                    }
                } else
                {
                    if (cur is BuffStackActiveEvent)
                    {
                        // means stack was not active between previous and cur
                        activeTime = 0;
                    } 
                    else if (cur is BuffStackResetEvent bsre)
                    {
                        // means stack was active between previous and cur
                        activeTime += cur.Time - previousTime;
                        // Total duration gets reset to given value
                        originalStackDuration = bsre.ResetToDuration;
                    }
                    else if (cur is BuffExtensionEvent bee)
                    {
                        // This is a stack reset in disguise
                        if (bee.OriginalNewDuration <= originalStackDuration) 
                        {
                            activeTime = 0;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("OffsetNewDuration elements after the first should either be StackActive or StackReset events");
                    }
                }
                previousTime = cur.Time;
            }
            activeTime += Time - previousTime;
            if (NewDuration <= originalStackDuration)
            {
                ExtendedDuration = activeTime;
                return;
            }
            NewDuration -= activeTime;
            if (evtcVersion < ArcDPSEnums.ArcDPSBuilds.BuffExtensionOverstackValueChanged && evtcVersion >= ArcDPSEnums.ArcDPSBuilds.BuffExtensionBroken)
            {
                ExtendedDuration = Math.Max(ExtendedDuration - activeTime, 0);
            }
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            if (ExtendedDuration <= 1)
            { 
                // no need to bother with 0 extensions
                return;
            }
            simulator.Extend(ExtendedDuration, OldDuration, CreditedBy, Time, BuffInstance);
        }

        /*internal override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffExtensionEvent)
            {
                return 0;
            }
            if (abe is BuffApplyEvent)
            {
                return 1;
            }
            return -1;
        }*/
    }
}
