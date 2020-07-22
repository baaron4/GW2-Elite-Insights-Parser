//// BASE MECHANIC
class MechanicDrawable {
    constructor(start, end, connectedTo) {
        this.start = start;
        this.end = end;
        this.connectedTo = connectedTo;
        this.master = null;
    }

    draw() {
        // to override
    }

    getPosition() {
        if (this.connectedTo === null) {
            return null;
        }
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start >= time || this.end <= time)) {
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

}
//// FACING
class FacingMechanicDrawable extends MechanicDrawable {
    constructor(start, end, connectedTo, facingData) {
        super(start, end, connectedTo);
        this.facingData = facingData;
    }

    getRotation() {
        if (this.facingData.length === 0) {
            return null;
        }
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start >= time || this.end <= time)) {
            return null;
        }
        if (this.facingData.length === 1) {
            return this.facingData[0];
        }
        const lastTime = animator.times[animator.times.length - 1];
        const startIndex = Math.ceil((animator.times.length - 1) * Math.max(this.start, 0) / lastTime);
        const currentIndex = Math.floor((animator.times.length - 1) * time / lastTime);
        const offsetedIndex = Math.max(currentIndex - startIndex, 0);
        return this.facingData[offsetedIndex]; 
    }

    draw() {
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        var ctx = animator.mainContext;
        const angle = rot * Math.PI / 180;
        ctx.save();
        ctx.translate(pos.x, pos.y);
        ctx.rotate(angle);
        const facingFullSize = 5 * this.master.pixelSize / (3 * animator.scale);
        const facingHalfSize = facingFullSize / 2;
        ctx.drawImage(facingIcon, -facingHalfSize, -facingHalfSize, facingFullSize, facingFullSize);
        ctx.restore();
    }
}

class FacingRectangleMechanicDrawable extends FacingMechanicDrawable {
    constructor(start, end, connectedTo, facingData, width, height, color) {
        super(start, end, connectedTo, facingData);
        this.width = width;
        this.height = height;
        this.color = color;
    }

    draw() {
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        var ctx = animator.mainContext;
        const angle = rot * Math.PI / 180;
        ctx.save();
        ctx.translate(pos.x, pos.y);
        ctx.rotate(angle);
        ctx.beginPath();
        ctx.rect(- 0.5 * this.width, - 0.5 * this.height, this.width, this.height);
        ctx.fillStyle = this.color;
        ctx.fill();
        ctx.restore();
    }
}
//// FORMS
class FormMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, connectedTo) {
        super(start, end, connectedTo);
        this.fill = fill;
        this.growing = growing;
        this.color = color;
    }

    getPercent() {
        if (this.growing === 0) {
            return 1.0;
        }
        var time = animator.reactiveDataStatus.time;
        return Math.min((time - this.start) / (this.growing - this.start), 1.0);
    }
}

class CircleMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, radius, connectedTo, minRadius) {
        super(start, end, fill, growing, color, connectedTo);
        this.radius = radius;
        this.minRadius = minRadius;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.mainContext;
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

class DoughnutMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, innerRadius, outerRadius, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.outerRadius = outerRadius;
        this.innerRadius = innerRadius;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.mainContext;
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

class RectangleMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, width, height, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.height = height;
        this.width = width;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.mainContext;
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
    constructor(start, end, fill, growing, color, width, height, rotation, translation, spinangle, connectedTo) {
        super(start, end, fill, growing, color, width, height, connectedTo);
        this.rotation = -rotation * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
        this.translation = translation;
        this.spinangle = -spinangle * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
    }

    getSpinPercent() {
        if (this.spinangle === 0) {
            return 1.0;
        }
        var time = animator.reactiveDataStatus.time;
        return Math.min((time - this.start) / (this.end - this.start), 1.0);
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.mainContext;
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

class PieMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, direction, openingAngle, radius, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.direction = -direction * Math.PI / 180; // positive mathematical direction, reversed since JS has downwards increasing y axis
        this.openingAngle = 0.5 * openingAngle * Math.PI / 180;
        this.radius = radius;
        this.dx = Math.cos(this.direction - this.openingAngle) * this.radius;
        this.dy = Math.sin(this.direction - this.openingAngle) * this.radius;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.mainContext;
        const percent = this.getPercent();
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

class LineMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, connectedFrom, connectedTo) {
        super(start, end, fill, growing, color, connectedTo);
        this.connectedFrom = connectedFrom;
        this.endmaster = null;
    }

    getTargetPosition() {
        if (this.connectedFrom === null) {
            return null;
        }
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start >= time || this.end <= time)) {
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
        var ctx = animator.mainContext;
        const percent = this.getPercent();
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
        ctx.lineTo(pos.x + percent * (target.x - pos.x), pos.y + percent * (target.y - pos.y));
        ctx.lineWidth = (2 / animator.scale).toString();
        ctx.strokeStyle = this.color;
        ctx.stroke();
    }
}
//// BACKGROUND
class BackgroundDrawable {
    constructor(start, end) {
        this.start = start;
        this.end = end;
    }

    draw() {
        // to override
    }

    getHeight() {
        // to override
    }

    getPosition() {
        // to override
    }
}

class MovingPlatformDrawable extends BackgroundDrawable {
    constructor(start, end, image, width, height, positions) {
        super(start, end);
        this.image = new Image();
        this.image.src = image;
        this.image.onload = function () {
            animateCanvas(noUpdateTime);
        };
        this.width = width;
        this.height = height;
        this.positions = positions;
        if (this.positions.length > 1) {
            this.currentIndex = 0;
            this.currentStart = Number.NEGATIVE_INFINITY;
            this.currentEnd = positions[0][5];
        }
    }

    draw() {
        const pos = this.getInterpolatedPosition();
        if (pos === null) {
            return;
        }
        let ctx = animator.mainContext;
        const rads = pos.angle;
        ctx.save();
        ctx.translate(pos.x, pos.y);
        ctx.rotate(rads % (2 * Math.PI));
        ctx.globalAlpha = pos.opacity;
        ctx.drawImage(this.image, -0.5 * this.width, -0.5 * this.height, this.width, this.height);
        ctx.restore();
    }

    getHeight() {
        let position = this.getInterpolatedPosition();
        if (position === null) {
            return Number.NEGATIVE_INFINITY;
        }

        return position.z;
    }

    getInterpolatedPosition() {
        let time = animator.reactiveDataStatus.time;
        if (time < this.start || time > this.end) {
            return null;
        }
        if (this.positions.length === 0) {
            return null;
        }
        if (this.positions.length === 1) {
            return {
                x: this.positions[0][0],
                y: this.positions[0][1],
                z: this.positions[0][2],
                angle: this.positions[0][3],
                opacity: this.positions[0][4],
            };
        }

        let i;
        let changed = false;
        if (this.currentStart <= time && time < this.currentEnd) {
            i = this.currentIndex;
        } else {
            for (i = 0; i < this.positions.length; i++) {
                let positionTime = this.positions[i][5];
                if (positionTime > time) {
                    break;
                }
            }
            changed = true;
        }

        if (changed) {
            this.currentIndex = i;
            if (i === 0) {
                this.currentStart = Number.NEGATIVE_INFINITY;
                this.currentEnd = this.positions[0][5];
            } else {
                this.currentStart = this.positions[i - 1][5];
                if (i === this.positions.length) {
                    this.currentEnd = Number.POSITIVE_INFINITY;
                } else {
                    this.currentEnd = this.positions[i][5];
                }
            }
        }

        if (i === 0) {
            // First position is in the future
            return null;
        }

        if (i === this.positions.length) {
            // The last position is in the past, use the last position
            return {
                x: this.positions[i - 1][0],
                y: this.positions[i - 1][1],
                z: this.positions[i - 1][2],
                angle: this.positions[i - 1][3],
                opacity: this.positions[i - 1][4],
            };
        }

        let progress = (time - this.positions[i - 1][5]) / (this.positions[i][5] - this.positions[i - 1][5]);

        return {
            x: (this.positions[i - 1][0] * (1 - progress) + this.positions[i][0] * progress),
            y: (this.positions[i - 1][1] * (1 - progress) + this.positions[i][1] * progress),
            z: (this.positions[i - 1][2] * (1 - progress) + this.positions[i][2] * progress),
            angle: (this.positions[i - 1][3] * (1 - progress) + this.positions[i][3] * progress),
            opacity: (this.positions[i - 1][4] * (1 - progress) + this.positions[i][4] * progress),
        };
    }
}