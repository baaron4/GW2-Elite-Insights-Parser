const deadIcon = new Image();
deadIcon.src = "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
const downIcon = new Image();
downIcon.src = "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
let time = 0;
let inch = 0;
let speed = 1;
const times = [];
let boss = null;
const playerData = new Map();
const trashMobData = new Map();
const mechanicActorData = new Set();
const rangeControl = new Map();
let selectedGroup = -1;
let selectedPlayer = null;
const timeSlider = document.getElementById('timeRange');
const timeSliderDisplay = document.getElementById('timeRangeDisplay');
const canvas = document.getElementById('replayCanvas');
const ctx = canvas.getContext('2d');
const bgImage = new Image();
let bgLoaded = false;
let animation = null;
// 60 fps by default
const timeOffset = 16;
let pollingRate = 100;

// canvas
ctx.imageSmoothingEnabled = true;
ctx.imageSmoothingQuality = 'high';

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
    boss.draw(ctx, time);
    if (selectedPlayer !== null) {
        selectedPlayer.draw(ctx, time);
    }
    let lastTime = times[times.length - 1];
    if (time === lastTime) {
        stopAnimate();
    }
    timeSlider.value = time.toString();
    updateTextInput(time);
    if (noRequest > -1 && animation !== null && bgLoaded) {
        time = Math.min(time + speed * timeOffset, lastTime);
        animation = requestAnimationFrame(animateCanvas);
    }
}
bgImage.onload = function () {
    animateCanvas();
    bgLoaded = true;
};
function startAnimate() {
    if (animation === null && times.length > 0) {
        if (time >= times[times.length - 1]) {
            time = 0;
        }
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
}

// slider
function updateTime(value) {
    time = parseInt(value);
    updateTextInput(time);
    animateCanvas(-1);
}
function updateTextInput(val) {
    timeSliderDisplay.value = val / 1000.0 + ' secs';
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
    if (!actor.selected) {
        let hasActive = document.getElementById('id' + pId).classList.contains('active');
        if (hasActive) {
            setTimeout(function () {
                document.getElementById('id' + pId).classList.remove('active');
            }, 50);
        }
    } else {
        selectedPlayer = actor;
    }
}

// Drawables
class Drawable {
    constructor(start, end) {
        this.pos = null;
        this.start = start;
        this.end = end;
    }

    getInterpolatedPosition(startIndex, currentIndex, currentTime) {
        const offsetedIndex = currentIndex - startIndex;
        const positionX = this.pos[2 * offsetedIndex];
        const positionY = this.pos[2 * offsetedIndex + 1];
        const timeValue = times[currentIndex];
        if (currentTime - timeValue > 0 && offsetedIndex < 0.5*this.pos.length - 1) {
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
}

class IconDrawable extends Drawable {
    constructor(start, end, imgSrc, pixelSize) {
        super(start, end);
        this.img = new Image();
        this.img.src = imgSrc;
        this.pixelSize = pixelSize;
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
    constructor(imgSrc, pixelSize, group, pos, dead,down) {
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

class BossIconDrawable extends IconDrawable {
    constructor(imgSrc, pixelSize, pos) {
        super(-1, -1, imgSrc, pixelSize);
        this.pos = pos;
    }
}

class MobIconDrawable extends IconDrawable {
    constructor(start, end, imgSrc, pixelSize, pos) {
        super(start, end, imgSrc, pixelSize);
        this.pos = pos;
    }
}

class MechanicDrawable extends Drawable {
    constructor(start, end, fill, growing, color) {
        super(start, end);
        this.fill = fill;
        this.growing = growing;
        this.color = color;
        this.master = null;
    }

    getPosition(currentTime) {
        if (this.pos === null) {
            return null;
        }
        if (this.start !== -1 && (this.start >= currentTime || this.end <= currentTime)) {
            return null;
        }
        if (this.pos instanceof Array) {
            return {
                x: this.pos[0],
                y: this.pos[1]
            };
        } else {
            if (this.master === null) {
                let masterId = this.pos;
                this.master = playerData.has(masterId) ? playerData.get(masterId) : trashMobData.has(masterId) ? trashMobData.get(masterId) : boss;
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
    constructor(start, end, fill, growing, color, radius, pos) {
        super(start, end, fill, growing, color);
        this.radius = inch * radius;
        this.pos = pos;
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, this.getPercent(currentTime) * this.radius, 0, 2 * Math.PI);
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
    constructor(start, end, fill, growing, color, innerRadius, outerRadius, pos) {
        super(start, end, fill, growing, color);
        this.radius = inch * 0.5 * (innerRadius + outerRadius);
        this.width = inch * (outerRadius - innerRadius);
        this.pos = pos;
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, this.radius, 0, 2 * Math.PI);
        ctx.lineWidth = (percent * this.width).toString();
        ctx.strokeStyle = this.color;
        ctx.stroke();
    }
}

class RectangleMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, width, height, pos) {
        super(start, end, fill, growing, color);
        this.height = height * inch;
        this.width = width * inch;
        this.pos = pos;
    }

    draw(ctx, currentTime) {
        const pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        const percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.rect(pos.x - 0.5 * this.width, pos.y - 0.5 * this.height, percent * this.width, percent * this.height);
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

class PieMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, direction, openingAngle, radius, pos) {
        super(start, end, fill, growing, color);
        this.direction = direction * Math.PI / 180;
        this.openingAngle = 0.5 * openingAngle * Math.PI / 180;
        this.radius = inch * radius;
        this.dx = Math.cos(this.direction - this.openingAngle) * this.radius;
        this.dy = Math.sin(this.direction - this.openingAngle) * this.radius;
        this.pos = pos;
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

let actors = [];


function createAllActors() {
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
                boss = new BossIconDrawable(actor.Img, 40, actor.Positions);
                break;
            case "Mob":
                trashMobData.set(actor.ID, new MobIconDrawable(actor.Start, actor.End, actor.Img, 30, actor.Positions));
                break;
            case "Circle":
                mechanicActorData.add(new CircleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Radius, actor.Position));
                break;
            case "Rectangle":
                mechanicActorData.add(new RectangleMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Width, actor.Height, actor.Position));
                break;
            case "Doughnut":
                mechanicActorData.add(new DoughnutMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.InnerRadius, actor.OuterRadius, actor.Position));
                break;
            case "Pie":
                mechanicActorData.add(new PieMechanicDrawable(actor.Start, actor.End, actor.Fill, actor.Growing, actor.Color, actor.Direction, actor.OpeningAngle, actor.Radius, actor.Position));
                break;
        }
    }
}
