/*jshint esversion: 6 */
const deadIcon = new Image();
deadIcon.src = "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
const downIcon = new Image();
downIcon.src = "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
const bgImage = new Image();
bgImage.onload = function () {
    animateCanvas();
    bgLoaded = true;
};
let time = 0;
let inch = 10;
let speed = 1;
const times = [];
const bossData = new Map();
const playerData = new Map();
const trashMobData = new Map();
const mechanicActorData = new Set();
const rangeControl = new Map();
let selectedGroup = -1;
let selectedPlayer = null;
let bgLoaded = false;
let animation = null;
let prevTime = 0;
let pollingRate = 150;
let timeSlider = null;
let timeSliderDisplay = null;
let canvas = null;
let ctx = null;


function initCombatReplay(actors, options) {
	time = 0;
	if (options) {
		if (options.inch) inch = options.inch;
		if (options.pollingRate) pollingRate = options.pollingRate;
		if (options.mapLink) bgImage.src = options.mapLink;
	}
	speed = 1;
	timeSlider = document.getElementById('timeRange');
	timeSliderDisplay = document.getElementById('timeRangeDisplay');
	canvas = document.getElementById('replayCanvas');
	ctx = canvas.getContext('2d');
	bgLoaded = false;
	animation = null;
	prevTime = 0;

	// canvas
	ctx.imageSmoothingEnabled = true;
	ctx.imageSmoothingQuality = 'high';

	createAllActors(actors);
}

// Animation methods
function animateCanvas(noRequest) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(bgImage, 0, 0, canvas.width, canvas.height);
    mechanicActorData.forEach(function (value, key, map) {
        value.draw(ctx, time);
    });
    playerData.forEach(function (value, key, map) {
        if (!value.selected) {
            value.draw(ctx, time);
        }
    });
    trashMobData.forEach(function (value, key, map) {
        value.draw(ctx, time);
    });
    bossData.forEach(function (value, key, map) {
        value.draw(ctx, time);
    });
    if (selectedPlayer !== null) {
        selectedPlayer.draw(ctx, time);
    }
    let lastTime = times[times.length - 1];
    if (time === lastTime) {
        stopAnimate();
    }
    timeSlider.value = time.toString();
    if (noRequest !== -2) {
        updateTextInput(time);
    }
    if (noRequest > -1 && animation !== null && bgLoaded) {
        let curTime = new Date().getTime();
        let timeOffset = curTime - prevTime;
        prevTime = curTime;
        time = Math.round(Math.min(time + speed * timeOffset, lastTime));
        animation = requestAnimationFrame(animateCanvas);
    }
}

function startAnimate() {
    if (animation === null && times.length > 0) {
        if (time >= times[times.length - 1]) {
            time = 0;
        }
        prevTime = new Date().getTime();
        animation = requestAnimationFrame(animateCanvas);
    }
}

function stopAnimate() {
    if (animation !== null) {
        window.cancelAnimationFrame(animation);
        animation = null;
    }
}

function restartAnimate() {
    time = 0;
    if (animation === null) {
        animateCanvas(-1);
    }
}

function eighthSpeed() {
    speed = 0.125;
}

function fourthSpeed() {
    speed = 0.25;
}

function halfSpeed() {
    speed = 0.5;
}

function normalSpeed() {
    speed = 1;
}

function twoSpeed() {
    speed = 2;
}

function fourSpeed() {
    speed = 4;
}

function eightSpeed() {
    speed = 8;
}

function sixteenSpeed() {
    speed = 16;
}

// range markers
rangeControl.set(180, false);
rangeControl.set(240, false);
rangeControl.set(300, false);
rangeControl.set(600, false);
rangeControl.set(900, false);
rangeControl.set(1200, false);

function toggleRange(radius) {
    rangeControl.set(radius, !rangeControl.get(radius));
    animateCanvas(-1);
}

// slider
function updateTime(value) {
    time = parseInt(value);
    updateTextInput(time);
    animateCanvas(-1);
}

function updateTextInput(val) {
    timeSliderDisplay.value = (val / 1000.0).toFixed(3);
}

function updateInputTime(value) {
    try {
        const cleanedString = value.replace(",", ".");
        const parsedTime = parseFloat(cleanedString);
        if (isNaN(parsedTime)) {
            return;
        }
        const ms = Math.round(parsedTime * 1000.0);
        time = Math.min(Math.max(ms, 0), times[times.length - 1]);
        animateCanvas(-2);
    } catch (error) {
        console.error(error);
    }
}

// selection
function selectActor(pId) {
    let actor = playerData.get(pId);
    selectedPlayer = null;
    let oldSelect = actor.selected;
    playerData.forEach(function (value, key, map) {
        value.selected = false;
    });
    actor.selected = !oldSelect;
    selectedGroup = actor.selected ? actor.group : -1;
    if (actor.selected) {
        selectedPlayer = actor;
    }
    playerData.forEach(function (value, key, map) {
        let hasActive = document.getElementById('id' + key).classList.contains('active') && !value.selected;
        if (hasActive) {
            setTimeout(function () {
                document.getElementById('id' + key).classList.remove('active');
            }, 50);
        }
    });
    animateCanvas(-1);
}

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

    getInterpolatedPosition(startIndex, currentIndex, currentTime) {
        const offsetedIndex = currentIndex - startIndex;
        const positionX = this.pos[2 * offsetedIndex];
        const positionY = this.pos[2 * offsetedIndex + 1];
        const timeValue = times[currentIndex];
        if (currentTime - timeValue > 0 && offsetedIndex < 0.5 * this.pos.length - 1) {
            const nextTimeValue = times[currentIndex + 1];
            const nextPositionX = this.pos[2 * offsetedIndex + 2];
            const nextPositionY = this.pos[2 * offsetedIndex + 3];
            return {
                x: Math.round(positionX + (currentTime - timeValue) / (nextTimeValue - timeValue) * (nextPositionX - positionX)),
                y: Math.round(positionY + (currentTime - timeValue) / (nextTimeValue - timeValue) * (nextPositionY - positionY))
            };
        } else {
            return {
                x: positionX,
                y: positionY
            };
        }
    }

    getPosition(currentTime) {
        if (this.pos === null || this.pos.length === 0) {
            return null;
        }
        if (this.start !== -1 && (this.start >= currentTime || this.end <= currentTime)) {
            return null;
        }
        if (this.pos.length === 2) {
            return {
                x: this.pos[0],
                y: this.pos[1]
            };
        }
        const lastTime = times[times.length - 1];
        const startIndex = Math.ceil((times.length - 1) * Math.max(this.start, 0) / lastTime);
        const currentIndex = Math.floor((times.length - 1) * currentTime / lastTime);
        return this.getInterpolatedPosition(startIndex, Math.max(currentIndex, startIndex), currentTime);
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const halfSize = this.pixelSize / 2;
        ctx.drawImage(this.img,
            pos.x - halfSize, pos.y - halfSize, this.pixelSize, this.pixelSize);
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

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const halfSize = this.pixelSize / 2;
        if (!this.selected && this.group === selectedGroup) {
            ctx.beginPath();
            ctx.lineWidth = '2';
            ctx.strokeStyle = 'blue';
            ctx.rect(pos.x - halfSize, pos.y - halfSize, this.pixelSize, this.pixelSize);
            ctx.stroke();
        } else if (this.selected) {
            ctx.beginPath();
            ctx.lineWidth = '4';
            ctx.strokeStyle = 'green';
            ctx.rect(pos.x - halfSize, pos.y - halfSize, this.pixelSize, this.pixelSize);
            ctx.stroke();
            rangeControl.forEach(function (enabled, radius, map) {
                if (!enabled) return;
                ctx.beginPath();
                ctx.lineWidth = '2';
                ctx.strokeStyle = 'green';
                ctx.arc(pos.x, pos.y, inch * radius, 0, 2 * Math.PI);
                ctx.stroke();
            });
        }
        ctx.drawImage(this.getIcon(currentTime),
            pos.x - halfSize, pos.y - halfSize, this.pixelSize, this.pixelSize);
    }

    died(currentTime) {
        if (this.dead === null || this.dead.length === 0) {
            return false;
        }
        for (let i = 0; i < this.dead.length; i += 2) {
            if (this.dead[i] <= currentTime && this.dead[i + 1] >= currentTime) {
                return true;
            }
        }
        return false;
    }

    downed(currentTime) {
        if (this.down === null || this.down.length === 0) {
            return false;
        }
        for (let i = 0; i < this.down.length; i += 2) {
            if (this.down[i] <= currentTime && this.down[i + 1] >= currentTime) {
                return true;
            }
        }
        return false;
    }

    getIcon(currentTime) {
        if (this.died(currentTime)) {
            return deadIcon;
        }
        if (this.downed(currentTime)) {
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

    getPosition(currentTime) {
        if (this.connectedTo === null) {
            return null;
        }
        if (this.start !== -1 && (this.start >= currentTime || this.end <= currentTime)) {
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
                this.master = playerData.has(masterId) ? playerData.get(masterId) : trashMobData.has(masterId) ? trashMobData.get(masterId) : bossData.get(masterId);
            }
            return this.master.getPosition(currentTime);
        }
    }

    getPercent(currentTime) {
        if (this.growing === 0) {
            return 1.0;
        }
        return Math.min((currentTime - this.start) / (this.growing - this.start), 1.0);
    }
}

class CircleMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, radius, connectedTo, minRadius) {
        super(start, end, fill, growing, color, connectedTo);
        this.radius = inch * radius;
        this.minRadius = inch * minRadius;
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, this.getPercent(currentTime) * (this.radius - this.minRadius) + this.minRadius, 0, 2 * Math.PI);
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = '2';
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class DoughnutMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, innerRadius, outerRadius, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.outerRadius = inch * outerRadius;
        this.innerRadius = inch * innerRadius;
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, this.innerRadius + percent * (this.outerRadius - this.innerRadius), 2 * Math.PI, 0, false);
        ctx.arc(pos.x, pos.y, this.innerRadius, 0, 2 * Math.PI, true);
        ctx.closePath();
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = '2';
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class RectangleMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, width, height, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.height = height * inch;
        this.width = width * inch;
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.rect(pos.x - 0.5 * percent * this.width, pos.y - 0.5 * percent * this.height, percent * this.width, percent * this.height);
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = '2';
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class RotatedRectangleMechanicDrawable extends RectangleMechanicDrawable {
    constructor(start, end, fill, growing, color, width, height, rotation, translation, spinangle, connectedTo) {
        super(start, end, fill, growing, color, width, height, connectedTo);
        this.rotation = -rotation * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
        this.translation = translation * inch;
        this.spinangle = -spinangle * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
    }

    getSpinPercent(currentTime) {
        if (this.spinangle === 0) {
            return 1.0;
        }
        return Math.min((currentTime - this.start) / (this.end - this.start), 1.0);
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const percent = this.getPercent(currentTime);
        const spinPercent = this.getSpinPercent(currentTime);
        const offset = {
            x: pos.x,// - 0.5 * percent * this.width,
            y: pos.y// - 0.5 * percent * this.height
        };
        const angle = this.rotation + spinPercent * this.spinangle;
        ctx.save();
        ctx.translate(offset.x, offset.y);
        ctx.rotate(angle % 360);
        ctx.beginPath();
        ctx.rect((- 0.5 * this.width + this.translation) * percent, - 0.5 * percent * this.height, percent * this.width, percent * this.height);
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = '2';
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
        ctx.restore();
    }
}

class PieMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, direction, openingAngle, radius, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.direction = -direction * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
        this.openingAngle = 0.5 * openingAngle * Math.PI / 180;
        this.radius = inch * radius;
        this.dx = Math.cos(this.direction - this.openingAngle) * this.radius;
        this.dy = Math.sin(this.direction - this.openingAngle) * this.radius;
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
        ctx.lineTo(pos.x + this.dx * percent, pos.y + this.dy * percent);
        ctx.arc(pos.x, pos.y, percent * this.radius, this.direction - this.openingAngle, this.direction + this.openingAngle);
        ctx.closePath();
        if (this.fill) {
            ctx.fillStyle = this.color;
            ctx.fill();
        } else {
            ctx.lineWidth = '2';
            ctx.strokeStyle = this.color;
            ctx.stroke();
        }
    }
}

class LineMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, width, connectedFrom, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.connectedFrom = connectedFrom;
        this.width = width*inch;
        this.endmaster = null;
    }

    getTargetPosition(currentTime) {
        if (this.connectedFrom === null) {
            return null;
        }
        if (this.start !== -1 && (this.start >= currentTime || this.end <= currentTime)) {
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
                this.endmaster = playerData.has(endMasterID) ? playerData.get(endMasterID) : trashMobData.has(endMasterID) ? trashMobData.get(endMasterID) : bossData.get(endMasterID);
            }
            return this.endmaster.getPosition(currentTime);
        }
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        const target = this.getTargetPosition(currentTime);

        if (pos === null || target === null) {
            return;
        }
        const percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
        ctx.lineTo(pos.x + percent * (target.x - pos.x), pos.y + percent * (target.y - pos.y));
        ctx.lineWidth = this.width;
        ctx.strokeStyle = this.color;
        ctx.stroke();
    }
}

function createAllActors(actors) {
    for (let i = 0; i < actors.length; i++) {
        const actor = actors[i];
        switch (actor.Type) {
            case "Player":
                playerData.set(actor.ID, new PlayerIconDrawable(actor.Img, 20, actor.Group, actor.Positions, actor.Dead, actor.Down));
                if (times.length === 0) {
                    for (let i = 0; i < actor.Positions.length / 2; i++) {
                        times.push(i * pollingRate);
                    }
                }
                break;
            case "Boss":
                bossData.set(actor.ID, new EnemyIconDrawable(actor.Start, actor.End, actor.Img, 30, actor.Positions));
                break;
            case "Mob":
                trashMobData.set(actor.ID, new EnemyIconDrawable(actor.Start, actor.End, actor.Img, 30, actor.Positions));
                break;
            case "Circle":
                mechanicActorData.add(new CircleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Radius, actor.ConnectedTo, actor.MinRadius));
                break;
            case "Rectangle":
                mechanicActorData.add(new RectangleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Width, actor.Height, actor.ConnectedTo));
                break;
            case "RotatedRectangle":
                mechanicActorData.add(new RotatedRectangleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Width, actor.Height, actor.Rotation, actor.RadialTranslation, actor.SpinAngle, actor.ConnectedTo));
                break;
            case "Doughnut":
                mechanicActorData.add(new DoughnutMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.InnerRadius, actor.OuterRadius, actor.ConnectedTo));
                break;
            case "Pie":
                mechanicActorData.add(new PieMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Direction, actor.OpeningAngle, actor.Radius, actor.ConnectedTo));
                break;
            case "Line":
                mechanicActorData.add(new LineMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Width, actor.ConnectedFrom, actor.ConnectedTo));
                break;
        }
    }
}
