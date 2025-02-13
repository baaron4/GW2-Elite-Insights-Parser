using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class NPC : SingleActor
{
    private IReadOnlyList<(long hpValue, double percent)>? HpDistribution;
    // Constructors
    internal NPC(AgentItem agent) : base(agent)
    {
        if (agent.IsPlayer)
        {
            throw new InvalidDataException("Agent is a player");
        }
    }

    internal override void OverrideName(string name)
    {
        Character = name;
    }
    internal override void SetManualHealth(int health, IReadOnlyList<(long hpValue, double percent)>? hpDistribution = null)
    {
        Health = health;
        HpDistribution = hpDistribution;
        if (hpDistribution != null && (hpDistribution.Count == 0 || hpDistribution[0].percent != 100))
        {
            throw new InvalidOperationException("In SetManualHealth, first element should always have a threshold of 100");
        }
    }

    public override IReadOnlyList<(long hpValue, double percent)>? GetHealthDistribution()
    {
        return HpDistribution;
    }

    public override int GetCurrentHealth(ParsedEvtcLog log, double currentHealthPercent)
    {
        int health = GetHealth(log.CombatData);
        if (health < 0 || currentHealthPercent < 0)
        {
            return -1;
        }
        if (HpDistribution == null)
        {
            return (int)Math.Round(health * currentHealthPercent / 100.0, 0);
        }
        int currentHealth = 0;
        for (int i = HpDistribution.Count - 1; i >= 0; i--)
        {
            (long hpValue, double threshold) = HpDistribution[i];
            double percentToUse = Math.Min(currentHealthPercent, threshold);
            bool breakAfter = currentHealthPercent < threshold;
            if (i <= HpDistribution.Count - 2)
            {
                double prevThreshold = HpDistribution[i + 1].percent;
                percentToUse -= prevThreshold;
            }
            currentHealth += (int)Math.Round(hpValue * percentToUse / 100);
            if (breakAfter)
            {
                break;
            }
        }
        return currentHealth;
    }
    public override int GetCurrentBarrier(ParsedEvtcLog log, double currentBarrierPercent, long time)
    {
        MaxHealthUpdateEvent? currentMaxHealth = log.CombatData.GetMaxHealthUpdateEvents(AgentItem).LastOrDefault(x => x.Time <= time);
        if (currentMaxHealth == null || currentBarrierPercent < 0)
        {
            return -1;
        }
        return (int)Math.Round(currentMaxHealth.MaxHealth * currentBarrierPercent / 100.0, 0);
    }

    public override string GetIcon()
    {
        return AgentItem.Type == AgentItem.AgentType.Gadget ? ParserHelper.GetGadgetIcon() : ParserHelper.GetNPCIcon(ID);
    }

    protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log, CombatReplay replay)
    {
        base.InitAdditionalCombatReplayData(log, replay);
        log.FightData.Logic.ComputeNPCCombatReplayActors(this, log, replay);
        if (replay.Rotations.Count != 0 && (log.FightData.Logic.TargetAgents.Contains(AgentItem) || log.FriendlyAgents.Contains(AgentItem)))
        {
            replay.Decorations.Add(new ActorOrientationDecoration(((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end), AgentItem));
        }
        // Don't put minions of NPC into the minion display system
        AgentItem master = AgentItem.GetFinalMaster();
        if (master != AgentItem && master.IsPlayer)
        {
            SingleActor masterActor = log.FindActor(master)!;
            // Basic linkage
            replay.Decorations.Add(new LineDecoration((replay.TimeOffsets.start, replay.TimeOffsets.end), Colors.Green, 0.5, new AgentConnector(this), new AgentConnector(masterActor)));
            // Prof specific treatment
            ProfHelper.ComputeMinionCombatReplayActors(this, masterActor, log, replay);
        }
    }


    //

    public override SingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
    {
        return new NPCCombatReplayDescription(this, log, map, InitCombatReplay(log));
    }
    protected override void TrimCombatReplay(ParsedEvtcLog log, CombatReplay replay)
    {
        var (_, _, _, actives) = GetStatus(log);
        long trimStart = FirstAware;
        long trimEnd = LastAware;
        long previousActiveEnd = FirstAware;
        for (int i = 0; i < actives.Count - 1; i++)
        {
            if (i == 0)
            {
                trimStart = actives[i].Start;
            } 
            else if (previousActiveEnd != actives[i].Start)
            {
                replay.Hidden.Add(new Segment(previousActiveEnd, actives[i].Start));
            }
            previousActiveEnd = actives[i].End;
        }
        if (actives.Count > 0)
        {
            var last = actives[actives.Count - 1];
            if (actives.Count == 1)
            {
                trimStart = actives[0].Start;
            }
            else if (previousActiveEnd != last.Start)
            {
                replay.Hidden.Add(new Segment(previousActiveEnd, last.Start));
            }
            trimEnd = last.End;
        }
        replay.Trim(trimStart, trimEnd);
    }
}
