
using System;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class GreerTheBlightbringer : MountBalrior
{
    private readonly long[] ReflectableProjectiles = [BlobOfBlight, BlobOfBlight2, ScatteringSporeblast, RainOfSpores];
    public GreerTheBlightbringer(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>()
        {
            new PlayerSrcHitMechanic(ReflectableProjectiles, "Reflected Projectiles", new MechanicPlotlySetting(Symbols.YDown, Colors.Pink), "ProjRefl.Greer.H", "Reflected projectiles have hit Greer", "Reflected Projectile Hit (Greer)", 0).UsingChecker((hde, log) => hde.To.IsSpecies(ArcDPSEnums.TargetID.Greer)),
            new PlayerSrcHitMechanic(ReflectableProjectiles, "Reflected Projectiles", new MechanicPlotlySetting(Symbols.YDown, Colors.Purple), "ProjRefl.Reeg.H", "Reflected projectiles have hit Reeg", "Reflected Projectile Hit (Reeg)", 0).UsingChecker((hde, log) => hde.To.IsSpecies(ArcDPSEnums.TrashID.Reeg)),
            new PlayerSrcHitMechanic(ReflectableProjectiles, "Reflected Projectiles", new MechanicPlotlySetting(Symbols.YDown, Colors.LightPurple), "ProjRefl.Gree.H", "Reflected projectiles have hit Gree", "Reflected Projectile Hit (Gree)", 0).UsingChecker((hde, log) => hde.To.IsSpecies(ArcDPSEnums.TrashID.Gree)),
            new PlayerDstBuffApplyMechanic(InfectiousRotBuff, "Infectious Rot", new MechanicPlotlySetting(Symbols.CircleX, Colors.Red), "InfRot.T", "Targeted by Infectious Rot (Failed Green)", "Infectious Rot (Green Fail)", 0),
            new PlayerDstEffectMechanic(EffectGUIDs.GreerNoxiousBlightGreen, "Noxious Blight", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "NoxBlight.T", "Targeted by Noxious Blight (Green)", "Noxious Blight (Green)", 0),
        });
        Extension = "greer";
        Icon = EncounterIconGreer;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
    }
    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayGreerTheBlightbringer,
                        (1912, 1845),
                        (11200, -10291, 16890, -4800));
    }
    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.Greer,
            (int)ArcDPSEnums.TrashID.Gree,
            (int)ArcDPSEnums.TrashID.Reeg,
        ];
    }

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.Greer,
            (int)ArcDPSEnums.TrashID.Gree,
            (int)ArcDPSEnums.TrashID.Reeg,
        ];
    }

    protected override Dictionary<int, int> GetTargetsSortIDs()
    {
        return new Dictionary<int, int>()
        {
            {(int)ArcDPSEnums.TargetID.Greer, 0 },
            {(int)ArcDPSEnums.TrashID.Gree, 1 },
            {(int)ArcDPSEnums.TrashID.Reeg, 1 },
        };
    }

    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return
        [
            ArcDPSEnums.TrashID.EmpoweringBeast,
        ];
    }


    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor greer = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Greer)) ?? throw new MissingKeyActorsException("Greer not found");
        phases[0].AddTarget(greer);
        var subTitanIDs = new List<int>
        {
            (int) ArcDPSEnums.TrashID.Reeg,
            (int) ArcDPSEnums.TrashID.Gree,
        };
        var subTitans = Targets.Where(x => x.IsAnySpecies(subTitanIDs));
        phases[0].AddSecondaryTargets(subTitans);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByCast(log, InvulnerableBarrier, greer, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (i % 2 == 0)
            {
                phase.Name = "Split " + (i) / 2;
                phase.AddTargets(subTitans);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(greer);
                phase.AddSecondaryTargets(subTitans);
            }
        }
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // Noxious Blight - Green AoE
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.GreerNoxiousBlightGreen, out var noxiousBlight))
        {
            foreach (EffectEvent effect in noxiousBlight)
            {
                long duration = 10000;
                long growing = effect.Time + duration;
                (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                var circle = new CircleDecoration(240, lifespan, Colors.Green, 0.2, new AgentConnector(player));
                replay.Decorations.AddDecorationWithGrowing(circle, growing);
            }
        }

        // Infectious Rot - Failed Green AoE
        var infectiousRot = player.GetBuffStatus(log, InfectiousRotBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        foreach (var segment in infectiousRot)
        {
            replay.Decorations.Add(new CircleDecoration(200, segment.TimeSpan, Colors.Red, 0.2, new AgentConnector(player)));
        }

    }
}
