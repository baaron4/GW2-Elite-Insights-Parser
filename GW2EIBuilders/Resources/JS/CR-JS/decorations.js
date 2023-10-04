/*jshint esversion: 6 */
/* jshint node: true */
/*jslint browser: true */
/*global animator, ToRadians, overheadAnimationFrame, maxOverheadAnimationFrame, facingIcon, animateCanvas, noUpdateTime, SkillDecorationCategory*/
"use strict";
//// BASE MECHANIC

function interpolatedPositionFetcher(connection, master) {
    var index = -1;
    var totalPoints = connection.positions.length / 3;
    var time = animator.reactiveDataStatus.time;
    for (var i = 0; i < totalPoints; i++) {
        var posTime = connection.positions[3 * i + 2];
        if (time < posTime) {
            break;
        }
        index = i;
    }
    if (index === -1) {
        return {
            x: connection.positions[0],
            y: connection.positions[1]
        };
    } else if (index === totalPoints - 1) {
        return {
            x: connection.positions[3 * index],
            y: connection.positions[3 * index + 1]
        };
    } else {
        var cur = {
            x: connection.positions[3 * index],
            y: connection.positions[3 * index + 1]
        };
        var curTime = connection.positions[3 * index + 2];
        var next = {
            x: connection.positions[3 * (index + 1)],
            y: connection.positions[3 * (index + 1) + 1]
        };
        var nextTime = connection.positions[3 * (index + 1) + 2];
        var pt = {
            x: 0,
            y: 0
        };
        pt.x = cur.x + (time - curTime) / (nextTime - curTime) * (next.x - cur.x);
        pt.y = cur.y + (time - curTime) / (nextTime - curTime) * (next.y - cur.y);
        return pt;
    }
}

function staticPositionFetcher(connection, master) {
    return {
        x: connection.position[0],
        y: connection.position[1]
    };
}

function noOffsetFetcher(connection) {
    return {
        x: 0,
        y: 0
    };
}

function staticOffsetFetcher(connection) {
    return {
        x: animator.inchToPixel * connection.offset[0],
        y: animator.inchToPixel * connection.offset[1]
    };
}

function masterPositionFetcher(connection, master) {
    if (!master) {
        return null;
    }
    return master.getPosition();
}

function noAngleFetcher(connection, master, start, end) {
    return 0;
}

function staticAngleFetcher(connection, master, start, end) {
    var time = animator.reactiveDataStatus.time;
    var velocity = Math.min((time - start) / (end - start), 1.0);
    return connection.angles[0] + velocity * connection.angles[1];
}

function masterRotationFetcher(connection, master, start, end) {
    if (!master) {
        return null;
    }
    return master.getRotation();
}

const RotationOffsetMode = {
    addToMaster: 0,
    absoluteOrientation: 1,
    rotateAfterTranslationOffset: 2,
};

class MechanicDrawable {
    constructor(start, end, connectedTo, rotationConnectedTo) {
        this.start = start;
        this.end = end;
        this.positionFetcher = null;
        this.connectedTo = connectedTo;
        if (connectedTo.interpolationMethod >= 0) {
            this.positionFetcher = interpolatedPositionFetcher;
        } else if (connectedTo.position) {
            this.positionFetcher = staticPositionFetcher;
        } else if (connectedTo.masterId >= 0) {         
            this.positionFetcher = masterPositionFetcher;
        }
        this.offsetFetcher = noOffsetFetcher;
        if (connectedTo.offset) {
            this.offsetFetcher = staticOffsetFetcher;
        }
        this.rotationFetcher = noAngleFetcher;
        this.rotationConnectedTo = rotationConnectedTo;
        this.rotationOffset = 0;
        this.rotationOffsetMode = RotationOffsetMode.addToMaster;
        if (rotationConnectedTo) {
            if (rotationConnectedTo.angles) {
                this.rotationFetcher = staticAngleFetcher;
            } else if (rotationConnectedTo.masterId) {
                this.rotationFetcher = masterRotationFetcher;
                this.rotationOffset = rotationConnectedTo.rotationOffset;
                this.rotationOffsetMode = rotationConnectedTo.rotationOffsetMode;
            }
        }
        this.master = null;
        this.rotationMaster = null;
        // Skill mode
        this.ownerID = null;
        this.owner = null;
        this.category = 0;
    }

    usingSkillMode(ownerID, category) {
        this.ownerID = ownerID;
        this.category = category;
        return this;
    }

    draw() {
        console.error("Draw should be overriden");
        // to override
    }

    getOffset() {
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start > time || this.end < time)) {
            return null;
        }
        return this.offsetFetcher(this.connectedTo);
    }

    getRotation() {
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start > time || this.end < time)) {
            return null;
        }
        return this.rotationFetcher(this.rotationConnectedTo, this.rotationMaster, this.start, this.end);
    }

    getPosition() {
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start > time || this.end < time)) {
            return null;
        }
        return this.positionFetcher(this.connectedTo, this.master);
    }

    moveContext(ctx, pos, rot) {
        const angle = ToRadians(rot);
        const offsetAngle = ToRadians(this.rotationOffset);
        const offset = this.getOffset();
        const offsetAfterRotation = this.connectedTo.offsetAfterRotation;
        ctx.translate(pos.x, pos.y);
        if (!offsetAfterRotation) {       
            ctx.translate(offset.x, offset.y);   
        }
        ctx.rotate(angle);
        if (offsetAngle !== 0 && this.rotationOffsetMode === RotationOffsetMode.addToMaster) {
            ctx.rotate(offsetAngle);
        }
        if (offsetAfterRotation) {       
            ctx.translate(offset.x, offset.y);   
        }
        if (offsetAngle !== 0 && this.rotationOffsetMode === RotationOffsetMode.rotateAfterTranslationOffset) {
            ctx.rotate(offsetAngle);
        }
        if (offsetAngle !== 0 && this.rotationOffsetMode === RotationOffsetMode.absoluteOrientation) {
            ctx.rotate(-angle);
            ctx.rotate(offsetAngle);
        }
    }

    canDraw() {
        if (this.connectedTo === null) {
            return false;
        }
        if (this.positionFetcher === masterPositionFetcher) {
            if (this.master === null) {
                let masterId = this.connectedTo.masterId;
                this.master = animator.getActorData(masterId);
            }
            if (!this.master || (!this.master.canDraw() && !this.ownerID )) {
                return false;
            }
        }
        if (this.rotationFetcher === masterRotationFetcher) {
            if (this.rotationMaster === null) {
                let masterId = this.rotationConnectedTo.masterId;
                this.rotationMaster = animator.getActorData(masterId);
            }
            if (!this.rotationMaster || (!this.rotationMaster.canDraw() && !this.ownerID)) {
                return false;
            }
        }
        if (this.ownerID !== null) {
            if (this.owner === null) {
                this.owner = animator.getActorData(this.ownerID);
            }
            if (!this.owner) {
                return false;
            }
            let renderMask = animator.displaySettings.skillMechanicsMask;
            let drawOnSelect = (renderMask & SkillDecorationCategory["Show On Select"]) > 0;
            renderMask &= ~SkillDecorationCategory["Show On Select"];
            if ((this.category & renderMask) > 0) {
                return true;
            } else if (drawOnSelect && (this.owner.isSelected() || (this.owner.master && this.owner.master.isSelected()))) {
                return true;
            }
            return false;
        }
        return true;
    }

}
//// FACING
class FacingMechanicDrawable extends MechanicDrawable {
    constructor(start, end, connectedTo, rotationConnectedTo) {
        super(start, end, connectedTo, rotationConnectedTo);
    }

    draw() {
        if (!this.canDraw()) {
            return;
        }
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        var ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        const facingFullSize = 5 * this.master.getSize() / 3;
        const facingHalfSize = facingFullSize / 2;
        if (this.master !== null && animator.coneControl.enabled && this.master.isSelected()) {           
            ctx.save(); 
            var coneOpening = ToRadians(animator.coneControl.openingAngle);
            ctx.rotate(0.5 * coneOpening);
            var coneRadius = animator.inchToPixel * animator.coneControl.radius;
            ctx.beginPath();
            ctx.arc(0, 0, coneRadius, -coneOpening, 0, false);
            ctx.arc(0, 0, 0, 0, coneOpening, true);
            ctx.closePath();
            ctx.fillStyle = "rgba(0, 255, 200, 0.3)";
            ctx.fill();
            ctx.restore();
        }
        ctx.drawImage(facingIcon, -facingHalfSize, -facingHalfSize, facingFullSize, facingFullSize);
        ctx.restore();
    }
}
//// FORMS
class FormMechanicDrawable extends MechanicDrawable {
    constructor(start, end, fill, growing, color, connectedTo, rotationConnectedTo) {
        super(start, end, connectedTo, rotationConnectedTo);
        this.fill = fill;
        this.growing = growing;
        this.color = color;
    }

    getPercent() {
        if (this.growing === 0) {
            return 1.0;
        }
        var time = animator.reactiveDataStatus.time;
        var value = Math.min((time - this.start) / (Math.abs(this.growing) - this.start), 1.0);
        if (this.growing < 0) {
            value = 1 - value;
        }
        return value;
    }
}

class CircleMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, radius, connectedTo, rotationConnectedTo, minRadius) {
        super(start, end, fill, growing, color, connectedTo, rotationConnectedTo);
        this.radius = radius;
        this.minRadius = minRadius;
    }

    draw() {
        if (!this.canDraw()) {
            return;
        }
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        var ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        ctx.beginPath();
        ctx.arc(0, 0, this.getPercent() * (this.radius - this.minRadius) + this.minRadius, 0, 2 * Math.PI);
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

class DoughnutMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, innerRadius, outerRadius, connectedTo, rotationConnectedTo) {
        super(start, end, fill, growing, color, connectedTo, rotationConnectedTo);
        this.outerRadius = outerRadius;
        this.innerRadius = innerRadius;
    }

    draw() {
        if (!this.canDraw()) {
            return;
        }
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        const percent = this.getPercent();
        var ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        ctx.beginPath();
        if (this.growing < 0) {    
            ctx.arc(0, 0, this.outerRadius , 2 * Math.PI, 0, false);
            ctx.arc(0, 0, this.innerRadius + percent * (this.outerRadius - this.innerRadius), 0, 2 * Math.PI, true);
        }  else {
            ctx.arc(0, 0, this.innerRadius + percent * (this.outerRadius - this.innerRadius), 2 * Math.PI, 0, false);
            ctx.arc(0, 0, this.innerRadius, 0, 2 * Math.PI, true);
        }
        ctx.closePath();
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

class RectangleMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, width, height, connectedTo, rotationConnectedTo) {
        super(start, end, fill, growing, color, connectedTo, rotationConnectedTo);
        this.height = height;
        this.width = width;
    }

    draw() {
        if (!this.canDraw()) {
            return;
        }
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        const percent = this.getPercent();
        var ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        ctx.beginPath();
        ctx.rect( - 0.5 * percent * this.width, - 0.5 * percent * this.height, percent * this.width, percent * this.height);
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
    constructor(start, end, fill, growing, color, openingAngle, radius, connectedTo, rotationConnectedTo) {
        super(start, end, fill, growing, color, connectedTo, rotationConnectedTo);
        this.openingAngleRadians = ToRadians(openingAngle);
        this.halfOpeningAngle = 0.5 * openingAngle;
        this.radius = radius;
    }

    draw() {
        if (!this.canDraw()) {
            return;
        }
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        var ctx = animator.mainContext;
        const percent = this.getPercent();
        ctx.save();
        this.moveContext(ctx, pos, rot + this.halfOpeningAngle);
        ctx.beginPath();
        ctx.arc(0, 0, percent * this.radius, -this.openingAngleRadians, 0, false);
        ctx.arc(0, 0, 0, 0, this.openingAngleRadians, true);
        ctx.closePath();
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

class LineMechanicDrawable extends FormMechanicDrawable {
    constructor(start, end, fill, growing, color, connectedFrom, connectedTo) {
        super(start, end, fill, growing, color, connectedTo, null);
        this.connectedFrom = connectedFrom;
        this.targetPositionFetcher = null;
        if (connectedFrom.interpolationMethod >= 0) {
            this.targetPositionFetcher = interpolatedPositionFetcher;
        } else if (connectedFrom.position instanceof Array) {
            this.targetPositionFetcher = staticPositionFetcher;
        } else {
            this.targetPositionFetcher = masterPositionFetcher;
        }
        this.targetOffsetFetcher = noOffsetFetcher;
        if (connectedFrom.offset) {
            this.targetOffsetFetcher = staticOffsetFetcher;
        }
        this.endMaster = null;
    }

    getTargetPosition() {
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start > time || this.end < time)) {
            return null;
        }
        var pos = this.targetPositionFetcher(this.connectedFrom, this.endMaster);
        if (!pos) {
            return null;
        }
        var offset = this.targetOffsetFetcher(this.connectedFrom);
        pos.x += offset.x;
        pos.y += offset.y;
        return pos;
    }
    
    canDraw() {
        if (this.connectedFrom === null) {
            return false;
        }
        if (this.targetPositionFetcher === masterPositionFetcher) {
            if (this.endMaster === null) {
                let masterId = this.connectedFrom.masterId;
                this.endMaster = animator.getActorData(masterId);
            }
            if (!this.endMaster || !this.endMaster.canDraw()) {
                return false;
            }
        }
        return super.canDraw();
    }

    draw() {
        if (!this.canDraw()) {
            return;
        }
        const pos = this.getPosition();
        const target = this.getTargetPosition();
        if (pos === null || target === null) {
            return;
        }
        const percent = this.getPercent();
        var ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, 0);
        ctx.beginPath();
        ctx.moveTo(0, 0);
        ctx.lineTo(percent * (target.x - pos.x), percent * (target.y - pos.y));
        ctx.lineWidth = (2 / animator.scale).toString();
        ctx.strokeStyle = this.color;
        ctx.stroke();
        ctx.restore();
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
        return 0;
    }

    getPosition() {
        // to override
        return null;
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

class IconMechanicDrawable extends MechanicDrawable {
    constructor(start, end, connectedTo, rotationConnectedTo, image, pixelSize, worldSize, opacity) {
        super(start, end, connectedTo, rotationConnectedTo);
        this.image = new Image();
        this.image.src = image;
        this.image.onload = () => animateCanvas(noUpdateTime);
        this.pixelSize = pixelSize;
        this.worldSize = worldSize;
        this.opacity = opacity;
    }

    getSize() {
        if (animator.displaySettings.useActorHitboxWidth && this.worldSize > 0) {
            return this.worldSize;
        } else {
            return this.pixelSize / animator.scale;
        }
    }

    getSecondaryOffset() {
        return null;
    }

    draw() {
        if (!this.canDraw()) {
            return;
        }
        const pos = this.getPosition();
        const rot = this.getRotation();
        if (pos === null || rot === null) {
            return;
        }
        const secondaryOffset = this.getSecondaryOffset();
        const size = this.getSize();
        
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        ctx.globalAlpha = this.opacity;
        if (secondaryOffset) {        
            ctx.translate(secondaryOffset.x, secondaryOffset.y);
        }
        // Don't rotate the icon
        ctx.rotate(-ToRadians(rot + this.rotationOffset));
        ctx.drawImage(this.image, - size / 2, - size / 2, size, size);
        ctx.restore();
    }
}

class IconOverheadMechanicDrawable extends IconMechanicDrawable {
    constructor(start, end, connectedTo, rotationConnectedTo, image, pixelSize, worldSize, opacity) {
        super(start, end, connectedTo, rotationConnectedTo, image, pixelSize, worldSize, opacity);
    }

    getSize() {
        if (animator.displaySettings.useActorHitboxWidth && this.worldSize > 0) {
            return this.worldSize;
        } else {
            return this.pixelSize / animator.scale;
        }
    }

    getSecondaryOffset() {
        if (!this.master) {
            console.error('Invalid IconOverhead decoration');
            return null; 
        }
        const masterSize = this.master.getSize();
        const scale = animator.displaySettings.useActorHitboxWidth ? 1/animator.inchToPixel : animator.scale;
        let offset = {
            x: 0,
            y: 0,
        };
        offset.y -= masterSize/4 + this.getSize()/2 + 3 * overheadAnimationFrame/ maxOverheadAnimationFrame / scale;
        return offset;
    }
}
