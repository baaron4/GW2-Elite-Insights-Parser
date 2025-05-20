using System.Diagnostics;
using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Decoration;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class CombatReplayDecorationContainer
{

    internal delegate GeographicalConnector? CustomConnectorBuilder(ParsedEvtcLog log, AgentItem agent, long start, long end);

    private readonly Dictionary<string, _DecorationMetadata> DecorationCache;
    private readonly List<(_DecorationMetadata metadata, _DecorationRenderingData renderingData)> Decorations;

    internal CombatReplayDecorationContainer(Dictionary<string, _DecorationMetadata> cache, int capacity = 0)
    {
        DecorationCache = cache;
        Decorations = new(capacity);
    }

    public void Add(Decoration decoration)
    {
        if (decoration.Lifespan.end <= decoration.Lifespan.start)
        {
            return;
        }

        _DecorationMetadata constantPart = decoration.DecorationMetadata;
        var id = constantPart.GetSignature();
        if (!DecorationCache.TryGetValue(id, out var cachedMetadata))
        {
            cachedMetadata = constantPart;
            DecorationCache[id] = constantPart;
        }
        Decorations.Add((cachedMetadata, decoration.DecorationRenderingData));
    }

    public void ReserveAdditionalCapacity(int additionalCapacity)
    {
        if(Decorations.Capacity >= Decorations.Count + additionalCapacity) { return; }

        Decorations.Capacity = (int)(Decorations.Capacity * 1.4f);
    }

    public List<DecorationRenderingDescription> GetCombatReplayRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var result = new List<DecorationRenderingDescription>(Decorations.Count);
        foreach (var (constant, renderingData) in Decorations)
        {
            result.Add(renderingData.GetCombatReplayRenderingDescription(map, log, usedSkills, usedBuffs, constant.GetSignature()));
        }
        return result;
    }

    /// <summary>
    /// Add an overhead icon decoration
    /// </summary>
    /// <param name="segment">Lifespan interval</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddOverheadIcon(Segment segment, SingleActor actor, string icon, uint pixelSize = CombatReplayOverheadDefaultSizeInPixel, float opacity = CombatReplayOverheadDefaultOpacity)
    {
        Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)));
    }
    internal void AddOverheadIcon((long start, long end) lifespan, SingleActor actor, string icon, uint pixelSize = CombatReplayOverheadDefaultSizeInPixel, float opacity = CombatReplayOverheadDefaultOpacity)
    {
        Add(new IconOverheadDecoration(icon, pixelSize, opacity, lifespan, new AgentConnector(actor)));
    }
    internal void AddOverheadIcon(Segment segment, AgentItem actor, string icon, uint pixelSize = CombatReplayOverheadDefaultSizeInPixel, float opacity = CombatReplayOverheadDefaultOpacity)
    {
        Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)));
    }
    internal void AddOverheadIcon((long start, long end) lifespan, AgentItem actor, string icon, uint pixelSize = CombatReplayOverheadDefaultSizeInPixel, float opacity = CombatReplayOverheadDefaultOpacity)
    {
        Add(new IconOverheadDecoration(icon, pixelSize, opacity, lifespan, new AgentConnector(actor)));
    }

    /// <summary>
    /// Add an overhead icon decoration
    /// </summary>
    /// <param name="segment">Lifespan interval</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="rotation">rotation of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddRotatedOverheadIcon(Segment segment, SingleActor actor, string icon, float rotation, uint pixelSize = CombatReplayOverheadDefaultSizeInPixel, float opacity = CombatReplayOverheadDefaultOpacity)
    {
        Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)).UsingRotationConnector(new AngleConnector(rotation)));
    }

    /// <summary>
    /// Add an overhead squad marker
    /// </summary>
    /// <param name="segment">Lifespan interval</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="rotation">rotation of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddRotatedOverheadMarkerIcon(Segment segment, SingleActor actor, string icon, float rotation, uint pixelSize = CombatReplayOverheadDefaultSizeInPixel, float opacity = CombatReplayOverheadDefaultOpacity)
    {
        Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)).UsingSquadMarker(true).UsingRotationConnector(new AngleConnector(rotation)));
    }

    /// <summary>
    /// Add overhead icon decorations
    /// </summary>
    /// <param name="segments">Lifespan intervals</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddOverheadIcons(IEnumerable<Segment> segments, SingleActor actor, string icon, uint pixelSize = CombatReplayOverheadDefaultSizeInPixel, float opacity = CombatReplayOverheadDefaultOpacity)
    {
        foreach (Segment segment in segments)
        {
            AddOverheadIcon(segment, actor, icon, pixelSize, opacity);
        }
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a copy using given extra parameters
    /// </summary>
    /// <param name="decoration"></param>
    /// <param name="filled"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddWithFilledWithGrowing(FormDecoration decoration, bool filled, long growingEnd, bool reverseGrowing = false)
    {
        Add(decoration);
        Add(decoration.Copy().UsingFilled(filled).UsingGrowingEnd(growingEnd, reverseGrowing));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a copy using given extra parameters
    /// </summary>
    /// <param name="decoration"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddWithGrowing(FormDecoration decoration, long growingEnd, bool reverseGrowing = false)
    {
        Add(decoration);
        Add(decoration.Copy().UsingGrowingEnd(growingEnd, reverseGrowing));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a copy using given extra parameters
    /// </summary>
    /// <param name="decoration"></param>
    /// <param name="filled"></param>
    internal void AddWithFilled(FormDecoration decoration, bool filled)
    {
        Add(decoration);
        Add(decoration.Copy().UsingFilled(filled));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    internal void AddWithBorder(FormDecoration decoration, string? color = null)
    {
        Add(decoration);
        Add(decoration.GetBorderDecoration(color));
    }
    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    /// <param name="opacity"></param>
    internal void AddWithBorder(FormDecoration decoration, Color color, double opacity)
    {
        AddWithBorder(decoration, color.WithAlpha(opacity).ToString(true));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddWithBorder(FormDecoration decoration, long growingEnd, string? color = null, bool reverseGrowing = false)
    {
        Add(decoration);
        Add(decoration.GetBorderDecoration(color).UsingGrowingEnd(growingEnd, reverseGrowing));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddWithBorder(FormDecoration decoration, long growingEnd, Color color, double opacity, bool reverseGrowing = false)
    {
        AddWithBorder(decoration, growingEnd, color.WithAlpha(opacity).ToString(true), reverseGrowing);
    }


    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="BuffEvent"/>.
    /// </summary>
    /// <param name="tethers">Buff events of the tethers.</param>
    /// <param name="color">color of the tether</param>
    /// <param name="thickness">thickness of the tether</param>
    /// <param name="worldSizeThickess">true to indicate that thickness is in inches instead of pixels</param>
    internal void AddTether(IEnumerable<BuffEvent> tethers, string color, uint thickness = 2, bool worldSizeThickess = false)
    {
        int tetherStart = 0;
        AgentItem src = _unknownAgent;
        AgentItem dst = _unknownAgent;
        foreach (BuffEvent tether in tethers)
        {
            if (tether is BuffApplyEvent)
            {
                tetherStart = (int)tether.Time;
                src = tether.By;
                dst = tether.To;
            }
            else if (tether is BuffRemoveAllEvent)
            {
                int tetherEnd = (int)tether.Time;
                if (!src.IsUnknown && !dst.IsUnknown)
                {
                    Add(new LineDecoration((tetherStart, tetherEnd), color, new AgentConnector(dst), new AgentConnector(src)).WithThickess(thickness, worldSizeThickess));
                    src = _unknownAgent;
                    dst = _unknownAgent;
                }
            }
        }
    }
    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="BuffEvent"/>.
    /// </summary>
    /// <param name="tethers">Buff events of the tethers.</param>
    /// <param name="color">color of the tether</param>
    /// <param name="opacity">opacity of the tether</param>
    /// <param name="thickness">thickness of the tether</param>
    /// <param name="worldSizeThickess">true to indicate that thickness is in inches instead of pixels</param>
    internal void AddTether(IEnumerable<BuffEvent> tethers, Color color, double opacity, uint thickness = 2, bool worldSizeThickess = false)
    {
        AddTether(tethers, color.WithAlpha(opacity).ToString(true), thickness, worldSizeThickess);
    }

    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="EffectEvent"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="effect">Tether effect.</param>
    /// <param name="color">Color of the tether decoration.</param>
    /// <param name="duration">Manual set duration to use as override of the <paramref name="effect"/> duration.</param>
    /// <param name="overrideDuration">Wether to override the duration or not.</param>
    internal void AddTetherByEffectGUID(ParsedEvtcLog log, EffectEvent effect, string color, int duration = 0, bool overrideDuration = false)
    {
        if (!effect.IsAroundDst) { return; }

        (long, long) lifespan;
        if (overrideDuration == false)
        {
            lifespan = effect.ComputeLifespan(log, effect.Duration);
        }
        else
        {
            lifespan = (effect.Time, effect.Time + duration);
        }

        if (!effect.Src.IsUnknown && !effect.Dst.IsUnknown)
        {
            Add(new LineDecoration(lifespan, color, new AgentConnector(effect.Dst), new AgentConnector(effect.Src)));
        }
    }

    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="EffectEvent"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="effect">Tether effect.</param>
    /// <param name="color">Color of the tether decoration.</param>
    /// <param name="opacity">Opacity of the tether decoration.</param>
    /// <param name="duration">Manual set duration to use as override of the <paramref name="effect"/> duration.</param>
    /// <param name="overrideDuration">Wether to override the duration or not.</param>
    internal void AddTetherByEffectGUID(ParsedEvtcLog log, EffectEvent effect, Color color, double opacity, int duration = 0, bool overrideDuration = false)
    {
        AddTetherByEffectGUID(log, effect, color.WithAlpha(opacity).ToString(true), duration, overrideDuration);
    }

    /// <summary>
    /// Add tether decoration connecting a player to an agent.<br></br>
    /// The <paramref name="buffId"/> is sourced by an agent that isn't the one to tether to.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="player">The player to tether to <paramref name="toTetherAgentId"/>.</param>
    /// <param name="buffId">ID of the buff sourced by <paramref name="buffSrcAgentId"/>.</param>
    /// <param name="buffSrcAgentId">ID of the agent sourcing the <paramref name="buffId"/>. Either <see cref="TargetID"/> or <see cref="TrashID"/>.</param>
    /// <param name="toTetherAgentId">ID of the agent to tether to the <paramref name="player"/>. Either <see cref="TargetID"/> or <see cref="TrashID"/>.</param>
    /// <param name="color">Color of the tether.</param>
    /// <param name="firstAwareThreshold">Time threshold in case the agent spawns before the buff application.</param>
    internal void AddTetherByThirdPartySrcBuff(ParsedEvtcLog log, PlayerActor player, long buffId, int buffSrcAgentId, int toTetherAgentId, string color, int firstAwareThreshold = 2000)
    {
        var buffEvents = log.CombatData.GetBuffDataByIDByDst(buffId, player.AgentItem).Where(x => x.CreditedBy.IsSpecies(buffSrcAgentId));
        var buffApplies = buffEvents.OfType<BuffApplyEvent>();
        var buffRemoves = buffEvents.OfType<BuffRemoveAllEvent>();
        var agentsToTether = log.AgentData.GetNPCsByID(toTetherAgentId);

        foreach (BuffApplyEvent buffApply in buffApplies)
        {
            BuffRemoveAllEvent? remove = buffRemoves.FirstOrDefault(x => x.Time > buffApply.Time);
            long removalTime = remove != null ? remove.Time : log.FightData.LogEnd;
            (long, long) lifespan = (buffApply.Time, removalTime);

            foreach (AgentItem agent in agentsToTether)
            {
                if ((Math.Abs(agent.FirstAware - buffApply.Time) < firstAwareThreshold || agent.FirstAware >= buffApply.Time) && agent.FirstAware < removalTime)
                {
                    Add(new LineDecoration(lifespan, color, new AgentConnector(agent), new AgentConnector(player)));
                }
            }
        }
    }
    /// <summary>
    /// Add tether decoration connecting a player to an agent.<br></br>
    /// The <paramref name="buffId"/> is sourced by an agent that isn't the one to tether to.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="player">The player to tether to <paramref name="toTetherAgentId"/>.</param>
    /// <param name="buffId">ID of the buff sourced by <paramref name="buffSrcAgentId"/>.</param>
    /// <param name="buffSrcAgentId">ID of the agent sourcing the <paramref name="buffId"/>. Either <see cref="TargetID"/> or <see cref="TrashID"/>.</param>
    /// <param name="toTetherAgentId">ID of the agent to tether to the <paramref name="player"/>. Either <see cref="TargetID"/> or <see cref="TrashID"/>.</param>
    /// <param name="color">Color of the tether.</param>
    /// <param name="opacity">Opacity of the tether.</param>
    /// <param name="firstAwareThreshold">Time threshold in case the agent spawns before the buff application.</param>
    internal void AddTetherByThirdPartySrcBuff(ParsedEvtcLog log, PlayerActor player, long buffId, int buffSrcAgentId, int toTetherAgentId, Color color, double opacity, int firstAwareThreshold = 2000)
    {
        AddTetherByThirdPartySrcBuff(log, player, buffId, buffSrcAgentId, toTetherAgentId, color.WithAlpha(opacity).ToString(true), firstAwareThreshold);
    }

    /// <summary>
    /// Adds a moving circle resembling a projectile from a <paramref name="startingPoint"/> to an <paramref name="endingPoint"/>.
    /// </summary>
    /// <param name="startingPoint">Starting position.</param>
    /// <param name="endingPoint">Ending position.</param>
    /// <param name="lifespan">Duration of the animation.</param>
    /// <param name="color">Color of the decoration.</param>
    /// <param name="opacity">Opacity of the color.</param>
    /// <param name="radius">Radius of the circle.</param>
    /// <remarks>To be used for logs without missile data.</remarks>
    internal void AddProjectile(in Vector3 startingPoint, in Vector3 endingPoint, (long start, long end) lifespan, Color color, double opacity = 0.2, uint radius = 50)
    {
        AddProjectile(startingPoint, endingPoint, lifespan, color.WithAlpha(opacity).ToString(true), radius);
    }

    /// <summary>
    /// Adds a moving circle resembling a projectile from a <paramref name="startingPoint"/> to an <paramref name="endingPoint"/>.
    /// </summary>
    /// <param name="startingPoint">Starting position.</param>
    /// <param name="endingPoint">Ending position.</param>
    /// <param name="lifespan">Duration of the animation.</param>
    /// <param name="color">Color of the decoration.</param>
    /// <param name="radius">Radius of the circle.</param>
    /// <remarks>To be used for logs without missile data.</remarks>
    internal void AddProjectile(in Vector3 startingPoint, in Vector3 endingPoint, (long start, long end) lifespan, string color, uint radius = 50)
    {
        if (startingPoint == default || endingPoint == default)
        {
            return;
        }
        var startPoint = new ParametricPoint3D(startingPoint, lifespan.start);
        var endPoint = new ParametricPoint3D(endingPoint, lifespan.end);
        var shootingCircle = new CircleDecoration(radius, lifespan, color, new InterpolationConnector([startPoint, endPoint]));
        Add(shootingCircle);
    }

    /// <summary>
    /// Adds a non-filled growing circle resembling a shockwave.
    /// </summary>
    /// <param name="connector">Starting position point.</param>
    /// <param name="lifespan">Lifespan of the shockwave.</param>
    /// <param name="color">Color.</param>
    /// <param name="opacity">Opacity of the <paramref name="color"/>.</param>
    /// <param name="radius">Radius of the shockwave.</param>
    /// <param name="reverse">If the shockwave grows outwards or inwards, outwards is set by default.</param>
    /// <remarks>Uses <see cref="GeographicalConnector"/> which allows us to use <see cref="AgentConnector"/> and <see cref="PositionConnector"/>.</remarks>
    internal void AddShockwave(GeographicalConnector connector, (long start, long end) lifespan, Color color, double opacity, uint radius, bool reverse = false)
    {
        AddShockwave(connector, lifespan, color.WithAlpha(opacity).ToString(true), radius, reverse);
    }

    /// <summary>
    /// Adds a non-filled growing circle resembling a shockwave.
    /// </summary>
    /// <param name="connector">Starting position point.</param>
    /// <param name="lifespan">Lifespan of the shockwave.</param>
    /// <param name="color">Color.</param>
    /// <param name="radius">Radius of the shockwave.</param>
    /// <param name="reverse">If the shockwave grows outwards or inwards, outwards is set by default.</param>
    /// <remarks>Uses <see cref="GeographicalConnector"/> which allows us to use <see cref="AgentConnector"/> and <see cref="PositionConnector"/>.</remarks>
    internal void AddShockwave(GeographicalConnector connector, (long start, long end) lifespan, string color, uint radius, bool reverse = false)
    {
        Add(new CircleDecoration(radius, lifespan, color, connector).UsingFilled(false).UsingGrowingEnd(lifespan.end, reverse));
    }



    /// <summary>
    /// Adds concentric doughnuts.
    /// </summary>
    /// <param name="minRadius">Starting radius.</param>
    /// <param name="radiusIncrease">Radius increase for each concentric ring.</param>
    /// <param name="lifespan">Lifespan of the decoration.</param>
    /// <param name="position">Starting position.</param>
    /// <param name="color">Color of the rings.</param>
    /// <param name="initialOpacity">Starting opacity of the rings' color.</param>
    /// <param name="rings">Total number of rings.</param>
    /// <param name="inverted">Inverts the opacity direction.</param>
    internal void AddContrenticRings(uint minRadius, uint radiusIncrease, (long, long) lifespan, in Vector3 position, Color color, float initialOpacity = 0.5f, int rings = 8, bool inverted = false)
    {
        var positionConnector = new PositionConnector(position);

        for (int i = 1; i <= rings; i++)
        {
            uint maxRadius = minRadius + radiusIncrease;
            float opacity = inverted ? initialOpacity * i : initialOpacity / i;
            var circle = new DoughnutDecoration(minRadius, maxRadius, lifespan, color, opacity, positionConnector);
            AddWithBorder(circle, color, 0.2);
            minRadius = maxRadius;
        }

    }

    /// <summary>
    /// Adds two rectangles over each other representing a loading bar.
    /// </summary>
    /// <param name="actor">Actor to attach the bar to.</param>
    /// <param name="segments">Time segments used to increase or decrease the bar over the background.</param>
    /// <param name="segmentMaxValue">The maximum value the segments could have. Necessary to know to calculate the bar size ratio.</param>
    /// <param name="offsetX">Horizontal offset to position more to the left or to the right.</param>
    /// <param name="offsetY">Vertical offset to position higher or lower.</param>
    /// <param name="width">The maximum width of the bar.</param>
    /// <param name="height">The maximum height of the bar.</param>
    /// <param name="angle">Rotation angle.</param>
    /// <param name="colors">Span containing the colors and opacity. They will be used in the following order:<br></br>
    /// <list type="bullet">
    /// <item>Bar Color</item>
    /// <item>Background Color</item>
    /// <item>Bar Border Color</item>
    /// </list>
    /// </param>
    /// <param name="sizeMultiplier">Multiplies the Value of the Segment to scale the size of the bar. The value cannot be 0 or less.</param>
    internal void AddDynamicBar(SingleActor actor, IReadOnlyList<GenericSegment<double>> segments, double segmentMaxValue, int offsetX, int offsetY, uint width, uint height, float angle, ReadOnlySpan<(Color color, double opacity)> colors, int sizeMultiplier = 1)
    {
        Debug.Assert(sizeMultiplier > 0, $"{nameof(sizeMultiplier)} must be greater than zero but was {sizeMultiplier}");

        uint barWidth;
        var offset = new Vector3(0, offsetY, 0);
        var offsetBackground = new Vector3(offsetX, offsetY, 0);
        var angleConnector = new AngleConnector(angle);

        var ratio = width / segmentMaxValue;

        foreach (var segment in segments)
        {
            offset.X = (float)(offsetX + (-(width / 2) + segment.Value * ratio * sizeMultiplier / 2));
            barWidth = (uint)(segment.Value * sizeMultiplier * ratio);
            var bar = new RectangleDecoration(barWidth, height, segment.TimeSpan, colors[0].color, colors[0].opacity, new AgentConnector(actor).WithOffset(offset, true)).UsingRotationConnector(angleConnector);
            var background = new RectangleDecoration(width, height, segment.TimeSpan, colors[1].color, colors[1].opacity, new AgentConnector(actor).WithOffset(offsetBackground, true)).UsingRotationConnector(angleConnector);
            AddWithBorder((FormDecoration)bar, colors[2].color, colors[2].opacity);
            Add(background);
        }
    }

    /// <summary>
    /// Adds a dynamic breakbar decoration with each various state.<br></br>
    /// The breakbar will be displayed as long as the actor is present and it's active, recovering or immune.
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="percentUpdates"></param>
    /// <param name="stateUpdates"></param>
    internal void AddBreakbar(SingleActor actor, IReadOnlyList<Segment> percentUpdates,
        (IReadOnlyList<Segment> breakbarNones, IReadOnlyList<Segment> breakbarActives, IReadOnlyList<Segment> breakbarImmunes, IReadOnlyList<Segment> breakbarRecoverings) stateUpdates)
    {
        foreach (var segment in stateUpdates.breakbarActives)
        {
            AddBreakbar(segment.TimeSpan, actor, percentUpdates, Colors.BreakbarActiveBlue);
        }
        foreach (var segment in stateUpdates.breakbarRecoverings)
        {
            AddBreakbar(segment.TimeSpan, actor, percentUpdates, Colors.BreakbarRecoveringOrange);
        }
        foreach (var segment in stateUpdates.breakbarImmunes)
        {
            AddBreakbar(segment.TimeSpan, actor, percentUpdates, Colors.BreakbarImmuneGrey);
        }
    }

    private void AddBreakbar((long start, long end) lifespan, SingleActor actor, IReadOnlyList<Segment> percentUpdates, Color color)
    {
        Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, color, 0.8, Colors.Black, 0.6,
            percentUpdates.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(actor))
            .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
            .UsingRotationConnector(new AngleConnector(180)));
    }

    /// <summary>
    /// Adds a dynamic breakbar decoration.<br></br>
    /// To be used during active state only.
    /// </summary>
    /// <param name="lifespan"></param>
    /// <param name="actor"></param>
    /// <param name="percentUpdates"></param>
    internal void AddActiveBreakbar((long start, long end) lifespan, SingleActor actor, IReadOnlyList<Segment> percentUpdates)
    {
        AddBreakbar(lifespan, actor, percentUpdates, Colors.BreakbarActiveBlue);
    }

    private void AddNonHomingMissile(MissileLaunchEvent launch, (long start, long end) trajectoryLifeSpan, Color color, double opacity, uint radius)
    {
        Add(
            new CircleDecoration(radius, trajectoryLifeSpan, color, opacity, new InterpolationConnector([
                  new ParametricPoint3D(launch.LaunchPosition, trajectoryLifeSpan.start),
                  launch.GetFinalPosition(trajectoryLifeSpan)
                ],
                Connector.InterpolationMethod.Linear)
            )
        );
    }

    private void AddHomingMissile(MissileLaunchEvent launch, (long start, long end) trajectoryLifeSpan, Color color, double opacity, uint radius)
    {
        Add(new CircleDecoration(radius, trajectoryLifeSpan, color, opacity, new PositionToAgentConnector(launch.TargetedAgent, launch.LaunchPosition, launch.Time, launch.Speed)));
    }

    /// <summary>
    /// Add missiles going from a Point A to Point B, supports multi launches
    /// </summary>
    /// <param name="log">Evtc log</param>
    /// <param name="missileEvents">Missile events to process</param>
    internal void AddNonHomingMissiles(ParsedEvtcLog log, IEnumerable<MissileEvent> missileEvents, Color color, double opacity, uint radius)
    {
        foreach (MissileEvent missileEvent in missileEvents)
        {
            (long start, long end) = (missileEvent.Time, missileEvent.RemoveEvent?.Time ?? log.FightData.FightEnd);
            for (int i = 0; i < missileEvent.LaunchEvents.Count; i++)
            {
                var launch = missileEvent.LaunchEvents[i];
                (long start, long end) trajectoryLifeSpan = (launch.Time, i != missileEvent.LaunchEvents.Count - 1 ? missileEvent.LaunchEvents[i + 1].Time : end);
                AddNonHomingMissile(launch, trajectoryLifeSpan, color, opacity, radius);
            }
        }
    }

    /// <summary>
    /// Add missiles going from a Point A to Agent, if possible, to Point B otherwise, supports multi launches
    /// </summary>
    /// <param name="log">Evtc log</param>
    /// <param name="missileEvents">Missile events to process</param>
    internal void AddHomingMissiles(ParsedEvtcLog log, IEnumerable<MissileEvent> missileEvents, Color color, double opacity, uint radius)
    {
        foreach (MissileEvent missileEvent in missileEvents)
        {
            (long start, long end) = (missileEvent.Time, missileEvent.RemoveEvent?.Time ?? log.FightData.FightEnd);
            for (int i = 0; i < missileEvent.LaunchEvents.Count; i++)
            {
                var launch = missileEvent.LaunchEvents[i];
                (long start, long end) trajectoryLifeSpan = (launch.Time, i != missileEvent.LaunchEvents.Count - 1 ? missileEvent.LaunchEvents[i + 1].Time : end);
                if (!launch.TargetedAgent.IsNonIdentifiedSpecies())
                {
                    AddHomingMissile(launch, trajectoryLifeSpan, color, opacity, radius);
                } 
                else
                {
                    AddNonHomingMissile(launch, trajectoryLifeSpan, color, opacity, radius);
                }
            }
        }
    }
}

