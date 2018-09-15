let deadIcon = new Image();
deadIcon.src = "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
let downIcon = new Image();
downIcon.src = "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
let time = 0;
let inch = 0;
let speed = 1;
let times = [];
let boss = null;
let playerData = new Map();
let trashMobData = new Map();
let mechanicActorData = new Set();
let rangeControl = new Map();
let selectedGroup = -1;
let selectedPlayer = null;
let timeSlider = document.getElementById('timeRange');
let timeSliderDisplay = document.getElementById('timeRangeDisplay');
let canvas = document.getElementById('replayCanvas');
let ctx = canvas.getContext('2d');
let bgImage = new Image();
let bgLoaded = false;
let animation = null;
// 60 fps by default
const timeOffset = 16;

// canvas
ctx.imageSmoothingEnabled = true;
ctx.imageSmoothingQuality = 'high';

// Animation methods
function animateCanvas() {
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
    time = Math.min(time + speed * timeOffset, lastTime);
    if (animation !== null && bgLoaded) {
        animation = requestAnimationFrame(animateCanvas);
    }
}
bgImage.onload = function () {
    animateCanvas();
    bgLoaded = true;
}
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
    updateTextInput(time)
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
                document.getElementById('id' + pId).classList.remove('active')
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
        let offsetedIndex = currentIndex - startIndex;
        let positionX = this.pos[2 * offsetedIndex];
        let positionY = this.pos[2 * offsetedIndex + 1];
        let timeValue = times[currentIndex];
        if (offsetedIndex < this.pos.length - 2) {
            let nextTimeValue = times[currentIndex + 1];
            let nextPositionX = this.pos[2 * offsetedIndex + 2];
            let nextPositionY = this.pos[2 * offsetedIndex + 3];
            return {
                x: positionX + (currentTime - timeValue) / (nextTimeValue - timeValue) * (nextPositionX - positionX),
                y: positionY + (currentTime - timeValue) / (nextTimeValue - timeValue) * (nextPositionY - positionY)
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
        let lastTime = times[times.length - 1];
        let startIndex = Math.round((times.length - 1) * Math.max(this.start, 0) / lastTime);
        let currentIndex = Math.round((times.length - 1) * currentTime / lastTime);
        return this.getInterpolatedPosition(startIndex, currentIndex, currentTime);
    }
}

class IconDrawable extends Drawable {
    constructor(start, end, imgSrc, pixelSize) {
        super(start, end);
        this.img = new Image();
        this.img.src = ImgSrc;
        this.pixelSize = pixelSize;
    }

    draw(ctx, currentTime) {
        let pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        let halfSize = this.pixelSize / 2;
        ctx.drawImage(this.img,
            pos.x - halfSize, pos.y - halfSize, this.pixelSize, this.pixelSize);
    }

}

class PlayerIconDrawable extends IconDrawable {
    constructor(start, end, imgSrc, pixelSize, group) {
        super(start, end, imgSrc, pixelSize);
        this.dead = null;
        this.down = null;
        this.selected = false;
        this.group = group;
    }

    draw(ctx, currentTime) {
        let pos = this.getPosition(currentTime);
        if (pos === null) {
            return;
        }
        let halfSize = this.pixelSize / 2;
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
        if (this.dead === null) {
            return false;
        }
        for (var i = 0; i < this.dead.length; i++) {
            let deadItem = this.dead[i];
            if (deadItem[0] <= currentTime && deadItem[1] >= currentTime) {
                return true;
            }
        }
        return false;
    }

    downed(currentTime) {
        if (this.down === null) {
            return false;
        }
        for (var i = 0; i < this.down.length; i++) {
            let downItem = this.down[i];
            if (downItem[0] <= currentTime && downItem[1] >= currentTime) {
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
        return img;
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
            }
        } else {
            if (this.master === null) {
                let masterId = this.pos;
                this.master = playerData.has(masterId) ? playerData.get(masterId) : (trashMobData.has(masterId) ? trashMobData.get(masterId) : boss);
            }
            return master.getPosition(currentTime);
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
    constructor(start, end, fill, growing, color, radius) {
        super(start, end, fill, growing, color);
        this.radius = inch * radius;
    }

    draw(ctx, currentTime) {
        let pos = this.getPosition(currentTime);
        if (pos == null) {
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
    constructor(start, end, fill, growing, color, innerRadius,outerRadius) {
        super(start, end, fill, growing, color);
        this.radius = inch *  0.5 * (innerRadius + outerRadius);
        this.width = inch * (outerRadius - innerRadius);
    }

    draw(ctx, currentTime) {
        let pos = this.getPosition(currentTime);
        if (pos == null) {
            return;
        }
        let percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.arc(pos.x, pos.y, this.radius, 0, 2 * Math.PI);
        ctx.lineWidth = (percent * this.width).toString();
        ctx.strokeStyle = this.color;
        ctx.stroke();
    }
}

class RectangleMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, width, height) {
        super(start, end, fill, growing, color);
        this.height = height * inch;
        this.width = width * inch;
    }

    draw(ctx, currentTime) {
        let pos = this.getPosition(currentTime);
        if (pos == null) {
            return;
        }
        let percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.rect(pos.x - 0.5*this.width , pos.y - 0.5*this.height , percent * this.width , percent * this.height );
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
    constructor(start, end, fill, growing, color, direction, openingAngle, radius) {
        super(start, end, fill, growing, color);
        this.direction = direction * Math.PI / 180;
        this.openingAngle = 0.5* openingAngle * Math.PI / 180;
        this.radius = inch *radius;
        this.dx = Math.cos(this.direction - this.openingAngle) * this.radius;
        this.dy = Math.sin(this.direction - this.openingAngle) * this.radius;
    }

    draw(ctx, currentTime) {
        let pos = this.getPosition(currentTime);
        if (pos == null) {
            return;
        }
        let percent = this.getPercent(currentTime);
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
        ctx.lineTo(pos.x + this.dx * percent, pos.y + this.dy * percent);
        ctx.arc(pos.x, pos.y, percent *  this.radius, this.direction - this.openingAngle, this.direction  + this.openingAngle);
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

// .... etc, move all other static methods here

/*


Another thing... right now your code looks like this:

{var a = new circleActor(100,true,2493,'rgba(0, 50, 200, 0.5)',2024,2410);mechanicData.add(a);a.pos ='358';}
{var p = new secondaryActor('https://i.imgur.com/elHjamF.png',4145,4441);secondaryData.set('733_4145_4441',p);p.pos = [697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101,];}

I would change that actual data to json-compatible objects, something like this:

var actors = [
	{
		"type": "circle",
		"radius": 100,
		"fill": true,
		"growing": 2493,
		"color":"rgba(0, 50, 200, 0.5)",
		"start": 2024,
		"end": 2410",
		"pos": 358
	},
	{
		"type": "secondary",
		"id": "733_4145_4441",
		"imgSrc": "https://i.imgur.com/elHjamF.png",
		"start": 4145,
		"end": 4441,
		"pos": [697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101,697,101]
	}
];

addActors(actors);

The addActors() method can then iterate over them and call the different constructors, but all JS code would
be completely static and separated from the data, and can be put in a real JS file without a problem.



That's not much of a change in the code, but suddently the whole data block is json compatible and we can:
	1.) generate the data using C# JSON library, so we don't have to think about escaping and formatting
	2.) leave the js in place or export the JSON as external file and load by ajax asynchronously,
	    would be very valuable for hosters like dps.report. When the data is real JSON anyway that
		would just be a minimal change in the code


Cheers, Flomix ;-)


*/