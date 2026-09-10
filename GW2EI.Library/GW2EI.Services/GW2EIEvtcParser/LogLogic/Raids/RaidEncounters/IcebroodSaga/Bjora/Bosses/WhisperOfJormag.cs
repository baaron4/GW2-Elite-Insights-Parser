using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class WhisperOfJormag : Bjora
{
    public WhisperOfJormag(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(ChainsOfFrostHit, Mech_ChainsOfFrost, new (Symbols.DiamondTall, Colors.Red), new ("H.Chains", "Hit by Chains of Frost", "Chains of Frost"), Sev0),
                new PlayerDstBuffApplyMechanic(ChainsOfFrostApplicationBuff, Mech_ChainsOfFrostApply, new (Symbols.DiamondTall, Colors.LightRed), new ("F.Chains", "Selected for Chains of Frost", "Chains of Frost"), Sev1, 500),
                new EnemyCastStartMechanic(ChainsOfFrostHit, Mech_ChainsOfFrostCast, new (Symbols.Hexagram, Colors.LightRed), new ("F.Chains.C", "Cast Chains of Frost", "Cast Chains of Frost"), Sev3),
            ]),
            new PlayerDstHealthDamageHitMechanic(SlitheringRime, Mech_SlitheringRime, new (Symbols.CircleX, Colors.Red), new ("SlitRime.H", "Hit by Slithering Rime (Orbs)", "Slithering Rime"), Sev2),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(LethalCoalescenceSoaked, Mech_LethalCoalescenceSoaked, new (Symbols.Circle, Colors.Green), new ("S.Lethal.Coal.", "Soaked Lethal Coalescence Damage", "Soaked Lethal Coalescence"), Sev0, 50),
                new EnemyCastStartMechanic(LethalCoalescenceSoaked, Mech_LethalCoalescenceSoakedStart, new (Symbols.Circle, Colors.DarkGreen), new ("Lethal Coalescence", "Cast Lethal Coalescence", "Cast Lethal Coalescence"), Sev3, 50),
                new PlayerDstBuffApplyMechanic(LethalCoalescenceBuff, Mech_LethalCoalescenceBuff, new (Symbols.CircleOpenDot, Colors.Green), new ("LethalCoa.A", "Selected for Lethal Coalescence (Green)", "Lethal Coalescence"), Sev1, 500),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(SpreadingIceOwn, Mech_SpreadingIceOwn, new (Symbols.Circle, Colors.Orange), new ("S.Ice", "Hit by own Spreading Ice", "Spreading Ice (Own)"), Sev3, 50),
                new EnemyCastStartMechanic(SpreadingIceOwn, Mech_SpreadingIceOwnCast, new (Symbols.Hexagram, Colors.DarkRed), new ("S.Ice.C", "Cast Spreading Ice", "Cast Spreading Ice"), Sev3),
                new PlayerDstHealthDamageHitMechanic(SpreadingIceOthers, Mech_SpreadingIceOthers, new (Symbols.TriangleUp, Colors.LightOrange), new ("S.Ice.O", "Hit by other's Spreading Ice", "Spreading Ice (Others)"), Sev0, 50),
            ]),
            new PlayerDstHealthDamageHitMechanic(IcySlice, Mech_IcySlice, new (Symbols.Hexagram, Colors.Orange), new ("I.Slice", "Hit by Icy Slice", "Icy Slice"), Sev1, 50),
            new PlayerDstHealthDamageHitMechanic(IcySlash, Mech_IcySlash, new (Symbols.HexagonOpen, Colors.Orange), new ("I.Slash", "Hit by Icy Slash", "Icy Slash"), Sev1, 50),
            new PlayerDstHealthDamageHitMechanic(IceTempest, Mech_IceTempest, new (Symbols.Square, Colors.Orange), new ("I.Tornado", "Hit by Ice Tempest (Tornadoes)", "Ice Tempest"), Sev1, 50),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(FrigidVortexDamage, Mech_FrigidVortex, new (Symbols.Star, Colors.Pink), new ("FrigVor.H", "Hit by Frigid Vortex", "Frigid Vortex Hit"), Sev0, 50),
                new EnemyCastStartMechanic(FrigidVortexSkill, Mech_FrigidVortexCast, new (Symbols.Star, Colors.Magenta), new ("Frigid Vortex", "Cast Frigid Vortex", "Cast Frigid Vortex"), Sev3, 50),
                new PlayerDstBuffApplyMechanic(FrigidVortexBuff, Mech_FrigidVortexApply, new (Symbols.Star, Colors.LightBlue), new ("FrigVor.A", "Frigid Vortex Applied", "Frigid Vortex Buff"), Sev1),
            ]),
            new PlayerDstHealthDamageHitMechanic([IceShatterWhisper4, IceShatterWhisper2, IceShatterWhisper1, IceShatterWhisper3], Mech_IceShatterWhisper, new (Symbols.Circle, Colors.Teal), new ("IceShatt.H", "Hit by Ice Shatter (Large AoEs)", "Ice Shatter"), Sev1, 150),
            new MechanicGroup([
                new PlayerDstBuffRemoveMechanic(WhisperTeleportBack, Mech_WhisperTPBack, new (Symbols.Circle, Colors.LightBlue), new ("TP In", "Teleported back to the arena", "Teleport Back"), Sev2, 500),
                new PlayerDstBuffRemoveMechanic(WhisperTeleportOut, Mech_WhisperTPOut, new (Symbols.CircleOpen, Colors.LightBlue), new ("TP Out", "Teleported outside of the arena", "Teleport Out"), Sev2, 500),
            ]),
            new PlayerDstHealthDamageHitMechanic([FallingIce1, FallingIce2, FallingIce3], Mech_FallingIce, new (Symbols.Circle, Colors.LightBlue), new ("FallIce", "Hit by Falling Ice", "Falling Ice"), Sev0),
            new EnemyCastStartMechanic([ViciousSlam1, ViciousSlam2], Mech_ViciousSlam, new (Symbols.TriangleUp, Colors.White),  new ("Vicious Slam", "Cast Vicious Slam (Launch)", "Vicious Slam (Launch)"), Sev1, 150),
        ])
        );
        Extension = "woj";
        Icon = EncounterIconWhisperOfJormag;
        LogCategoryInformation.InSubCategoryOrder = 3;
        LogID |= 0x000005;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap(
                        (1682, 1682),
                        (-3287, -1772, 3313, 4828));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayWhisperOfJormag, crMap, parentMap);
        return crMap;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(FrostbiteAuraWhisperOfJormag, FrostbiteAuraWhisperOfJormag),
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor woj = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.WhisperOfJormag)) ?? throw new MissingKeyActorsException("Whisper of Jormag not found");
        phases[0].AddTarget(woj, log);
        if (!requirePhases)
        {
            return phases;
        }
        long start, end;
        var tpOutEvents = log.CombatData.GetBuffRemoveAllData(WhisperTeleportOut).ToList();
        var tpBackEvents = log.CombatData.GetBuffRemoveAllData(WhisperTeleportBack).ToList();
        // 75% tp happened
        if (tpOutEvents.Count > 0)
        {
            end = tpOutEvents.Min(x => x.Time);
            phases.Add(new SubPhasePhaseData(0, end, "Pre Doppelganger 1"));
            // remove everything related to 75% tp out
            tpOutEvents.RemoveAll(x => x.Time <= end + 1000);
        }
        // 75% tp finished
        if (tpBackEvents.Count > 0)
        {
            start = tpBackEvents.Min(x => x.Time);
            // 25% tp happened
            if (tpOutEvents.Count > 0)
            {
                end = tpOutEvents.Min(x => x.Time);
                tpOutEvents.Clear();
                tpBackEvents.RemoveAll(x => x.Time <= end);
            }
            // 25% tp did not happen
            else
            {
                end = log.LogData.LogEnd;
                tpBackEvents.Clear();
            }
            phases.Add(new SubPhasePhaseData(start, end, "Pre Doppelganger 2"));
            // 25% tp finished
            if (tpBackEvents.Count > 0)
            {
                start = tpBackEvents.Min(x => x.Time);
                phases.Add(new SubPhasePhaseData(start, log.LogData.LogEnd, "Final"));
            }
        }
        for (int i = 1; i < phases.Count; i++)
        {
            phases[i].AddTarget(woj, log);
            phases[i].AddParentPhase(phases[0]);
        }
        return phases;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.WhisperEcho,
            TargetID.DoppelgangerElementalist,
            TargetID.DoppelgangerElementalist2,
            TargetID.DoppelgangerEngineer,
            TargetID.DoppelgangerEngineer2,
            TargetID.DoppelgangerGuardian,
            TargetID.DoppelgangerGuardian2,
            TargetID.DoppelgangerMesmer,
            TargetID.DoppelgangerMesmer2,
            TargetID.DoppelgangerNecromancer,
            TargetID.DoppelgangerNecromancer2,
            TargetID.DoppelgangerRanger,
            TargetID.DoppelgangerRanger2,
            TargetID.DoppelgangerRevenant,
            TargetID.DoppelgangerRevenant2,
            TargetID.DoppelgangerThief,
            TargetID.DoppelgangerThief2,
            TargetID.DoppelgangerWarrior,
            TargetID.DoppelgangerWarrior2,
        ];
    }
    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }

        // Lethal Coalescence - Green AoE
        var lethalCoalescence = p.GetBuffStatus(log, LethalCoalescenceBuff).Where(x => x.Value > 0);
        foreach (var segment in lethalCoalescence)
        {
            var circle = new CircleDecoration(260, segment.TimeSpan, Colors.DarkGreen, 0.3, new AgentConnector(p.AgentItem));
            replay.Decorations.AddWithGrowing(circle, segment.TimeSpan.Item2, true);
        }

        // Chains of Frost - Warning
        var chainSegments = p.GetBuffStatus(log, ChainsOfFrostApplicationBuff).Where(x => x.Value > 0);
        var chainsOfFrostApp = GetBuffApplyRemoveSequence(log.CombatData, ChainsOfFrostApplicationBuff, p, true, true);
        replay.Decorations.AddOverheadIcons(chainSegments, p, ParserIcons.BombTimerFullOverhead);
        replay.Decorations.AddTethers(chainsOfFrostApp, Colors.SkyBlue, 0.5);

        // Chains of Frost - Active
        var chainsOfFrostAct = GetBuffApplyRemoveSequence(log.CombatData, ChainsOfFrostActive, p, true, true);
        replay.Decorations.AddTethers(chainsOfFrostAct, Colors.DarkPurpleBlue, 0.4);

        // Spreading Ice - Orange spread AoE
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(p.AgentItem, EffectGUIDs.WhisperOfJormagSpreadingIceIndicator, out var spreadingIce1Events))
        {
            foreach (EffectEvent effect in spreadingIce1Events)
            {
                // The orange indicator lasts 6666 ms
                // The growing red lasts 5000 ms, when the damage lands
                (long start, long end) lifespan = effect.ComputeLifespan(log, 6666);
                (long start, long end) lifespanRed = (effect.Time, effect.Time + 5000);
                var orange = new CircleDecoration(350, lifespan, Colors.LightOrange, 0.2, new AgentConnector(p.AgentItem));
                var red = new CircleDecoration(350, lifespanRed, Colors.Red, 0.2, new AgentConnector(p.AgentItem)).UsingGrowingEnd(lifespanRed.end);
                replay.Decorations.Add(orange);
                replay.Decorations.Add(red);
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }

        switch (target.ID)
        {
            case (int)TargetID.WhisperOfJormag:
                var casts = log.CombatData.GetAnimatedCastData(target.AgentItem);

                foreach (AnimatedCastEvent cast in casts)
                {
                    switch (cast.SkillID)
                    {
                        case WhisperOfJormagShockwave:
                            replay.Decorations.AddShockwave(new AgentConnector(target.AgentItem), (cast.Time, cast.Time + 1000), Colors.Ice, 0.3, 2000);
                            break;
                        default:
                            break;
                    }
                }

                // Ice Tempest - 4 spinning tornadoes
                var iceTempest = log.CombatData.GetMissileEventsBySrcBySkillID(target.AgentItem, IceTempest);
                foreach (MissileEvent missile in iceTempest)
                {
                    replay.Decorations.AddNonHomingMissileWithBorder(log, missile, Colors.BlueishGrey, 0.1, 200, Colors.Red, 0.2);
                }

                // Ice Shatter - 3 orbs spawning 3 big AoEs
                var iceShatter = log.CombatData.GetMissileEventsBySrcBySkillIDs(target.AgentItem, [IceShatterWhisper1, IceShatterWhisper2, IceShatterWhisper3, IceShatterWhisper4]);
                foreach (MissileEvent missile in iceShatter)
                {
                    replay.Decorations.AddNonHomingMissile(log, missile, Colors.CobaltBlue, 0.2, 40);
                }

                // Slithering Rime - Exploding orbs sub 25%
                var slitheringRime = log.CombatData.GetMissileEventsBySrcBySkillID(target.AgentItem, SlitheringRime);
                foreach (MissileEvent missile in slitheringRime)
                {
                    // Orb AoE radius is 80, the orb itself is visually slightly smaller
                    replay.Decorations.AddNonHomingMissile(log, missile, Colors.LightBlue, 0.2, 50);
                    replay.Decorations.AddNonHomingMissile(log, missile, Colors.LightOrange, 0.2, 80);
                }

                // Ice Shatter - Triple orb projectile with big AoE - Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.WhisperOfJormagIceShatterIndicator, out var iceShatterIndicators))
                {
                    foreach (EffectEvent effect in iceShatterIndicators)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1500);
                        var circle = new CircleDecoration(395, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithBorder(circle, Colors.LightOrange, 0.4);
                    }
                }

                // Vicious Slam - Launch in air - Orange indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.WhisperOfJormagViciousSlamIndicator, out var viciousSlamIndicators))
                {
                    foreach (EffectEvent effect in viciousSlamIndicators)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1500);
                        var circle = new CircleDecoration(140, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.Add(circle);
                    }
                }

                // Icy Slash / Icy Slice - Indicator
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.WhisperOfJormagIcySlashIcySliceIndicator, out var icyIndicator))
                {
                    foreach (EffectEvent effect in icyIndicator)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 750);
                        var cone = (PieDecoration)new PieDecoration(1350, 196, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                        replay.Decorations.AddWithBorder(cone, Colors.LightOrange, 0.4);
                    }
                }

                // Icy Slash / Icy Slice - Ice Cone
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.WhisperOfJormagIcySlashIcySliceIce, out var icyCones))
                {
                    foreach (EffectEvent effect in icyCones)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                        var cone = (PieDecoration)new PieDecoration(1350, 28, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position)).UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                        var border = cone.GetBorderDecoration();
                        replay.Decorations.AddWithGrowing(cone, lifespan.start + 200);
                        replay.Decorations.Add(border);
                    }
                }

                // Slithering Rime - Orb explosion
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.WhisperOfJormagSlitheringRimeAoE, out var slitheringRimeAoEs))
                {
                    foreach (EffectEvent effect in slitheringRimeAoEs)
                    {
                        (long start, long end) lifespan = effect.ComputeLifespan(log, 1500);
                        var circle = new CircleDecoration(300, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                        replay.Decorations.AddWithGrowing(circle, lifespan.end);
                    }
                }                
                break;
            default:
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        // Ice Tempest - AoE Indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperOfJormagIceTempestIndicator, out var iceTempestIndicators))
        {
            foreach (EffectEvent effect in iceTempestIndicators)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1666);
                var orange = new CircleDecoration(200, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                var red = new CircleDecoration(200, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position)).UsingGrowingEnd(lifespan.end);
                environmentDecorations.Add(orange);
                environmentDecorations.Add(red);
            }
        }

        // Ice Shatter - Triple orb projectile with big AoE - Damage field
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperOfJormagIceShatterDamage, out var iceShatterDamage))
        {
            foreach (EffectEvent effect in iceShatterDamage)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 6200);
                var circle = new CircleDecoration(390, lifespan, Colors.Ice, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithBorder(circle, Colors.Red, 0.2);
            }
        }

        // Vicious Slam - Launch in air - Ice spike
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.WhisperOfJormagViciousSlamSpike, out var viciousSlamSpikes))
        {
            foreach (EffectEvent effect in viciousSlamSpikes)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 2366);
                var circle = new CircleDecoration(140, lifespan, Colors.DarkBlue, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
            }
        }

        // Falling Ice - Indicator
        AddFallingIceIndicator(log, environmentDecorations, EffectGUIDs.WhisperOfJormagFallingIceIndicator60, 60);
        AddFallingIceIndicator(log, environmentDecorations, EffectGUIDs.WhisperOfJormagFallingIceIndicator120, 120);
        AddFallingIceIndicator(log, environmentDecorations, EffectGUIDs.WhisperOfJormagFallingIceIndicator180, 180);

        // Falling Ice - Spike
        AddFallingIceSpike(log, environmentDecorations, EffectGUIDs.WhisperOfJormagFallingIceSpike60, 60);
        AddFallingIceSpike(log, environmentDecorations, EffectGUIDs.WhisperOfJormagFallingIceSpike120, 120);
        AddFallingIceSpike(log, environmentDecorations, EffectGUIDs.WhisperOfJormagFallingIceSpike180, 180);
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }

    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }

    private static void AddFallingIceIndicator(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations, GUID guid, uint radius)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(guid, out var fallingIceIndicators))
        {
            foreach (EffectEvent effect in fallingIceIndicators)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 1400);
                var circle = new CircleDecoration(radius, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
            }
        }
    }

    private static void AddFallingIceSpike(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations, GUID guid, uint radius)
    {
        if (log.CombatData.TryGetEffectEventsByGUID(guid, out var fallingIceSpikes))
        {
            foreach (EffectEvent effect in fallingIceSpikes)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 4666);
                var circle = new CircleDecoration(radius, lifespan, Colors.White, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithGrowing(circle, lifespan.start + 1400);
            }
        }
    }
}
