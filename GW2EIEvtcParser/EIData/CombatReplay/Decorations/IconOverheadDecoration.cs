﻿using System;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconOverheadDecoration : IconDecoration
    {

        public IconOverheadDecoration(string icon, int pixelSize, float opacity, (int start, int end) lifespan, AgentConnector connector) : base(icon, pixelSize, Math.Min((int)connector.Agent.HitboxWidth, 250), opacity, lifespan, connector)
        {
        }

        public IconOverheadDecoration(string icon, int pixelSize, float opacity, Segment lifespan, AgentConnector connector) : base(icon, pixelSize, Math.Min((int)connector.Agent.HitboxWidth, 250), opacity, lifespan, connector)
        {
        }

        public override GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, SkillModeCategory category = SkillModeCategory.NotApplicable)
        {
            return this;
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new IconOverheadDecorationCombatReplayDescription(log, this, map);
        }
    }
}