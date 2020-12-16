/*jshint esversion: 6 */
// const images
"use strict";
const noUpdateTime = -1;
const updateText = -2;
const deadIcon = new Image();
deadIcon.onload = function () {
    animateCanvas(noUpdateTime);
};
const downIcon = new Image();
downIcon.onload = function () {
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

const resolutionMultiplier = window.devicePixelRatio;

var animator = null;
// reactive structures
var reactiveAnimationData = {
    time: 0,
    selectedPlayer: null,
    selectedPlayerID: null,
    animated: false
};

var sliderDelimiter = {
    min: -1,
    max: -1,
    name: logData.phases[0].name
}
//

class Animator {
    constructor(options) {
        var _this = this;
        // status
        this.reactiveDataStatus = reactiveAnimationData;
        // time
        this.prevTime = 0;
        this.times = [];
        // simulation params
        this.inch = 10;
        this.pollingRate = 150;
        this.speed = 1;
        this.backwards = false;
        this.rangeControl = [{ enabled: false, radius: 180 }, { enabled: false, radius: 360 }, { enabled: false, radius: 720 }];
        this.highlightSelectedGroup = true;
        this.displayMechanics = true;
        this.displayTrashMobs = true;
        this.selectedGroup = -1;
        this.coneControl = {
            enabled: false,
            openingAngle: 90,
            radius: 360,
        }
        // actors
        this.targetData = new Map();
        this.playerData = new Map();
        this.trashMobData = new Map();
        this.mechanicActorData = [];
        this.attachedActorData = new Map();
        this.backgroundActorData = [];
        this.backgroundImages = [];
        // animation
        this.needBGUpdate = false;
        this.prevBGImage = null;
        this.animation = null;
        // manipulation
        this.dragStart = null;
        this.dragged = false;
        this.scale = 1.0;
        // options
        if (options) {
            if (options.inch) this.inch = options.inch;
            if (options.pollingRate) this.pollingRate = options.pollingRate;
            if (options.maps) {
                for (var i = 0; i < options.maps.length; i++) {
                    var mapData = options.maps[i];
                    var image = new Image();
                    image.onload = function () {
                        _this.needBGUpdate = true;
                        animateCanvas(noUpdateTime);
                    };
                    image.src = mapData.link;
                    this.backgroundImages.push({
                        image: image,
                        start: mapData.start,
                        end: mapData.end
                    });
                }
            }
            if (options.actors) this._initActors(options.actors);
            downIcon.src = "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
            dcIcon.src = "https://wiki.guildwars2.com/images/f/f5/Talk_end_option_tango.png";
            deadIcon.src = "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
            facingIcon.src = "https://i.imgur.com/tZTmTRn.png";
        }
    }

    attachDOM() {
        // animation
        this.timeSlider = document.getElementById('timeRange');
        this.timeSliderDisplay = document.getElementById('timeRangeDisplay');
        // main canvas
        this.mainCanvas = document.getElementById('main-canvas');
        this.mainCanvas.style.width = this.mainCanvas.width + "px";
        this.mainCanvas.style.height = this.mainCanvas.height + "px";
        this.mainCanvas.width *= resolutionMultiplier;
        this.mainCanvas.height *= resolutionMultiplier;
        this.mainContext = this.mainCanvas.getContext('2d');
        this.mainContext.imageSmoothingEnabled = true;
        this.mainContext.imageSmoothingQuality = 'high';
        // bg canvas
        this.bgCanvas = document.getElementById('bg-canvas');
        this.bgCanvas.style.width = this.bgCanvas.width + "px";
        this.bgCanvas.style.height = this.bgCanvas.height + "px";
        this.bgCanvas.width *= resolutionMultiplier;
        this.bgCanvas.height *= resolutionMultiplier;
        this.bgContext = this.bgCanvas.getContext('2d');
        this.bgContext.imageSmoothingEnabled = true;
        this.bgContext.imageSmoothingQuality = 'high';
        // manipulation
        this.lastX = this.mainCanvas.width / 2;
        this.lastY = this.mainCanvas.height / 2;
        //
        this._trackTransforms(this.mainContext);
        this._trackTransforms(this.bgContext);
        this.mainContext.scale(resolutionMultiplier, resolutionMultiplier);
        this.bgContext.scale(resolutionMultiplier, resolutionMultiplier);
        this._initMouseEvents();
        this._initTouchEvents();
    }

    _initActors(actors) {
        this.playerData.clear();
        this.targetData.clear();
        this.trashMobData.clear();
        this.attachedActorData.clear();
        this.mechanicActorData = [];
        for (let i = 0; i < actors.length; i++) {
            const actor = actors[i];
            switch (actor.type) {
                case "Player":
                    this.playerData.set(actor.id, new PlayerIconDrawable(actor.img, 20, actor.group, actor.positions, actor.dead, actor.down, actor.dc));
                    if (this.times.length === 0) {
                        for (let j = 0; j < actor.positions.length / 2; j++) {
                            this.times.push(j * this.pollingRate);
                        }
                    }
                    break;
                case "Target":
                    this.targetData.set(actor.id, new EnemyIconDrawable(actor.start, actor.end, actor.img, 30, actor.positions));
                    break;
                case "Mob":
                    this.trashMobData.set(actor.id, new EnemyIconDrawable(actor.start, actor.end, actor.img, 25, actor.positions));
                    break;
                case "Circle":
                    this.mechanicActorData.push(new CircleMechanicDrawable(actor.start, actor.end, actor.fill, actor.growing, actor.color, this.inch * actor.radius, actor.connectedTo, this.inch * actor.minRadius));
                    break;
                case "Rectangle":
                    this.mechanicActorData.push(new RectangleMechanicDrawable(actor.start, actor.end, actor.fill, actor.growing, actor.color, this.inch * actor.width, this.inch * actor.height, actor.connectedTo));
                    break;
                case "RotatedRectangle":
                    this.mechanicActorData.push(new RotatedRectangleMechanicDrawable(actor.start, actor.end, actor.fill, actor.growing, actor.color, this.inch * actor.width, this.inch * actor.height, actor.rotation, this.inch * actor.radialTranslation, actor.spinAngle, actor.connectedTo));
                    break;
                case "Doughnut":
                    this.mechanicActorData.push(new DoughnutMechanicDrawable(actor.start, actor.end, actor.fill, actor.growing, actor.color, this.inch * actor.innerRadius, this.inch * actor.outerRadius, actor.connectedTo));
                    break;
                case "Pie":
                    this.mechanicActorData.push(new PieMechanicDrawable(actor.start, actor.end, actor.fill, actor.growing, actor.color, actor.direction, actor.openingAngle, this.inch * actor.radius, actor.connectedTo));
                    break;
                case "Line":
                    this.mechanicActorData.push(new LineMechanicDrawable(actor.start, actor.end, actor.fill, actor.growing, actor.color, actor.connectedFrom, actor.connectedTo));
                    break;
                case "Facing":
                    this.attachedActorData.set(actor.connectedTo, new FacingMechanicDrawable(actor.start, actor.end, actor.connectedTo, actor.facingData));
                    break;
                case "FacingRectangle":
                    this.attachedActorData.set(actor.connectedTo, new FacingRectangleMechanicDrawable(actor.start, actor.end, actor.connectedTo, actor.facingData, this.inch * actor.width, this.inch * actor.height, actor.color));
                    break;
                case "MovingPlatform":
                    this.backgroundActorData.push(new MovingPlatformDrawable(actor.start, actor.end, actor.image, this.inch * actor.width, this.inch * actor.height, actor.positions));
                    break;
            }
        }
    }

    updateTime(value) {
        this.reactiveDataStatus.time = parseInt(value);
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }

    updateTextInput() {
        this.timeSliderDisplay.value = (this.reactiveDataStatus.time / 1000.0).toFixed(3);
    }

    updateInputTime(value) {
        try {
            const cleanedString = value.replace(",", ".");
            const parsedTime = parseFloat(cleanedString);
            if (isNaN(parsedTime)) {
                return;
            }
            const ms = Math.round(parsedTime * 1000.0);
            this.reactiveDataStatus.time = Math.min(Math.max(ms, 0), this.times[this.times.length - 1]);
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
            if (this.reactiveDataStatus.time >= this.times[this.times.length - 1]) {
                this.reactiveDataStatus.time = 0;
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
        this.reactiveDataStatus.time = 0;
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }

    selectActor(pId) {
        let actor = this.playerData.get(pId);
        this.reactiveDataStatus.selectedPlayer = null;
        this.reactiveDataStatus.selectedPlayerID = null;
        let oldSelect = actor.selected;
        this.playerData.forEach(function (value, key, map) {
            value.selected = false;
        });
        actor.selected = !oldSelect;
        this.selectedGroup = actor.selected ? actor.group : -1;
        if (actor.selected) {
            this.reactiveDataStatus.selectedPlayer = actor;
            this.reactiveDataStatus.selectedPlayerID = pId;
        }
        if (this.animation === null) {
            animateCanvas(noUpdateTime);
        }
    }

    toggleHighlightSelectedGroup() {
        this.highlightSelectedGroup = !this.highlightSelectedGroup;
        animateCanvas(noUpdateTime);
    }

    toggleTrashMobs() {
        this.displayTrashMobs = !this.displayTrashMobs;
        animateCanvas(noUpdateTime);
    }

    toggleMechanics() {
        this.displayMechanics = !this.displayMechanics;
        animateCanvas(noUpdateTime);
    }

    toggleConeDisplay() {
        this.coneControl.enabled = !this.coneControl.enabled;
        animateCanvas(noUpdateTime);
    }

    setConeRadius(value) {
        this.coneControl.radius = value;
        animateCanvas(noUpdateTime);
    }

    setConeAngle(value) {
        this.coneControl.openingAngle = value;
        animateCanvas(noUpdateTime);
    }

    _initMouseEvents() {
        var _this = this;
        var canvas = this.mainCanvas;
        var ctx = this.mainContext;
        var bgCtx = this.bgContext;

        canvas.addEventListener('dblclick', function (evt) {
            _this.lastX = canvas.width / 2;
            _this.lastY = canvas.height / 2;
            _this.dragStart = null;
            _this.dragged = false;
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.scale(resolutionMultiplier, resolutionMultiplier);
            bgCtx.setTransform(1, 0, 0, 1, 0, 0);
            bgCtx.scale(resolutionMultiplier, resolutionMultiplier);
            _this.needBGUpdate = true;
            if (_this.animation === null) {
                animateCanvas(noUpdateTime);
            }
        }, false);

        canvas.addEventListener('mousedown', function (evt) {
            _this.lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
            _this.lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
            _this.dragStart = ctx.transformedPoint(_this.lastX, _this.lastY);
            _this.dragged = false;
        }, false);

        canvas.addEventListener('mousemove', function (evt) {
            _this.lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
            _this.lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
            _this.dragged = true;
            if (_this.dragStart) {
                var pt = ctx.transformedPoint(_this.lastX, _this.lastY);
                ctx.translate(pt.x - _this.dragStart.x, pt.y - _this.dragStart.y);
                bgCtx.translate(pt.x - _this.dragStart.x, pt.y - _this.dragStart.y);
                _this.needBGUpdate = true;
                if (_this.animation === null) {
                    animateCanvas(noUpdateTime);
                }
            }
        }, false);

        document.body.addEventListener('mouseup', function (evt) {
            _this.dragStart = null;
        }, false);

        var zoom = function (evt) {
            var delta = evt.wheelDelta ? evt.wheelDelta / 40 : evt.detail ? -evt.detail : 0;
            if (delta) {
                var pt = ctx.transformedPoint(_this.lastX, _this.lastY);
                ctx.translate(pt.x, pt.y);
                bgCtx.translate(pt.x, pt.y);
                var factor = Math.pow(1.1, delta);
                ctx.scale(factor, factor);
                ctx.translate(-pt.x, -pt.y);
                bgCtx.scale(factor, factor);
                bgCtx.translate(-pt.x, -pt.y);
                _this.needBGUpdate = true;
                if (_this.animation === null) {
                    animateCanvas(noUpdateTime);
                }
            }
            return evt.preventDefault() && false;
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

    toggleRange(index) {
        this.rangeControl[index].enabled = !this.rangeControl[index].enabled;
        animateCanvas(noUpdateTime);
    }

    setRangeRadius(index, value) {
        this.rangeControl[index].radius = value;
        animateCanvas(noUpdateTime);
    }

    // https://codepen.io/anon/pen/KrExzG
    _trackTransforms(ctx) {
        var svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
        var xform = svg.createSVGMatrix();
        ctx.getTransform = function () {
            return xform;
        };

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
            xform = xform.scaleNonUniform(sx, sy);
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
    _getBackgroundImage() {
        var time = this.reactiveDataStatus.time;
        for (var i = 0; i < this.backgroundImages.length; i++) {
            var imageData = this.backgroundImages[i];
            if (imageData.start < 0 || imageData.end < 0 || (imageData.start <= time && imageData.end >= time)) {
                return imageData.image;
            }
        }
        return null;
    }

    _drawBGCanvas() {
        var imgToDraw = this._getBackgroundImage();
        if ((imgToDraw !== null && imgToDraw !== this.prevBGImage) || this.needBGUpdate) {
            this.needBGUpdate = false;
            this.prevBGImage = imgToDraw;
            var ctx = this.bgContext;
            var canvas = this.bgCanvas;
            var p1 = ctx.transformedPoint(0, 0);
            var p2 = ctx.transformedPoint(canvas.width, canvas.height);
            ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

            ctx.save();
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.restore();


            ctx.drawImage(this._getBackgroundImage(), 0, 0, canvas.width / resolutionMultiplier, canvas.height / resolutionMultiplier);

            //ctx.globalCompositeOperation = "color-burn";
            ctx.save();
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
            ctx.fillText((50 / (this.inch * this.scale)).toFixed(1) + " units", resolutionMultiplier * 95, resolutionMultiplier * 60);
            ctx.restore();
            //ctx.globalCompositeOperation = 'normal';
        }
    }

    _drawMainCanvas() {
        var _this = this;
        var ctx = this.mainContext;
        var canvas = this.mainCanvas;
        var p1 = ctx.transformedPoint(0, 0);
        var p2 = ctx.transformedPoint(canvas.width, canvas.height);
        ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);
        ctx.save();
        ctx.setTransform(1, 0, 0, 1, 0, 0);
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.restore();
        // Background items commonly overlap so they need to be drawn in the correct order by height
        // This is sorted in reverse order because the z axis is inverted
        animator.backgroundActorData.sort((x, y) => y.getHeight() - x.getHeight());
        for (let i = 0; i < animator.backgroundActorData.length; i++) {
            animator.backgroundActorData[i].draw();
        }
        if (this.displayMechanics) {
            for (let i = 0; i < this.mechanicActorData.length; i++) {
                this.mechanicActorData[i].draw();
            }
        }
        
        this.playerData.forEach(function (value, key, map) {
            if (!value.selected) {
                value.draw();
                if (_this.attachedActorData.has(key)) {
                    _this.attachedActorData.get(key).draw();
                }
            }
        });
        
        if (this.displayTrashMobs) {
            this.trashMobData.forEach(function (value, key, map) {
                value.draw();
                if (_this.attachedActorData.has(key)) {
                    _this.attachedActorData.get(key).draw();
                }
            });
        }
        
        this.targetData.forEach(function (value, key, map) {
            value.draw();
            if (_this.attachedActorData.has(key)) {
                _this.attachedActorData.get(key).draw();
            }
        });
        if (this.reactiveDataStatus.selectedPlayer !== null) {
            this.reactiveDataStatus.selectedPlayer.draw();
            if (this.attachedActorData.has(this.reactiveDataStatus.selectedPlayerID)) {
                this.attachedActorData.get(this.reactiveDataStatus.selectedPlayerID).draw();
            }
        }
    }

    draw() {
        if (!this.mainCanvas) {
            return;
        }
        //
        this._drawBGCanvas();
        this._drawMainCanvas();
    }
}

function animateCanvas(noRequest) {
    if (animator == null) {
        return;
    }
    let lastTime = animator.times[animator.times.length - 1];
    if (noRequest > noUpdateTime && animator.animation !== null) {
        let curTime = new Date().getTime();
        let timeOffset = curTime - animator.prevTime;
        animator.prevTime = curTime;
        animator.reactiveDataStatus.time = Math.round(Math.max(Math.min(animator.reactiveDataStatus.time + animator.getSpeed() * timeOffset, lastTime), 0));
    }
    if ((animator.reactiveDataStatus.time === lastTime && !animator.backwards) || (animator.reactiveDataStatus.time === 0 && animator.backwards)) {
        animator.stopAnimate(true);
    }
    animator.timeSlider.value = animator.reactiveDataStatus.time.toString();
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
        dragStart = ctx.transformedPoint(lastX, lastY);
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
        if (dragStart) {
            var pt = ctx.transformedPoint(lastX, lastY);
            ctx.translate(pt.x - dragStart.x, pt.y - dragStart.y);
            animateCanvas(noUpdateTime);
        }
        return evt.preventDefault() && false;
    }, false);
    document.body.addEventListener('touchend', function (evt) {
        dragStart = null;
    }, false);
}
*/
