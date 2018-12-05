/*jshint esversion: 6 */
// const images
const deadIcon = new Image();
deadIcon.src = "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
const downIcon = new Image();
downIcon.src = "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
const bgImage = new Image();
let bgLoaded = false;
bgImage.onload = function () {
    animateCanvas(-1);
    bgLoaded = true;
};
const resolutionMultiplier = 2;

var animator = null;

class Animator {
    constructor(options, actors) {
        // time
        this.prevTime = 0;
        this.time = 0;
        this.times = [];
        // simulation params
        this.inch = 10;
        this.pollingRate = 150;
        this.speed = 1;
        this.rangeControl = new Map();
        this.selectedGroup = -1;
        this.selectedPlayer = null;
        this.selectedPlayerID = null;
        // actors
        this.targetData = new Map();
        this.playerData = new Map();
        this.trashMobData = new Map();
        this.mechanicActorData = new Set();
        // animation
        this.animation = null;
        this.timeSlider = document.getElementById('timeRange');
        this.timeSliderDisplay = document.getElementById('timeRangeDisplay');
        this.canvas = document.getElementById('replayCanvas');
        this.canvas.style.width = this.canvas.width +"px";
        this.canvas.style.height = this.canvas.height +"px";
        this.canvas.width *= resolutionMultiplier;
        this.canvas.height *= resolutionMultiplier;
        this.ctx = this.canvas.getContext('2d');
        this.ctx.imageSmoothingEnabled = true;
        this.ctx.imageSmoothingQuality = "high";
        // manipulation
        this.lastX = this.canvas.width / 2;
        this.lastY = this.canvas.height / 2;
        this.dragStart = null;
        this.dragged = false;
        this.scale = 1.0;
        if (options) {
            if (options.inch) this.inch = options.inch;
            if (options.pollingRate) this.pollingRate = options.pollingRate;
            if (options.mapLink) bgImage.src = options.mapLink;
        }
        //
        this.rangeControl.set(this.inch * 180, false);
        this.rangeControl.set(this.inch * 240, false);
        this.rangeControl.set(this.inch * 300, false);
        this.rangeControl.set(this.inch * 600, false);
        this.rangeControl.set(this.inch * 900, false);
        this.rangeControl.set(this.inch * 1200, false);
        this.trackTransforms();
        this.ctx.scale(resolutionMultiplier, resolutionMultiplier);
        this.initActors(actors);
        this.initMouseEvents();
        this.initTouchEvents();
        if (typeof mainComponent !== "undefined" && mainComponent !== null) {
            mainComponent.animator = this;
        }
    }

    initActors(actors) {
        for (let i = 0; i < actors.length; i++) {
            const actor = actors[i];
            switch (actor.Type) {
                case "Player":
                    this.playerData.set(actor.ID, new PlayerIconDrawable(actor.Img, 25, actor.Group, actor.Positions, actor.Dead, actor.Down));
                    if (this.times.length === 0) {
                        for (let j = 0; j < actor.Positions.length / 2; j++) {
                            this.times.push(j * this.pollingRate);
                        }
                    }
                    break;
                case "Target":
                    this.targetData.set(actor.ID, new EnemyIconDrawable(actor.Start, actor.End, actor.Img, 35, actor.Positions));
                    break;
                case "Mob":
                    this.trashMobData.set(actor.ID, new EnemyIconDrawable(actor.Start, actor.End, actor.Img, 25, actor.Positions));
                    break;
                case "Circle":
                    this.mechanicActorData.add(new CircleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Radius, actor.ConnectedTo, actor.MinRadius, this.inch));
                    break;
                case "Rectangle":
                    this.mechanicActorData.add(new RectangleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Width, actor.Height, actor.ConnectedTo, this.inch));
                    break;
                case "RotatedRectangle":
                    this.mechanicActorData.add(new RotatedRectangleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Width, actor.Height, actor.Rotation, actor.RadialTranslation, actor.SpinAngle, actor.ConnectedTo, this.inch));
                    break;
                case "Doughnut":
                    this.mechanicActorData.add(new DoughnutMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.InnerRadius, actor.OuterRadius, actor.ConnectedTo, this.inch));
                    break;
                case "Pie":
                    this.mechanicActorData.add(new PieMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Direction, actor.OpeningAngle, actor.Radius, actor.ConnectedTo, this.inch));
                    break;
                case "Line":
                    this.mechanicActorData.add(new LineMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.ConnectedFrom, actor.ConnectedTo));
                    break;
            }
        }
    }

    updateTime(value) {
        this.time = parseInt(value);
        this.updateTextInput();
        animateCanvas(-1);
    }

    updateTextInput(val) {
        this.timeSliderDisplay.value = (val / 1000.0).toFixed(3);
    }

    updateInputTime(value) {
        try {
            const cleanedString = value.replace(",", ".");
            const parsedTime = parseFloat(cleanedString);
            if (isNaN(parsedTime)) {
                return;
            }
            const ms = Math.round(parsedTime * 1000.0);
            this.time = Math.min(Math.max(ms, 0), this.times[this.times.length - 1]);
            animateCanvas(-2);
        } catch (error) {
            console.error(error);
        }
    }

    startAnimate() {
        if (this.animation === null && this.times.length > 0) {
            if (this.time >= this.times[this.times.length - 1]) {
                this.time = 0;
            }
            this.prevTime = new Date().getTime();
            this.animation = requestAnimationFrame(animateCanvas);
        }
    }

    stopAnimate() {
        if (this.animation !== null) {
            window.cancelAnimationFrame(this.animation);
            this.animation = null;
        }
    }

    restartAnimate() {
        this.time = 0;
        if (this.animation === null) {
            animateCanvas(-1);
        }
    }

    selectActor(pId) {
        let actor = this.playerData.get(pId);
        this.selectedPlayer = null;
        let oldSelect = actor.selected;
        this.playerData.forEach(function (value, key, map) {
            value.selected = false;
        });
        actor.selected = !oldSelect;
        this.selectedGroup = actor.selected ? actor.group : -1;
        if (actor.selected) {
            this.selectedPlayer = actor;
            this.selectedPlayerID = pId;
        }
        this.playerData.forEach(function (value, key, map) {
            let hasActive = document.getElementById('id' + key).classList.contains('active') && !value.selected;
            if (hasActive) {
                setTimeout(function () {
                    document.getElementById('id' + key).classList.remove('active');
                }, 50);
            }
        });
        animateCanvas(-1);
    }

    initMouseEvents() {
        var _this = this;
        var canvas = this.canvas;
        var ctx = this.ctx;

        canvas.addEventListener('dblclick', function (evt) {
            _this.lastX = canvas.width / 2;
            _this.lastY = canvas.height / 2;
            _this.dragStart = null;
            _this.dragged = false;
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.scale(resolutionMultiplier, resolutionMultiplier);
            animateCanvas(-1);
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
                animateCanvas(-1);
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
                var factor = Math.pow(1.1, delta);
                ctx.scale(factor, factor);
                ctx.translate(-pt.x, -pt.y);
                animateCanvas(-1);
            }
            return evt.preventDefault() && false;
        };

        canvas.addEventListener('DOMMouseScroll', zoom, false);
        canvas.addEventListener('mousewheel', zoom, false);
    }

    initTouchEvents() {
        // todo
    }

    setSpeed(value) {
        this.speed = value;
    }

    toggleRange(radius) {
        this.rangeControl.set(this.inch * radius, !this.rangeControl.get(this.inch * radius));
        animateCanvas(-1);
    }

    // https://codepen.io/anon/pen/KrExzG
    trackTransforms() {
        var ctx = this.ctx;
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
}

function animateCanvas(noRequest) {
    var ctx = animator.ctx;
    var canvas = animator.canvas;
    var p1 = ctx.transformedPoint(0, 0);
    var p2 = ctx.transformedPoint(canvas.width, canvas.height);
    ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

    ctx.save();
    ctx.setTransform(1, 0, 0, 1, 0, 0);
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.restore();
    //
    ctx.drawImage(bgImage, 0, 0, canvas.width / resolutionMultiplier, canvas.height / resolutionMultiplier);
    animator.mechanicActorData.forEach(function (value, key, map) {
        value.draw();
    });
    animator.playerData.forEach(function (value, key, map) {
        if (!value.selected) {
            value.draw();
        }
    });
    animator.trashMobData.forEach(function (value, key, map) {
        value.draw();
    });
    animator.targetData.forEach(function (value, key, map) {
        value.draw();
    });
    if (animator.selectedPlayer !== null) {
        animator.selectedPlayer.draw();
    }
    let lastTime = animator.times[animator.times.length - 1];
    if (animator.time === lastTime) {
        animator.stopAnimate();
    }
    animator.timeSlider.value = animator.time.toString();
    if (noRequest !== -2) {
        animator.updateTextInput(animator.time);
    }
    if (noRequest > -1 && animator.animation !== null && bgLoaded) {
        let curTime = new Date().getTime();
        let timeOffset = curTime - animator.prevTime;
        animator.prevTime = curTime;
        animator.time = Math.round(Math.min(animator.time + animator.speed * timeOffset, lastTime));
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
            animateCanvas(-1);
        }
        return evt.preventDefault() && false;
    }, false);
    document.body.addEventListener('touchend', function (evt) {
        dragStart = null;
    }, false);
}
*/

// Drawables

class IconDrawable {
    constructor(start, end, imgSrc, pixelSize) {
        this.pos = null;
        this.start = start;
        this.end = end;
        this.img = new Image();
        this.img.src = imgSrc;
        this.pixelSize = pixelSize;
    }

    getInterpolatedPosition(startIndex, currentIndex) {
        const offsetedIndex = currentIndex - startIndex;
        const positionX = this.pos[2 * offsetedIndex];
        const positionY = this.pos[2 * offsetedIndex + 1];
        const timeValue = animator.times[currentIndex];
        var pt = {
            x: 0,
            y: 0
        };
        if (animator.time - timeValue > 0 && offsetedIndex < 0.5 * this.pos.length - 1) {
            const nextTimeValue = animator.times[currentIndex + 1];
            const nextPositionX = this.pos[2 * offsetedIndex + 2];
            const nextPositionY = this.pos[2 * offsetedIndex + 3];
            pt.x = positionX + (animator.time - timeValue) / (nextTimeValue - timeValue) * (nextPositionX - positionX);
            pt.y = positionY + (animator.time - timeValue) / (nextTimeValue - timeValue) * (nextPositionY - positionY);
        } else {
            pt.x = positionX;
            pt.y = positionY;
        }
        pt.x = Math.round(10*pt.x * animator.scale) / (10*animator.scale);
        pt.y = Math.round(10*pt.y * animator.scale) / (10*animator.scale);
        return pt;
    }

    getPosition() {
        if (this.pos === null || this.pos.length === 0) {
            return null;
        }
        if (this.start !== -1 && (this.start >= animator.time || this.end <= animator.time)) {
            return null;
        }
        if (this.pos.length === 2) {
            return {
                x: this.pos[0],
                y: this.pos[1]
            };
        }
        const lastTime = animator.times[animator.times.length - 1];
        const startIndex = Math.ceil((animator.times.length - 1) * Math.max(this.start, 0) / lastTime);
        const currentIndex = Math.floor((animator.times.length - 1) * animator.time / lastTime);
        return this.getInterpolatedPosition(startIndex, Math.max(currentIndex, startIndex), animator.time);
    }

    draw() {
        const pos = this.getPosition(animator.time, animator.times);
        if (pos === null) {
            return;
        }
        const fullSize = this.pixelSize / animator.scale;
        const halfSize = fullSize / 2;
        animator.ctx.drawImage(this.img,
            pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
    }

}

class PlayerIconDrawable extends IconDrawable {
    constructor(imgSrc, pixelSize, group, pos, dead, down) {
        super(-1, -1, imgSrc, pixelSize);
        this.pos = pos;
        this.dead = dead;
        this.down = down;
        this.selected = false;
        this.group = group;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.ctx;
        const fullSize = this.pixelSize / animator.scale;
        const halfSize = fullSize / 2;
        if (!this.selected && this.group === animator.selectedGroup) {
            ctx.beginPath();
            ctx.lineWidth = (2/animator.scale).toString();
            ctx.strokeStyle = 'blue';
            ctx.rect(pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
            ctx.stroke();
        } else if (this.selected) {
            ctx.beginPath();
            ctx.lineWidth = (4 / animator.scale).toString();
            ctx.strokeStyle = 'green';
            ctx.rect(pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
            ctx.stroke();
            animator.rangeControl.forEach(function (enabled, radius, map) {
                if (!enabled) return;
                ctx.beginPath();
                ctx.lineWidth = (2 / animator.scale).toString();
                ctx.strokeStyle = 'green';
                ctx.arc(pos.x, pos.y, radius, 0, 2 * Math.PI);
                ctx.stroke();
            });
        }
        ctx.drawImage(this.getIcon(),
            pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
    }

    died() {
        if (this.dead === null || this.dead.length === 0) {
            return false;
        }
        for (let i = 0; i < this.dead.length; i += 2) {
            if (this.dead[i] <= animator.time && this.dead[i + 1] >= animator.time) {
                return true;
            }
        }
        return false;
    }

    downed() {
        if (this.down === null || this.down.length === 0) {
            return false;
        }
        for (let i = 0; i < this.down.length; i += 2) {
            if (this.down[i] <= animator.time && this.down[i + 1] >= animator.time) {
                return true;
            }
        }
        return false;
    }

    getIcon() {
        if (this.died()) {
            return deadIcon;
        }
        if (this.downed()) {
            return downIcon;
        }
        return this.img;
    }

}

class EnemyIconDrawable extends IconDrawable {
    constructor(start, end, imgSrc, pixelSize, pos) {
        super(start, end, imgSrc, pixelSize);
        this.pos = pos;
    }
}

class MechanicDrawable {
    constructor(start, end, fill, growing, color, connectedTo) {
        this.start = start;
        this.end = end;
        this.connectedTo = connectedTo;
        this.fill = fill;
        this.growing = growing;
        this.color = color;
        this.master = null;
    }

    getPosition() {
        if (this.connectedTo === null) {
            return null;
        }
        if (this.start !== -1 && (this.start >= animator.time || this.end <= animator.time)) {
            return null;
        }
        if (this.connectedTo instanceof Array) {
            return {
                x: this.connectedTo[0],
                y: this.connectedTo[1]
            };
        } else {
            if (this.master === null) {
                let masterId = this.connectedTo;
                this.master = animator.playerData.has(masterId) ? animator.playerData.get(masterId) : animator.trashMobData.has(masterId) ? animator.trashMobData.get(masterId) : animator.targetData.get(masterId);
            }
            return this.master.getPosition();
        }
    }

    getPercent() {
        if (this.growing === 0) {
            return 1.0;
        }
        return Math.min((animator.time - this.start) / (this.growing - this.start), 1.0);
    }
}

class CircleMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, radius, connectedTo, minRadius, inch) {
        super(start, end, fill, growing, color, connectedTo);
        this.radius = inch * radius;
        this.minRadius = inch * minRadius;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.ctx;
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, this.getPercent() * (this.radius - this.minRadius) + this.minRadius, 0, 2 * Math.PI);
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class DoughnutMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, innerRadius, outerRadius, connectedTo, inch) {
        super(start, end, fill, growing, color, connectedTo);
        this.outerRadius = inch * outerRadius;
        this.innerRadius = inch * innerRadius;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.ctx;
        const percent = this.getPercent();
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, this.innerRadius + percent * (this.outerRadius - this.innerRadius), 2 * Math.PI, 0, false);
        ctx.arc(pos.x, pos.y, this.innerRadius, 0, 2 * Math.PI, true);
        ctx.closePath();
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class RectangleMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, width, height, connectedTo, inch) {
        super(start, end, fill, growing, color, connectedTo);
        this.height = height * inch;
        this.width = width * inch;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.ctx;
        const percent = this.getPercent();
        ctx.beginPath();
        ctx.rect(pos.x - 0.5 * percent * this.width, pos.y - 0.5 * percent * this.height, percent * this.width, percent * this.height);
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class RotatedRectangleMechanicDrawable extends RectangleMechanicDrawable {
    constructor(start, end, fill, growing, color, width, height, rotation, translation, spinangle, connectedTo, inch) {
        super(start, end, fill, growing, color, width, height, connectedTo);
        this.rotation = -rotation * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
        this.translation = translation * inch;
        this.spinangle = -spinangle * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
    }

    getSpinPercent() {
        if (this.spinangle === 0) {
            return 1.0;
        }
        return Math.min((animator.time - this.start) / (this.end - this.start), 1.0);
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.ctx;
        const percent = this.getPercent();
        const spinPercent = this.getSpinPercent();
        const offset = {
            x: pos.x, // - 0.5 * percent * this.width,
            y: pos.y // - 0.5 * percent * this.height
        };
        const angle = this.rotation + spinPercent * this.spinangle;
        ctx.save();
        ctx.translate(offset.x, offset.y);
        ctx.rotate(angle % 360);
        ctx.beginPath();
        ctx.rect((-0.5 * this.width + this.translation) * percent, -0.5 * percent * this.height, percent * this.width, percent * this.height);
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
        ctx.restore();
    }
}

class PieMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, direction, openingAngle, radius, connectedTo, inch) {
        super(start, end, fill, growing, color, connectedTo);
        this.direction = -direction * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
        this.openingAngle = 0.5 * openingAngle * Math.PI / 180;
        this.radius = inch * radius;
        this.dx = Math.cos(this.direction - this.openingAngle) * this.radius;
        this.dy = Math.sin(this.direction - this.openingAngle) * this.radius;
    }

    draw() {
        const pos = this.getPosition(animator.time);
        if (pos === null) {
            return;
        }
        var ctx = animator.ctx;
        const percent = this.getPercent(animator.time);
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
        ctx.lineTo(pos.x + this.dx * percent, pos.y + this.dy * percent);
        ctx.arc(pos.x, pos.y, percent * this.radius, this.direction - this.openingAngle, this.direction + this.openingAngle);
        ctx.closePath();
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class LineMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, connectedFrom, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.connectedFrom = connectedFrom;
        this.endmaster = null;
    }

    getTargetPosition() {
        if (this.connectedFrom === null) {
            return null;
        }
        if (this.start !== -1 && (this.start >= animator.time || this.end <= animator.time)) {
            return null;
        }
        if (this.connectedFrom instanceof Array) {
            return {
                x: this.target[0],
                y: this.target[1]
            };
        } else {
            if (this.endmaster === null) {
                let endMasterID = this.connectedFrom;
                this.endmaster = animator.playerData.has(endMasterID) ? animator.playerData.get(endMasterID) : animator.trashMobData.has(endMasterID) ? animator.trashMobData.get(endMasterID) : animator.targetData.get(endMasterID);
            }
            return this.endmaster.getPosition();
        }
    }

    draw() {
        const pos = this.getPosition();
        const target = this.getTargetPosition();

        if (pos === null || target === null) {
            return;
        }
        var ctx = animator.ctx;
        const percent = this.getPercent();
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
        ctx.lineTo(pos.x + percent * (target.x - pos.x), pos.y + percent * (target.y - pos.y));
        ctx.lineWidth = (2 / animator.scale).toString();
        ctx.strokeStyle = this.color;
        ctx.stroke();
    }
}