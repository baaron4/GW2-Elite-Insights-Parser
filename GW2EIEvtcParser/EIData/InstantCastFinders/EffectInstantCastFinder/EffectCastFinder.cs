using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class EffectCastFinder : CheckedCastFinder<EffectEvent>
    {
        protected bool Minions { get; set; } = false;
        private readonly string _effectGUID;
        private int _speciesId { get; set; } = 0;

        public EffectCastFinder WithMinions(bool minions)
        {
            Minions = minions;
            return this;
        }

        protected AgentItem GetAgent(EffectEvent effectEvent)
        {
            return Minions ? GetKeyAgent(effectEvent).GetFinalMaster() : GetKeyAgent(effectEvent);
        }

        protected virtual AgentItem GetKeyAgent(EffectEvent effectEvent)
        {
            return effectEvent.Src;
        }

        public EffectCastFinder(long skillID, string effectGUID) : base(skillID)
        {
            UsingNotAccurate(true); // TODO: confirm if culling is server side logic
            UsingEnable((combatData) => combatData.HasEffectData);
            _effectGUID = effectGUID;
        }

        internal EffectCastFinder UsingSrcBaseSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == spec);
            return this;
        }

        internal EffectCastFinder UsingDstBaseSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.BaseSpec == spec);
            return this;
        }

        internal EffectCastFinder UsingSrcSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == spec);
            return this;
        }

        internal EffectCastFinder UsingSrcNotSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec != spec);
            return this;
        }

        internal EffectCastFinder UsingSrcSpecsChecker(HashSet<Spec> specs)
        {
            UsingChecker((evt, combatData, agentData, skillData) => specs.Contains(evt.Src.Spec));
            return this;
        }
        internal EffectCastFinder UsingSrcNotSpecsChecker(HashSet<Spec> specs)
        {
            UsingChecker((evt, combatData, agentData, skillData) => !specs.Contains(evt.Src.Spec));
            return this;
        }

        internal EffectCastFinder UsingDstSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.Spec == spec);
            return this;
        }

        internal EffectCastFinder UsingDstNotSpecChecker(Spec spec)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && evt.Dst.Spec != spec);
            return this;
        }

        internal EffectCastFinder UsingDstSpecsChecker(HashSet<Spec> specs)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && specs.Contains(evt.Dst.Spec));
            return this;
        }
        internal EffectCastFinder UsingDstNotSpecsChecker(HashSet<Spec> specs)
        {
            UsingChecker((evt, combatData, agentData, skillData) => evt.IsAroundDst && !specs.Contains(evt.Dst.Spec));
            return this;
        }

        internal EffectCastFinder UsingSecondaryEffectChecker(string effectGUID, long timeOffset = 0, long epsilon = ServerDelayConstant)
        {
            UsingChecker((evt, combatData, agentData, skillData) =>
            {
                if (combatData.TryGetEffectEventsByGUID(effectGUID, out IReadOnlyList<EffectEvent> effectEvents))
                {
                    return effectEvents.Any(other => GetAgent(other) == GetAgent(evt) && Math.Abs(other.Time - timeOffset - evt.Time) < epsilon);
                }
                return false;
            });
            return this;
        }

        protected virtual bool DebugEffectChecker(EffectEvent evt, CombatData combatData, AgentData agentData, SkillData skillData)
        {
            var test = combatData.GetEffectEventsBySrc(evt.Src).Where(x => Math.Abs(x.Time - evt.Time) <= ServerDelayConstant && x.EffectID != evt.EffectID).ToList();
            var testGUIDs = test.Select(x => combatData.GetEffectGUIDEvent(x.EffectID)).Select(x => x.HexContentGUID).ToList();
            var test2 = combatData.GetEffectEventsByDst(evt.Src).Where(x => Math.Abs(x.Time - evt.Time) <= ServerDelayConstant && x.EffectID != evt.EffectID).ToList();
            var test2GUIDs = test2.Select(x => combatData.GetEffectGUIDEvent(evt.EffectID)).Select(x => x.HexContentGUID).ToList();
            return true;
        }

        internal EffectCastFinder UsingDebugEffectChecker()
        {
            UsingChecker(DebugEffectChecker);
            return this;
        }

        internal EffectCastFinder UsingAgentRedirectionIfUnknown(int speciesID)
        {
            _speciesId = speciesID;
            return this;
        }

        public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<InstantCastEvent>();
            EffectGUIDEvent effectGUIDEvent = combatData.GetEffectGUIDEvent(_effectGUID);
            if (effectGUIDEvent != null)
            {
                var effects = combatData.GetEffectEventsByEffectID(effectGUIDEvent.ContentID).GroupBy(x => GetAgent(x)).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<AgentItem, List<EffectEvent>> pair in effects)
                {
                    long lastTime = int.MinValue;
                    foreach (EffectEvent effectEvent in pair.Value)
                    {
                        if (CheckCondition(effectEvent, combatData, agentData, skillData))
                        {
                            if (effectEvent.Time - lastTime < ICD)
                            {
                                lastTime = effectEvent.Time;
                                continue;
                            }
                            lastTime = effectEvent.Time;
                            AgentItem caster = pair.Key;
                            if (_speciesId > 0 && caster.IsUnamedSpecies())
                            {
                                AgentItem agent = agentData.GetNPCsByID(_speciesId).FirstOrDefault(x => x.LastAware >= effectEvent.Time && x.FirstAware <= effectEvent.Time);
                                if (agent != null)
                                {
                                    caster = agent;
                                }
                            }
                            res.Add(new InstantCastEvent(GetTime(effectEvent, caster, combatData), skillData.Get(SkillID), caster));
                        }
                    }
                }
            }
            return res;
        }
    }
}
