/*jshint esversion: 6 */
/* jshint node: true */
/*jslint browser: true */
/* global logData*/
// const images
"use strict";

function compileCRTemplates() {
    TEMPLATE_CR_COMPILE
};

const noUpdateTime = -1;
const updateText = -2;
const deadIcon = new Image();
deadIcon.onload = function () {
    animateCanvas(noUpdateTime);
};
const downEnemyIcon = new Image();
downEnemyIcon.onload = function () {
    animateCanvas(noUpdateTime);
};
const downAllyIcon = new Image();
downAllyIcon.onload = function () {
    animateCanvas(noUpdateTime);
};
const dcIcon = new Image();
dcIcon.onload = function () {
    animateCanvas(noUpdateTime);
};
const facingIcon = new Image();
facingIcon.onload = function () {
    animateCanvas(noUpdateTime);
};

function ToRadians(degrees) {
    return degrees * (Math.PI / 180);
}
function ToDegrees(radians) {
    return radians / (Math.PI / 180);
}

const resolutionMultiplier = 2.0;

const maxOverheadAnimationFrame = 50;
let overheadAnimationFrame = maxOverheadAnimationFrame / 2;
let overheadAnimationIncrement = 1;

const uint32 = new Uint32Array(1);
const uint32ToUint8 = new Uint8Array(uint32.buffer);


// Define the type of the decoration. Must match ordering of the enum in CombatReplayDescription.cs
const Types = {
    ActorOrientation: 0,
    BackgroundIcon: 1,
    Circle: 2,
    Doughnut: 3,
    Friendly: 4,
    FriendlyPlayer: 5,
    Icon: 6,
    IconOverhead: 7,
    Line: 8,
    Mob: 9,
    MovingPlatform: 10,
    Pie: 11,
    Player: 12,
    ProgressBar: 13,
    ProgressBarOverhead: 14,
    Rectangle: 15,
    SquadMarker: 16,
    SquadMarkerOverhead: 17,
    Target: 18,
    TargetPlayer: 19,
    Text: 20,
    RegularPolygon: 21,
    TextOverhead: 22,
    Arena: 23,
    CustomPolygon: 24,
};

function getDefaultCombatReplayTime() {
    var time = EIUrlParams.get("crTime");
    if (!time) {
        return 0;
    }
    return Math.max(parseFloat(time), 0.0) * 1000;
}

let animator = null;
let animationControlComponent = null;
// reactive structures
const reactiveAnimationData = {
    time: getDefaultCombatReplayTime(),
    selectedActorID: null,
    animated: false,
    range: {
        min: 0,
        max: 1e12
    },
    selectedExtraDecorations: false,
    selectedMechanic: {
        actorId: null,
        actorName: null,
        name: null,
        times: [],
    }
};

var sliderDelimiter = {
    min: -1,
    max: -1,
    name: logData.phases[0].name
}
//

let InchToPixel = 10;
let PollingRate = 150;

// Scenegraph

function standardDraw(drawable) {
    drawable.draw();
}

function selectableDraw(drawable) {
    if (!drawable.isSelected()) {
        drawable.draw();
        animator._drawActorOrientation(drawable.id);
    }
}

function selectablePickingDraw(drawable) {
    if (!drawable.isSelected()) {
        drawable.drawPicking();
    }
}

class RenderablesBranch {
    constructor(start, end) {
        this.start = start;
        this.end = end;
        this.halfPoint = (end - start) * 0.5 + start;
        this.left = null;
        this.right = null;
        this.renderables = [];
        this.leaf = true;
        // Won't allow leaf below this
        this.finalLeaf = this.end - this.start < 10000;
    }

    add(item) {
        if (this.leaf) {
            this.renderables.push(item);
            // If too many renderables, remove leaf and redistribute
            if (this.renderables.length > 50 && !this.finalLeaf) {
                this.leaf = false;
                const renderablesToRedistribute = this.renderables;
                this.renderables = [];
                for (let i = 0; i < renderablesToRedistribute.length; i++) {
                    this.add(renderablesToRedistribute[i]);
                }
            }
            return;
        }
        if (item.end <= this.halfPoint) {
            if (!this.left) {
                this.left = new RenderablesBranch(this.start, this.halfPoint);
            }
            this.left.add(item);
        } else if (item.start > this.halfPoint && item.end <= this.end) {
            if (!this.right) {
                this.right = new RenderablesBranch(this.halfPoint, this.end);
            }
            this.right.add(item);
        } else {
            this.renderables.push(item);
        }
    }

    forEach(cb) {  
        for (let i = 0; i < this.renderables.length; i++) {
            cb(this.renderables[i]);
        }
        if (this.left) {
            this.left.forEach(cb);
        }
        if (this.right) {
            this.right.forEach(cb);
        }
    }

    draw(drawFunction) {
        var time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
            return;
        }
        for (let i = 0; i < this.renderables.length; i++) {
            drawFunction(this.renderables[i]);
        }
        if (this.left) {
            this.left.draw(drawFunction);
        }
        if (this.right) {
            this.right.draw(drawFunction);
        }
    }

    any()  {
        return this.renderables.length > 0 || this.left || this.right;
    }
}

class RenderablesRoot extends RenderablesBranch{
    constructor(start, end) {
        super(start, end);
        this._allRenderables = [];
    }

    add(item) {
        super.add(item);
        this._allRenderables.push(item);
    }
}

class MappedRenderablesRoot extends RenderablesRoot {
    constructor(start, end) {
        super(start, end);
        this.map = new Map();
    }

    add(item) {
        super.add(item);
        this.map.set(item.id, item);
    }

    get(id) {
        return this.map.get(id);
    }
    
    has(id) {
        return this.map.has(id);
    }
}

//

class RangeControl {
    constructor(radius) {
        this.enabled = false;
        this.radius = radius;
    }
}

class RangeControls {
    constructor() {
        this.ranges = [];
    }

    addRangeControl(radius) {
        this.ranges.push(new RangeControl(radius));
    }
}

class ConeControl {
    constructor(openingAngle, radius) {
        this.enabled = false;
        this.openingAngle = openingAngle;
        this.radius = radius;
    }
}

class Animator {
    constructor(options) {
        var _this = this;
        // status
        this.reactiveDataStatus = reactiveAnimationData;
        // time
        this.prevTime = 0;
        this.times = [];
        this.defaultViewpoints = [];
        // simulation params
        this.speed = 1;
        this.backwards = false;
        this.extraDecorationMap = new Map();
        this.displaySettings = {
            highlightSelectedGroup: true,
            displayAllMinions: false,
            displaySelectedMinions: true,
            displayMechanics: true,
            displaySquadMarkers: true,
            displaySkillMechanics: true,
            skillMechanicsMask: DefaultSkillDecorations,
            displayTrashMobs: true,
            useActorHitboxWidth: false,
            followSelected: false
        };
        this.selectedExtraDecorations = null;
        // actors
        const start = logData.phases[0].start * 1000;
        const end = logData.phases[0].end * 1000;
        this.targetData = new MappedRenderablesRoot(start, end);
        this.targetPlayerData = new MappedRenderablesRoot(start, end);
        this.playerData = new MappedRenderablesRoot(start, end);
        this.trashMobData = new MappedRenderablesRoot(start, end);
        this.friendlyMobData = new MappedRenderablesRoot(start, end);
        this.friendlyPlayerData = new MappedRenderablesRoot(start, end);
        this.decorationMetadata = new Map();
        this.overheadActorData = new RenderablesRoot(start, end);
        this.squadMarkerData = new RenderablesRoot(start, end);
        this.overheadSquadMarkerData = new RenderablesRoot(start, end);
        this.mechanicActorData = new RenderablesRoot(start, end);
        this.skillMechanicActorData = new RenderablesRoot(start, end);
        this.actorOrientationData = new Map();
        this.backgroundActorData = [];
        this.screenSpaceActorData = new RenderablesRoot(start, end);
        this.agentDataPerParentID = new Map();
        this.selectedActor = null;
        // maps
        this.backgroundImages = new RenderablesRoot(start, end);
        // animation
        this.needBGUpdate = false;
        this.animation = null;
        // manipulation
        this.mouseDown = null;
        this.dragged = false;
        this.scale = 1.0;
        // options
        if (options) {
            if (options.inchToPixel) {
                InchToPixel = options.inchToPixel;
            }
            if (options.pollingRate) {
                PollingRate = options.pollingRate;
            }
            if (options.actors) {
                this._initActors(options.actors, options.decorationRenderings, options.decorationMetadata);
            }
            if (options.defaultViewpoints) {
                for (let i = 0; i < options.defaultViewpoints.length; i++) {
                    this.defaultViewpoints.push({
                        tx: options.defaultViewpoints[i][0],
                        ty: options.defaultViewpoints[i][1],
                        s: options.defaultViewpoints[i][2],
                        eiid: options.defaultViewpoints[i][3]
                    });
                }
            }
            if (!replaceImgur) {
                downEnemyIcon.crossOrigin = "Anonymous";
                downAllyIcon.crossOrigin = "Anonymous";
                dcIcon.crossOrigin = "Anonymous";
                deadIcon.crossOrigin = "Anonymous";
            }
            downEnemyIcon.src = UIIcons.DownedEnemy;
            downAllyIcon.src = UIIcons.DownedAlly;
            dcIcon.src = UIIcons.Disconnected;
            deadIcon.src = UIIcons.Dead;
            facingIcon.src = UIIcons.Facing;
        }
        let cur = start;
        while (cur < end) {
            this.times.push(cur);
            cur += PollingRate;
        }
        this.reactiveDataStatus.time = start;
        this.reactiveDataStatus.range.min = this.times[0];
        this.reactiveDataStatus.range.max = this.times[this.times.length - 1];
    }

    attachDOM(mainCanvasID, bgCanvasID, pickCanvasID, timeRangeID, timeRangeDisplayID) {
        // animation
        this.timeSlider = document.getElementById(timeRangeID);
        this.timeSliderDisplay = document.getElementById(timeRangeDisplayID);
        // main canvas
        this.mainCanvas = document.getElementById(mainCanvasID);
        this.mainCanvas.style.width = this.mainCanvas.width + "px";
        this.mainCanvas.style.height = this.mainCanvas.height + "px";
        this.mainCanvas.width *= resolutionMultiplier;
        this.mainCanvas.height *= resolutionMultiplier;
        this.mainContext = this.mainCanvas.getContext('2d');
        this.mainContext.imageSmoothingEnabled = true;
        // bg canvas
        this.bgCanvas = document.getElementById(bgCanvasID);
        this.bgCanvas.style.width = this.bgCanvas.width + "px";
        this.bgCanvas.style.height = this.bgCanvas.height + "px";
        this.bgCanvas.width *= resolutionMultiplier;
        this.bgCanvas.height *= resolutionMultiplier;
        this.bgContext = this.bgCanvas.getContext('2d');
        this.bgContext.imageSmoothingEnabled = true;
        // pick canvas
        this.pickCanvas = document.getElementById(pickCanvasID);
        this.pickCanvas.style.width = this.pickCanvas.width + "px";
        this.pickCanvas.style.height = this.pickCanvas.height + "px";
        this.pickCanvas.width *= resolutionMultiplier;
        this.pickCanvas.height *= resolutionMultiplier;
        this.pickContext = this.pickCanvas.getContext('2d', {
            willReadFrequently: true,
        });
        // manipulation
        this.lastX = this.mainCanvas.width / 2;
        this.lastY = this.mainCanvas.height / 2;
        //
        this._trackTransforms(this.mainContext);
        this._trackTransforms(this.bgContext);
        this._trackTransforms(this.pickContext);
        this.mainContext.scale(resolutionMultiplier, resolutionMultiplier);
        this.bgContext.scale(resolutionMultiplier, resolutionMultiplier);
        this.pickContext.scale(resolutionMultiplier, resolutionMultiplier);
        this._initMouseEvents();
        this._initTouchEvents();
    }

    _initActors(actors, decorationRenderings, decorationMetadata) {
        for (let i = 0; i < decorationMetadata.length; i++) {
            const metadata = decorationMetadata[i];
            let MetadataClass = null;
            switch (metadata.type) {
                case Types.ActorOrientation:
                    MetadataClass = ActorOrientationMetadata;
                    break;
                case Types.Circle:
                    MetadataClass = CircleMetadata;
                    break;
                case Types.RegularPolygon:
                    MetadataClass = RegularPolygonMetadata;
                    break;
                case Types.CustomPolygon:
                    MetadataClass = CustomPolygonMetadata;
                    break;
                case Types.Doughnut:
                    MetadataClass = DoughnutMetadata;
                    break;
                case Types.Line:
                    MetadataClass = LineMetadata;
                    break;
                case Types.Pie:
                    MetadataClass = PieMetadata;
                    break;
                case Types.Rectangle:
                    MetadataClass = RectangleMetadata;
                    break;
                case Types.ProgressBar:
                    MetadataClass = ProgressBarMetadata;
                    break;
                case Types.BackgroundIcon:
                    MetadataClass = IconMetadata;
                    break;
                case Types.Icon:
                    MetadataClass = IconMetadata;
                    break;
                case Types.IconOverhead:
                    MetadataClass = IconOverheadMetadata;
                    break;
                case Types.ProgressBarOverhead:
                    MetadataClass = OverheadProgressBarMetadata;
                    break;
                case Types.MovingPlatform:
                    MetadataClass = MovingPlatformMetadata;
                    break;
                case Types.Text:
                    MetadataClass = TextMetadata;
                    break;
                case Types.TextOverhead:
                    MetadataClass = TextOverheadMetadata;
                    break;
                case Types.Arena:
                    MetadataClass = ArenaMetadata;
                    break;
                default:
                    throw "Unknown decoration type " + metadata.type;
            }
            this.decorationMetadata.set(metadata.signature, new MetadataClass(metadata));
        }
        for (let i = 0; i < actors.length; i++) {
            const actor = actors[i];
            let ActorClass;
            let actorSize = 0;
            let mapToFill;
            switch (actor.type) {
                case Types.Player:
                    ActorClass = PlayerIconDrawable;
                    actorSize = 22;
                    mapToFill = this.playerData;
                    break;
                case Types.Target:
                    ActorClass = NPCIconDrawable;
                    actorSize = 30;
                    mapToFill = this.targetData;
                    break;
                case Types.TargetPlayer:
                    ActorClass = EnemyPlayerDrawable;
                    actorSize = 22;
                    mapToFill = this.targetPlayerData;
                    break;
                case Types.Mob:
                    ActorClass = NPCIconDrawable;
                    actorSize = 25;
                    mapToFill = this.trashMobData;
                    break;
                case Types.Friendly:
                    ActorClass = NPCIconDrawable;
                    actorSize = 22;
                    mapToFill = this.friendlyMobData;
                    break;
                case Types.FriendlyPlayer:
                    ActorClass = FriendlyPlayerDrawable;
                    actorSize = 22;
                    mapToFill = this.friendlyPlayerData;
                    break;
                default:
                    throw "Unknown decoration type " + actor.type;
            }
            const renderable = new ActorClass(actor, actorSize);
            mapToFill.add(renderable);
            if (renderable.parentID >= 0) {
                let array = this.agentDataPerParentID.get(renderable.parentID) ?? [];
                array.push(renderable);
                this.agentDataPerParentID.set(renderable.parentID, array);
            }
        }
        for (let i = 0; i < decorationRenderings.length; i++) {
            const decorationRendering = {};
            decorationRendering._metadataContainer = this.decorationMetadata;
            Object.assign(decorationRendering, decorationRenderings[i]);
            if (!decorationRendering.isMechanicOrSkill) {
                switch (decorationRendering.type) {
                    case Types.ActorOrientation:
                        let orientationID = decorationRendering.connectedTo.masterID;
                        var orientationDrawable = new ActorOrientationDrawable(decorationRendering);
                        if (this.agentDataPerParentID.has(orientationID)) {
                            let halfTime = (orientationDrawable.start + orientationDrawable.end) / 2;
                            let agents = this.agentDataPerParentID.get(orientationID);
                            for (let i = 0; i < agents.length; i++) {
                                let agent = agents[i];
                                if (agent.start <= halfTime && agent.end >= halfTime) {
                                    this.actorOrientationData.set(agents[i].id, orientationDrawable);
                                    break;
                                }
                            }
                        } else {
                            this.actorOrientationData.set(orientationID, orientationDrawable);
                        }
                        break;
                    case Types.MovingPlatform:
                        this.backgroundActorData.push(new MovingPlatformDrawable(decorationRendering));
                        break;
                    case Types.BackgroundIcon:
                        this.backgroundActorData.push(new BackgroundIconMechanicDrawable(decorationRendering));
                        break;
                    case Types.Arena:
                        this.backgroundImages.add(new ArenaDrawable(decorationRendering));
                        break;
                    default:
                        throw "Unknown decoration type " + decorationRendering.type;
                }
            } else {
                let DecorationClass;
                switch (decorationRendering.type) {
                    case Types.Text:
                        if (decorationRendering.connectedTo.isScreenSpace) {
                            this.screenSpaceActorData.add(new TextDrawable(decorationRendering));
                            continue;
                        }
                        DecorationClass = TextDrawable;
                        break;
                    case Types.Circle:
                        DecorationClass = CircleMechanicDrawable;
                        break;
                    case Types.RegularPolygon:
                        DecorationClass = RegularPolygonMechanicDrawable;
                        break;
                    case Types.CustomPolygon:
                        DecorationClass = CustomPolygonMechanicDrawable;
                        break;
                    case Types.Rectangle:
                        DecorationClass = RectangleMechanicDrawable;
                        break;
                    case Types.ProgressBar:
                        DecorationClass = ProgressBarMechanicDrawable;
                        break;
                    case Types.Doughnut:
                        DecorationClass = DoughnutMechanicDrawable;
                        break;
                    case Types.Pie:
                        DecorationClass = PieMechanicDrawable;
                        break;
                    case Types.Line:
                        DecorationClass = LineMechanicDrawable;
                        break;
                    case Types.Icon:
                        DecorationClass = IconMechanicDrawable;
                        break;
                    // Special cases
                    case Types.TextOverhead:
                        this.overheadActorData.add(new TextOverheadDrawable(decorationRendering));
                        continue;
                    case Types.IconOverhead:
                        this.overheadActorData.add(new IconOverheadMechanicDrawable(decorationRendering));
                        continue;
                    case Types.ProgressBarOverhead:
                        this.overheadActorData.add(new OverheadProgressBarMechanicDrawable(decorationRendering));
                        continue;
                    case Types.SquadMarker:
                        this.squadMarkerData.add(new IconMechanicDrawable(decorationRendering));
                        continue;
                    case Types.SquadMarkerOverhead:
                        this.overheadSquadMarkerData.add(new IconOverheadMechanicDrawable(decorationRendering));
                        continue;
                    default:
                        throw "Unknown decoration type " + decorationRendering.type;
                }
                const decoration = new DecorationClass(decorationRendering);
                if (decorationRendering.skillMode) {
                    this.skillMechanicActorData.add(decoration);
                } else {
                    this.mechanicActorData.add(decoration);
                }
            }
        }
    }

    updateRange(phase) {
        let min = Math.max(this.times[0], phase.start * 1000);
        let max = Math.min(this.times[this.times.length - 1], phase.end * 1000);
        this.reactiveDataStatus.range.min = min;
        this.reactiveDataStatus.range.max = max;
    }

    updateTime(value) {
        this.reactiveDataStatus.time = parseInt(value);
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }

    updateTextInput() {
        this.timeSliderDisplay.value = ((this.reactiveDataStatus.time - this.reactiveDataStatus.range.min) / 1000.0).toFixed(3);
    }

    updateInputTime(value) {
        try {
            const cleanedString = value.replace(",", ".");
            const parsedTime = parseFloat(cleanedString);
            if (isNaN(parsedTime) || !isFinite(parsedTime)) {
                return;
            }
            const ms = Math.round(parsedTime * 1000.0);
            const min = this.reactiveDataStatus.range.min;
            const max = this.reactiveDataStatus.range.max;
            this.reactiveDataStatus.time = Math.min(Math.max(ms, min), max);
            animateCanvas(updateText);
        } catch (error) {
            console.error(error);
        }
    }

    toggleAnimate() {
        if (!this.startAnimate(true)) {
            this.stopAnimate(true);
        }
    }

    startAnimate(updateReactiveStatus) {
        if (this.animation === null && this.times.length > 0) {
            const max = this.reactiveDataStatus.range.max;
            const min = this.reactiveDataStatus.range.min;
            if (this.reactiveDataStatus.time >= max && !this.backwards) {
                this.reactiveDataStatus.time = min;
            }
            this.prevTime = new Date().getTime();
            this.animation = requestAnimationFrame(animateCanvas);
            if (updateReactiveStatus) {
                this.reactiveDataStatus.animated = true;
            }
            return true;
        }
        return false;
    }

    stopAnimate(updateReactiveStatus) {
        if (this.animation !== null) {
            window.cancelAnimationFrame(this.animation);
            this.animation = null;
            if (updateReactiveStatus) {
                this.reactiveDataStatus.animated = false;
            }
            return true;
        }
        return false;
    }

    restartAnimate() {
        this.reactiveDataStatus.time = this.reactiveDataStatus.range.min;
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }

    _hideExtraDecorations() {   
        this.selectedExtraDecorations = null;
        $('#circle1Text').val(180);
        $('#circle1Check').prop('checked', false);
        $('#circle2Text').val(360);
        $('#circle2Check').prop('checked', false);
        $('#circle3Text').val(720);
        $('#circle3Check').prop('checked', false);
        $('#coneRadiusText').val(360);
        $('#coneAngleText').val(90);
        $('#coneCheck').prop('checked', false);
        this.reactiveDataStatus.selectedExtraDecorations = false;
    }

    _setCurrentExtraDecorations() {
        this.selectedExtraDecorations = this.extraDecorationMap.get(this.reactiveDataStatus.selectedActorID);
        if (!this.selectedExtraDecorations) {
            this.selectedExtraDecorations = {
                rangeControls: new RangeControls(),
                coneControl: new ConeControl(90, 360),
            };
            this.selectedExtraDecorations.rangeControls.addRangeControl(180);
            this.selectedExtraDecorations.rangeControls.addRangeControl(360);
            this.selectedExtraDecorations.rangeControls.addRangeControl(720);
            this.extraDecorationMap.set(this.reactiveDataStatus.selectedActorID, this.selectedExtraDecorations);
        }
        $('#circle1Text').val(this.selectedExtraDecorations.rangeControls.ranges[0].radius);
        $('#circle1Check').prop('checked', this.selectedExtraDecorations.rangeControls.ranges[0].enabled);
        $('#circle2Text').val(this.selectedExtraDecorations.rangeControls.ranges[1].radius);
        $('#circle2Check').prop('checked', this.selectedExtraDecorations.rangeControls.ranges[1].enabled);
        $('#circle3Text').val(this.selectedExtraDecorations.rangeControls.ranges[2].radius);
        $('#circle3Check').prop('checked', this.selectedExtraDecorations.rangeControls.ranges[2].enabled);
        $('#coneRadiusText').val(this.selectedExtraDecorations.coneControl.radius);
        $('#coneAngleText').val(this.selectedExtraDecorations.coneControl.openingAngle);
        $('#coneCheck').prop('checked', this.selectedExtraDecorations.coneControl.enabled);
        this.reactiveDataStatus.selectedExtraDecorations = true;
    }

    selectActor(actorId, keepIfEqual = false) {
        if (DEBUG) {
            const inLogActor = logData.players.filter(x => x.uniqueID === actorId)[0] || logData.targets.filter(x => x.uniqueID === actorId)[0];
            if (inLogActor) {
                alert(actorId + " " + inLogActor.name)
            } else {
                alert(actorId);
            }
        }
        let actor = this.getActorData(actorId);
        if (!actor || (!keepIfEqual && this.selectedActor === actor)) {
            this.selectedActor = null;
            this.reactiveDataStatus.selectedActorID = null;
            this._hideExtraDecorations();
        } else {
            this.selectedActor = actor;
            this.reactiveDataStatus.selectedActorID = actorId;
            this._setCurrentExtraDecorations();
        }
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }
    
    _reselectIfEnglobed() {     
        if (this.selectedActor && this.selectedActor.parentID >= 0) {
            const perParentArray = this.agentDataPerParentID.get(this.selectedActor.parentID);
            if (perParentArray) {
                let actor = perParentArray.filter(x => x.getPosition() != null)[0];
                if (!actor) {
                    const time = this.reactiveDataStatus.time;
                    // check for first in interval
                    let candidates = perParentArray.filter(x => x.start <= time && x.end >= time);
                    if (candidates.length) {
                        actor = candidates[0];
                    } else {
                        // first
                        candidates = perParentArray.filter(x => x.start >= time);
                        if (candidates.length) {
                            actor = candidates[0];
                        } else {
                            // last
                            candidates = perParentArray.filter(x => x.end <= time);
                            if (candidates.length) {
                                actor = candidates[candidates.length - 1];
                            }
                        }
                    }
                }
                this.selectedActor = actor || this.selectedActor;
                this.reactiveDataStatus.selectedActorID = this.selectedActor.id;             
            }
        }
    }

    getSelectableActorData(actorId) {
        return animator.targetData.get(actorId) || animator.playerData.get(actorId) || 
                animator.friendlyMobData.get(actorId) || animator.friendlyPlayerData.get(actorId) || 
                animator.targetPlayerData.get(actorId);
    }

    getActorData(actorId) {
        return this.getSelectableActorData(actorId) || animator.trashMobData.get(actorId);
    }

    getActiveActorMarkers(actorID) {
        let res = [];
        const _this = this;
        this.overheadSquadMarkerData.forEach((marker) => {
            if (marker.canDraw() && marker.getPosition() && marker.master === _this.getActorData(actorID)) {
                res.push(marker);
            }
        });
        return res;
    }

    toggleFollowSelected() {
        this.displaySettings.followSelected = !this.displaySettings.followSelected;
        animateCanvas(noUpdateTime);
    }

    toggleHighlightSelectedGroup() {
        this.displaySettings.highlightSelectedGroup = !this.displaySettings.highlightSelectedGroup;
        animateCanvas(noUpdateTime);
    }

    toggleDisplayAllMinions() {
        this.displaySettings.displayAllMinions = !this.displaySettings.displayAllMinions;
        animateCanvas(noUpdateTime);
    }

    toggleDisplaySelectedMinions() {
        this.displaySettings.displaySelectedMinions = !this.displaySettings.displaySelectedMinions;
        animateCanvas(noUpdateTime);
    }

    toggleUseActorHitboxWidth() {
        this.displaySettings.useActorHitboxWidth = !this.displaySettings.useActorHitboxWidth;
        animateCanvas(noUpdateTime);
    }

    toggleTrashMobs() {
        this.displaySettings.displayTrashMobs = !this.displaySettings.displayTrashMobs;
        animateCanvas(noUpdateTime);
    }

    toggleMechanics() {
        this.displaySettings.displayMechanics = !this.displaySettings.displayMechanics;
        animateCanvas(noUpdateTime);
    }

    toggleSquadMarkers() {
        this.displaySettings.displaySquadMarkers = !this.displaySettings.displaySquadMarkers;
        animateCanvas(noUpdateTime);
    }

    toggleSkills() {
        this.displaySettings.displaySkillMechanics = !this.displaySettings.displaySkillMechanics;
        animateCanvas(noUpdateTime);
    }

    toggleSkillCategoryMask(mask) {
        if ((this.displaySettings.skillMechanicsMask & mask) > 0) {
            this.displaySettings.skillMechanicsMask &= ~mask;
        } else {
            this.displaySettings.skillMechanicsMask |= mask;
        }
        animateCanvas(noUpdateTime);
    }

    toggleConeDisplay(iOnOff) {
        if (!this.selectedExtraDecorations) {
            return;
        }
        this.selectedExtraDecorations.coneControl.enabled = iOnOff;
        animateCanvas(noUpdateTime);
    }

    setConeRadius(value) {
        if (!this.selectedExtraDecorations) {
            return;
        }
        this.selectedExtraDecorations.coneControl.radius = value;
        animateCanvas(noUpdateTime);
    }

    setConeAngle(value) {
        if (!this.selectedExtraDecorations) {
            return;
        }
        this.selectedExtraDecorations.openingAngle.radius = value;
        animateCanvas(noUpdateTime);
    }

    resetViewpoint(eiid = 0) {
        var canvas = this.mainCanvas;
        var ctx = this.mainContext;
        var bgCtx = this.bgContext;

        this.mouseDown = null;
        this.dragged = false;

        let defaultViewpoint = this.defaultViewpoints.filter(x => x.eiid === eiid)[0];
        this.lastX = canvas.width / 2;
        this.lastY = canvas.height / 2;
        if (defaultViewpoint) {
            var x = -canvas.width * defaultViewpoint.tx / 100;
            var y = -canvas.height * defaultViewpoint.ty / 100;
            
            ctx.setTransform(1, 0, 0, 1, x, y);
            bgCtx.setTransform(1, 0, 0, 1, x, y);
        } else {
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            bgCtx.setTransform(1, 0, 0, 1, 0, 0);
        }
        ctx.scale(resolutionMultiplier, resolutionMultiplier);
        bgCtx.scale(resolutionMultiplier, resolutionMultiplier);
        if (defaultViewpoint) {
            this._setScaleOnPoint(defaultViewpoint.s, 0, 0);
        }
        this.needBGUpdate = true;
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }
    _setScaleOnPoint(factor, ptX, ptY) {
        const ctx = this.mainContext;
        const bgCtx = this.bgContext;

        const pt = ctx.transformedPoint(ptX, ptY);
        ctx.translate(pt.x, pt.y);
        bgCtx.translate(pt.x, pt.y);
        ctx.scale(factor, factor);
        if ((50 / (InchToPixel * this.scale) < 10)) {
            ctx.scale(1.0 / factor, 1.0 / factor);
            factor = 1.0;
        }
        ctx.translate(-pt.x, -pt.y);
        bgCtx.scale(factor, factor);
        bgCtx.translate(-pt.x, -pt.y);
        this.needBGUpdate = true;
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }
    _initMouseEvents() {
        const _this = this;
        const canvas = this.mainCanvas;
        const ctx = this.mainContext;
        const bgCtx = this.bgContext;
        const pickCtx = this.pickContext;

        canvas.addEventListener('mousedown', function (evt) {
            evt.preventDefault();
            _this.lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
            _this.lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
            _this.mouseDown = {
                pt: ctx.transformedPoint(_this.lastX, _this.lastY),
                time: Date.now()
            }
            _this.dragged = false;
        }, false);

        canvas.addEventListener('mousemove', function (evt) {
            evt.preventDefault();
            _this.lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
            _this.lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
            _this.dragged = true;
            if (_this.mouseDown) {
                const pt = ctx.transformedPoint(_this.lastX, _this.lastY);
                const downPt = _this.mouseDown.pt;
                ctx.translate(pt.x - downPt.x, pt.y - downPt.y);
                bgCtx.translate(pt.x - downPt.x, pt.y - downPt.y);
                _this.needBGUpdate = true;
                if (_this.animation === null) {
                    animateCanvas(noUpdateTime);
                }
            }
        }, false);

        document.body.addEventListener('mouseup', function (evt) {
            if (_this.mouseDown && Date.now() - _this.mouseDown.time < 150) {
                _this._drawPickCanvas();
                const downPt = {
                    x: Math.round(_this.lastX * resolutionMultiplier),
                    y: Math.round(_this.lastY * resolutionMultiplier)
                };
                const pickedColor = pickCtx.getImageData(downPt.x, downPt.y, 1, 1).data;
                uint32ToUint8[0] = pickedColor[0];
                uint32ToUint8[1] = pickedColor[1];
                uint32ToUint8[2] = pickedColor[2];
                uint32ToUint8[3] = 0;
                const actorID = uint32[0];
                _this.selectActor(actorID, true);
            }
            _this.mouseDown = null;
        }, false);

        var zoom = function (evt) {
            evt.preventDefault();
            const delta = evt.wheelDelta ? evt.wheelDelta / 40 : evt.detail ? -evt.detail : 0;
            if (delta) {
                const factor = Math.pow(1.1, delta);
                _this._setScaleOnPoint(factor, _this.lastX, _this.lastY);
            }
        };

        canvas.addEventListener('DOMMouseScroll', zoom, false);
        canvas.addEventListener('mousewheel', zoom, false);
    }

    _initTouchEvents() {
        // todo
    }

    setSpeed(value) {
        this.speed = value;
    }

    getSpeed() {
        if (this.backwards) {
            return -this.speed;
        }
        return this.speed;
    }

    toggleBackwards() {
        this.backwards = !this.backwards;
        return this.backwards;
    }

    toggleRange(index, iOnOff) {
        if (!this.selectedExtraDecorations) {
            return;
        }
        this.selectedExtraDecorations.rangeControls.ranges[index].enabled = iOnOff;
        animateCanvas(noUpdateTime);
    }

    setRangeRadius(index, value) {
        if (!this.selectedExtraDecorations) {
            return;
        }
        this.selectedExtraDecorations.rangeControls.ranges[index].radius = value;
        animateCanvas(noUpdateTime);
    }

    // https://codepen.io/anon/pen/KrExzG
    _trackTransforms(ctx) {
        var svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
        var xform = svg.createSVGMatrix();
        ctx.getTransform = function () {
            return xform;
        };

        var drawImage = ctx.drawImage;
        ctx.drawImage = function() {
            const image = arguments[0];
            if (!image || !image.complete || image.naturalWidth === 0) {
                return;
            }
            return drawImage.call(ctx, ...arguments);
        }

        var savedTransforms = [];
        var save = ctx.save;
        ctx.save = function () {
            savedTransforms.push(xform.translate(0, 0));
            return save.call(ctx);
        };

        var restore = ctx.restore;
        ctx.restore = function () {
            xform = savedTransforms.pop();
            return restore.call(ctx);
        };

        var scale = ctx.scale;
        var _this = this;
        ctx.scale = function (sx, sy) {
            xform = xform.scale(sx, sy);
            var xAxis = Math.sqrt(xform.a * xform.a + xform.b * xform.b);
            var yAxis = Math.sqrt(xform.c * xform.c + xform.d * xform.d);
            _this.scale = Math.max(xAxis, yAxis) / resolutionMultiplier;
            return scale.call(ctx, sx, sy);
        };
        

        var rotate = ctx.rotate;
        ctx.rotate = function (radians) {
            xform = xform.rotate(radians * 180 / Math.PI);
            return rotate.call(ctx, radians);
        };

        var translate = ctx.translate;
        ctx.translate = function (dx, dy) {
            xform = xform.translate(dx, dy);
            return translate.call(ctx, dx, dy);
        };

        var transform = ctx.transform;
        ctx.transform = function (a, b, c, d, e, f) {
            var m2 = svg.createSVGMatrix();
            m2.a = a;
            m2.b = b;
            m2.c = c;
            m2.d = d;
            m2.e = e;
            m2.f = f;
            xform = xform.multiply(m2);
            return transform.call(ctx, a, b, c, d, e, f);
        };

        var setTransform = ctx.setTransform;
        ctx.setTransform = function (a, b, c, d, e, f) {
            xform.a = a;
            xform.b = b;
            xform.c = c;
            xform.d = d;
            xform.e = e;
            xform.f = f;
            return setTransform.call(ctx, a, b, c, d, e, f);
        };

        var pt = svg.createSVGPoint();
        ctx.transformedPoint = function (x, y) {
            pt.x = x * resolutionMultiplier;
            pt.y = y * resolutionMultiplier;
            return pt.matrixTransform(xform.inverse());
        };
    }
    // animation
    _drawBGCanvas() {
        const _this = this;
        if (!this.needBGUpdate) {
            this.backgroundImages.forEach(x => {
                if (x.needsUpdate()) {
                    _this.needBGUpdate = true;
                }
            });
        }
        if (this.needBGUpdate || this._mustMoveToSelected()) {
            this.needBGUpdate = false;
            var ctx = this.bgContext;
            var canvas = this.bgCanvas;
            var p1 = ctx.transformedPoint(0, 0);
            var p2 = ctx.transformedPoint(canvas.width, canvas.height);
            ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

            ctx.save();
            {
                ctx.setTransform(1, 0, 0, 1, 0, 0);
                ctx.clearRect(0, 0, canvas.width, canvas.height);
            }
            ctx.restore();

            //ctx.save();
            {

                this._moveToSelected(ctx);
                this.backgroundImages.draw(standardDraw);
                //ctx.globalCompositeOperation = "color-burn";
                ctx.save();
                {
                    ctx.setTransform(1, 0, 0, 1, 0, 0);
                    // draw scale
                    ctx.lineWidth = 3 * resolutionMultiplier;
                    ctx.strokeStyle = "#CC2200";
                    var pos = resolutionMultiplier * 70;
                    var width = resolutionMultiplier * 50;
                    var height = resolutionMultiplier * 6;
                    // main line
                    ctx.beginPath();
                    ctx.moveTo(pos, pos);
                    ctx.lineTo(pos + width, pos);
                    ctx.stroke();
                    ctx.lineWidth = 2 * resolutionMultiplier;
                    // right border
                    ctx.beginPath();
                    ctx.moveTo(pos - resolutionMultiplier, pos + height);
                    ctx.lineTo(pos - resolutionMultiplier, pos - height);
                    ctx.stroke();
                    // left border
                    ctx.beginPath();
                    ctx.moveTo(pos + width + resolutionMultiplier, pos + height);
                    ctx.lineTo(pos + width + resolutionMultiplier, pos - height);
                    ctx.stroke();
                    // text
                    var fontSize = 13 * resolutionMultiplier;
                    ctx.font = "bold " + fontSize + "px Comic Sans MS";
                    ctx.fillStyle = "#CC2200";
                    ctx.textAlign = "center";
                    ctx.fillText((50 / (InchToPixel * this.scale)).toFixed(1) + " units", resolutionMultiplier * 95, resolutionMultiplier * 60);
                }
                ctx.restore();
            }
            //ctx.restore();
            //ctx.globalCompositeOperation = 'normal';
        }
    }

    _drawActorOrientation(key) {
        if (this.actorOrientationData.has(key)) {
            this.actorOrientationData.get(key).draw();
        }
    }

    _drawPickCanvas() {
        var _this = this;
        var mainCtx = this.mainContext;
        var mainTransform = mainCtx.getTransform();
        var ctx = this.pickContext;
        var canvas = this.pickCanvas;
        var p1 = ctx.transformedPoint(0, 0);
        var p2 = ctx.transformedPoint(canvas.width, canvas.height);
        ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);
        ctx.save();
        {
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.clearRect(0, 0, canvas.width, canvas.height);
        }
        ctx.restore();

        //ctx.save();
        {
            ctx.setTransform(mainTransform.a, mainTransform.b, mainTransform.c, mainTransform.d, mainTransform.e, mainTransform.f);


            if (!this.displaySettings.useActorHitboxWidth) {
                this.friendlyMobData.draw(selectablePickingDraw);
                this.friendlyPlayerData.draw(selectablePickingDraw);
                this.playerData.draw(selectablePickingDraw);
            }

            if (this.displaySettings.displayTrashMobs) {
                this.trashMobData.draw(selectablePickingDraw);
            }

            this.targetData.draw(selectablePickingDraw);
            this.targetPlayerData.draw(selectablePickingDraw);
            if (this.displaySettings.useActorHitboxWidth) {
                this.friendlyMobData.draw(selectablePickingDraw);
                this.friendlyPlayerData.draw(selectablePickingDraw);
                this.playerData.draw(selectablePickingDraw);
            }
            if (this.selectedActor !== null) {
                this.selectedActor.drawPicking();
            }
        }

        //ctx.restore();
    }

    _drawMainCanvas() {
        var _this = this;
        var ctx = this.mainContext;
        var canvas = this.mainCanvas;
        var p1 = ctx.transformedPoint(0, 0);
        var p2 = ctx.transformedPoint(canvas.width, canvas.height);
        ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);
        ctx.save();
        {
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.clearRect(0, 0, canvas.width, canvas.height);
        }
        ctx.restore();
        //ctx.save();
        {

            this._moveToSelected(ctx);
            // Background items commonly overlap so they need to be drawn in the correct order by height
            // This is sorted in reverse order because the z axis is inverted
            animator.backgroundActorData.sort((x, y) => y.getHeight() - x.getHeight());
            for (let i = 0; i < animator.backgroundActorData.length; i++) {
                animator.backgroundActorData[i].draw();
            }
            if (this.displaySettings.displayMechanics) {
                this.mechanicActorData.draw(standardDraw);
            }

            if (this.displaySettings.displaySkillMechanics) {
                this.skillMechanicActorData.draw(standardDraw);
            }


            if (!this.displaySettings.useActorHitboxWidth) {
                this.friendlyMobData.draw(selectableDraw);
                this.friendlyPlayerData.draw(selectableDraw);
                this.playerData.draw(selectableDraw);
            }

            if (this.displaySettings.displayTrashMobs) {
                this.trashMobData.draw(selectableDraw);
            }

            this.targetData.draw(selectableDraw);
            this.targetPlayerData.draw(selectableDraw);
            if (this.displaySettings.useActorHitboxWidth) {
                this.friendlyMobData.draw(selectableDraw);
                this.friendlyPlayerData.draw(selectableDraw);
                this.playerData.draw(selectableDraw);
            }
            if (this.selectedActor !== null) {
                this.selectedActor.draw();
                this._drawActorOrientation(this.selectedActor.id);
            }
            if (this.displaySettings.displayMechanics) {
                this.overheadActorData.draw(standardDraw);
            }
            if (this.displaySettings.displaySquadMarkers) {
                this.squadMarkerData.draw(standardDraw);
                this.overheadSquadMarkerData.draw(standardDraw);
            }
            ctx.save();
            {
                ctx.setTransform(1, 0, 0, 1, 0, 0);
                // Screen space actors
                this.screenSpaceActorData.draw(standardDraw);
            }
            ctx.restore()
        }
        //ctx.restore();  
    }

    _mustMoveToSelected() {
        return this.displaySettings.followSelected && this.selectedActor !== null && this.selectedActor.canDraw();
    }

    _moveToSelected(ctx) {

        if (this._mustMoveToSelected()) {
            const pos = this.selectedActor.getPosition();
            if (pos !== null) {
                ctx.setTransform(1, 0, 0, 1, 0, 0);
                ctx.scale(this.scale * resolutionMultiplier, this.scale * resolutionMultiplier);
                const translateScale = 0.5 / resolutionMultiplier / this.scale
                ctx.translate(-pos.x + this.mainCanvas.width * translateScale, -pos.y + this.mainCanvas.height * translateScale);
            }
        }
    }
    draw() {
        if (!this.mainCanvas) {
            return;
        }    
        this._reselectIfEnglobed();
        //
        //this._drawPickCanvas();
        this._drawBGCanvas();
        this._drawMainCanvas();
        if (overheadAnimationFrame === maxOverheadAnimationFrame || overheadAnimationFrame === 0) {
            overheadAnimationIncrement *= -1;
        }
        overheadAnimationFrame += overheadAnimationIncrement;
    }
}

function animateCanvas(noRequest) {
    if (animator == null) {
        return;
    }
    let lastTime = animator.reactiveDataStatus.range.max;
    let firstTime = animator.reactiveDataStatus.range.min;
    if (noRequest > noUpdateTime && animator.animation !== null) {
        let curTime = new Date().getTime();
        let timeOffset = curTime - animator.prevTime;
        animator.prevTime = curTime;
        animator.reactiveDataStatus.time = Math.round(Math.max(Math.min(animator.reactiveDataStatus.time + animator.getSpeed() * timeOffset, lastTime), 0));
    }
    if ((animator.reactiveDataStatus.time === lastTime && !animator.backwards) || (animator.reactiveDataStatus.time === firstTime && animator.backwards)) {
        animator.stopAnimate(true);
    }
    animator.timeSlider.value = (animator.reactiveDataStatus.time - animator.reactiveDataStatus.range.min).toString()
    if (noRequest > updateText) {
        animator.updateTextInput();
    }
    animator.draw();
    if (noRequest > noUpdateTime && animator.animation !== null) {
        animator.animation = requestAnimationFrame(animateCanvas);
    }
}
/*
function initCombatReplay(actors, options) {
    // manipulation events
    canvas.addEventListener('touchstart', function (evt) {
        var touch = evt.changedTouches[0];
        if (!touch) {
            return;
        }
        lastX = (touch.pageX - canvas.offsetLeft);
        lastY = (touch.pageY - canvas.offsetTop);
        mouseDown = ctx.transformedPoint(lastX, lastY);
        dragged = false;
        return evt.preventDefault() && false;
    }, false);

    canvas.addEventListener('touchmove', function (evt) {
        var touch = evt.changedTouches[0];
        if (!touch) {
            return;
        }
        lastX = (touch.pageX - canvas.offsetLeft);
        lastY = (touch.pageY - canvas.offsetTop);
        dragged = true;
        if (mouseDown) {
            var pt = ctx.transformedPoint(lastX, lastY);
            ctx.translate(pt.x - mouseDown.x, pt.y - mouseDown.y);
            animateCanvas(noUpdateTime);
        }
        return evt.preventDefault() && false;
    }, false);
    document.body.addEventListener('touchend', function (evt) {
        mouseDown = null;
    }, false);
}
*/
