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
        public long NewDuration { get; protected set; }
        private bool _sourceFinderRan = false;

        internal BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
            NewDuration = evtcItem.OverstackValue;
            ExtendedDuration = evtcItem.Value;
        }

        internal override void TryFindSrc(ParsedEvtcLog log)
        {
            if (!_sourceFinderRan && By == ParserHelper._unknownAgent)
            {
                _sourceFinderRan = true;
                By = log.Buffs.TryFindSrc(To, Time, ExtendedDuration, log, BuffID, BuffInstance);
            }
        }

        internal void OffsetNewDuration(IReadOnlyList<AbstractBuffEvent> events, int evtcVersion)
        {
            long activeTime = 0;
            long previousTime = long.MinValue;
            for (int i = 0; i < events.Count; i++) {
                AbstractBuffEvent cur = events[i];
                if (i == 0)
                {
                    if (cur is BuffApplyEvent bae)
                    {
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
                    } 
                    else if (cur is BuffStackResetEvent)
                    {
                        // means stack was active between previous and cur
                        activeTime += cur.Time - previousTime;
                    } 
                    else
                    {
                        throw new InvalidOperationException("OffsetNewDuration elements after the first should either be StackActive or StackReset events");
                    }
                }
                previousTime = cur.Time;
            }
            activeTime += Time - previousTime; 
            NewDuration -= activeTime;
            if (evtcVersion < ArcDPSEnums.ArcDPSBuilds.BuffExtensionOverstackValueChanged && evtcVersion >= ArcDPSEnums.ArcDPSBuilds.BuffExtensionBroken)
            {
                ExtendedDuration -= activeTime;
            }
        }

        internal override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Extend(ExtendedDuration, OldDuration, CreditedBy, Time, BuffInstance);
        }

        internal override bool IsBuffSimulatorCompliant(bool useBuffInstanceSimulator)
        {
            return base.IsBuffSimulatorCompliant(useBuffInstanceSimulator) && ExtendedDuration > 0; // security check
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
