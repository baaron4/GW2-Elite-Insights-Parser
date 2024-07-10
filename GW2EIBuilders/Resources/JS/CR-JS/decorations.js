/*jshint esversion: 6 */
/* jshint node: true */
/*jslint browser: true */
/*global animator, ToRadians, overheadAnimationFrame, maxOverheadAnimationFrame, facingIcon, animateCanvas, noUpdateTime, SkillDecorationCategory*/
"use strict";

class GenericDecorationMetadata {
    constructor(params) {

    }
}

class GenericAttachedDecorationMetadata extends GenericDecorationMetadata{
    constructor(params) {
        super(params);
    }
}

class ActorOrientationDecorationMetadata extends GenericAttachedDecorationMetadata {
    constructor(params) {
        super(params);
    }
}

class FormDecorationMetadata extends GenericAttachedDecorationMetadata {
    constructor(params) {
        super(params);
        this.color = params.color;
    }
}

class CircleDecorationMetadata extends FormDecorationMetadata {
    constructor(params) {
        super(params);
        this.radius = InchToPixel * params.radius;
        this.minRadius = InchToPixel * params.minRadius;
    }
}

class DoughnutDecorationMetadata extends FormDecorationMetadata {
    constructor(params) {
        super(params);
        this.outerRadius = InchToPixel * params.outerRadius;
        this.innerRadius = InchToPixel * params.innerRadius;
    }
}

class LineDecorationMetadata extends FormDecorationMetadata {
    constructor(params) {
        super(params);
    }
}

class PieDecorationMetadata extends CircleDecorationMetadata {
    constructor(params) {
        super(params);
        this.openingAngle = params.openingAngle;
        this.openingAngleRadians = ToRadians(this.openingAngle);
        this.halfOpeningAngle = 0.5 * this.openingAngle;
    }
}

class RectangleDecorationMetadata extends FormDecorationMetadata {
    constructor(params) {
        super(params);
        this.width = InchToPixel * params.width;
        this.height = InchToPixel * params.height;
    }
}

class GenericIconDecorationMetadata extends GenericAttachedDecorationMetadata{
    constructor(params) {
        super(params);
        this.imageUrl = params.image;
        this.image = new Image();
        this.image.src = this.imageUrl;
        this.image.onload = () => animateCanvas(noUpdateTime);
        this.pixelSize = params.pixelSize;
        this.worldSize = InchToPixel * params.worldSize;
    }
}

class BackgroundIconDecorationMetadata extends GenericIconDecorationMetadata {
    constructor(params) {
        super(params);
    }
}

class IconDecorationMetadata extends GenericIconDecorationMetadata {
    constructor(params) {
        super(params);
        this.opacity = params.opacity;
    }
}

class IconOverheadDecorationMetadata extends IconDecorationMetadata {
    constructor(params) {
        super(params);
    }
}

class BackgroundDecorationMetadata extends GenericDecorationMetadata{
    constructor(params) {
        super(params);
    }
}

class MovingPlatformDecorationMetadata extends BackgroundDecorationMetadata{
    constructor(params, ) {
        super(params);
        this.imageUrl = params.image;
        this.image = new Image();
        this.image.src = this.imageUrl;
        this.image.onload = () => animateCanvas(noUpdateTime);
        this.width = InchToPixel * params.width;
        this.height = InchToPixel * params.height;
    }
}


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
        x: InchToPixel * connection.offset[0],
        y: InchToPixel * connection.offset[1]
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

function interpolatedAngleFetcher(connection, master, dstMaster, start, end) {
    var index = -1;
    var totalPoints = connection.angles.length / 2;
    var time = animator.reactiveDataStatus.time;
    for (var i = 0; i < totalPoints; i++) {
        var posTime = connection.angles[2 * i + 1];
        if (time < posTime) {
            break;
        }
        index = i;
    }
    if (index === -1) {
        return connection.angles[0];
    } else if (index === totalPoints - 1) {
        return connection.angles[2 * index];
    } else {
        var cur = connection.angles[2 * index];
        var curTime = connection.angles[2 * index + 1];
        var next = connection.angles[2 * (index + 1)];
        var nextTime = connection.angles[2 * (index + 1) + 1];
        // Make sure the interpolation is only done on the shortest path to avoid big flips around PI or -PI radians
        if (next - cur < -180) {
            next += 360.0;
        } else if (next - cur > 180) {
            next -= 360.0;
        }
        var interpolatedAngle = cur + (time - curTime) / (nextTime - curTime) * (next - cur);
        return interpolatedAngle;
    }
}

function staticAngleFetcher(connection, master, dstMaster, start, end) {
    var time = animator.reactiveDataStatus.time;
    var velocity = Math.min((time - start) / (end - start), 1.0);
    return connection.angles[0] + velocity * connection.angles[1];
}

function masterRotationFetcher(connection, master, dstMaster, start, end) {
    if (!master) {
        return null;
    }
    return master.getRotation();
}

function masterToMasterRotationFetcher(connection, master, dstMaster, start, end) {
    if (!master || !dstMaster) {
        return null;
    }
    var origin = master.getPosition();
    var dst = dstMaster.getPosition();
    if (!origin || !dst) {
        return null;
    }
    var vector = {
        x: dst.x - origin.x,
        y: dst.y - origin.y,
    }
    return ToDegrees(Math.atan2(vector.y, vector.x));
}

const RotationOffsetMode = {
    addToMaster: 0,
    absoluteOrientation: 1,
    rotateAfterTranslationOffset: 2,
};

class MechanicDrawable {
    constructor(params) {
        this.start = params.start;
        this.end = params.end;
        this.positionFetcher = null;
        this.connectedTo = params.connectedTo;
        if (this.connectedTo.interpolationMethod >= 0) {
            this.positionFetcher = interpolatedPositionFetcher;
        } else if (this.connectedTo.position) {
            this.positionFetcher = staticPositionFetcher;
        } else if (this.connectedTo.masterId >= 0) {         
            this.positionFetcher = masterPositionFetcher;
        }
        this.offsetFetcher = noOffsetFetcher;
        if (this.connectedTo.offset) {
            this.offsetFetcher = staticOffsetFetcher;
        }
        this.rotationFetcher = noAngleFetcher;
        this.rotationConnectedTo = params.rotationConnectedTo;
        this.rotationOffset = 0;
        this.rotationOffsetMode = RotationOffsetMode.addToMaster;
        if (this.rotationConnectedTo) {
            if (this.rotationConnectedTo.interpolationMethod >= 0) {
                this.rotationFetcher = interpolatedAngleFetcher;
            } else if (this.rotationConnectedTo.angles) {
                this.rotationFetcher = staticAngleFetcher;
            } else if (this.rotationConnectedTo.dstMasterId) {
                this.rotationFetcher = masterToMasterRotationFetcher;
            } else if (this.rotationConnectedTo.masterId) {
                this.rotationFetcher = masterRotationFetcher;
                this.rotationOffset = this.rotationConnectedTo.rotationOffset;
                this.rotationOffsetMode = this.rotationConnectedTo.rotationOffsetMode;
            }
        }
        this.master = null;
        this.rotationMaster = null;
        this.dstRotationMaster = null;
        // Skill mode
        this.ownerID = null;
        this.owner = null;
        this.category = 0;
        if (params.skillMode) {
            this.ownerID = params.skillMode.owner;
            this.category = params.skillMode.category;
        }
        //
        this.metadata = params._metadataContainer.get(params.metadataSignature);
    }

    draw() {
        console.error("Draw should be overriden");
        // to override
    }

    getOffset() {
        var time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
            return null;
        }
        return this.offsetFetcher(this.connectedTo);
    }

    getRotation() {
        var time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
            return null;
        }
        return this.rotationFetcher(this.rotationConnectedTo, this.rotationMaster, this.dstRotationMaster, this.start, this.end);
    }

    getPosition() {
        var time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
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
        if (this.rotationFetcher === masterRotationFetcher || this.rotationFetcher === masterToMasterRotationFetcher) {
            if (this.rotationMaster === null) {
                let masterId = this.rotationConnectedTo.masterId;
                this.rotationMaster = animator.getActorData(masterId);
            }
            if (!this.rotationMaster || (!this.rotationMaster.canDraw() && !this.ownerID)) {
                return false;
            }
            if (this.rotationFetcher === masterToMasterRotationFetcher) {
                if (this.dstRotationMaster === null) {
                    let dstMasterId = this.rotationConnectedTo.dstMasterId;
                    this.dstRotationMaster = animator.getActorData(dstMasterId);
                }
                if (!this.dstRotationMaster || (!this.dstRotationMaster.canDraw() && !this.ownerID)) {
                    return false;
                }
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
    constructor(params) {
        super(params);
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
            var coneRadius = InchToPixel * animator.coneControl.radius;
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
    constructor(params) {
        super(params);
        this.fill = !!params.fill;
        this.growingEnd = !!params.growingEnd;
        this.growingReverse = !!params.growingReverse;
    }

    get color() {
        return this.metadata.color;
    }

    getPercent() {
        if (this.growingEnd <= this.start) {
            return 1.0;
        }
        var time = animator.reactiveDataStatus.time;
        var value = Math.min((time - this.start) / (this.growingEnd - this.start), 1.0);
        if (this.growingReverse) {
            value = 1 - value;
        }
        return value;
    }
}

class CircleMechanicDrawable extends FormMechanicDrawable {
    constructor(params) {
        super(params);
    }

    get radius() {
        return this.metadata.radius;
    }

    get minRadius() {
        return this.metadata.minRadius;
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
    constructor(params) {
        super(params);
    }

    get outerRadius() {
        return this.metadata.outerRadius;
    }

    get innerRadius() {
        return this.metadata.innerRadius;
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
        if (this.growingReverse) {    
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
    constructor(params) {
        super(params);
    }

    get height() {
        return this.metadata.height;
    }

    get width() {
        return this.metadata.width;
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
    constructor(params) {
        super(params);
    }

    get openingAngleRadians() {
        return this.metadata.openingAngleRadians;
    }

    get halfOpeningAngle() {
        return this.metadata.halfOpeningAngle;
    }

    get radius() {
        return this.metadata.radius;
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
    constructor(params) {
        super(params);
        this.connectedFrom = params.connectedFrom;
        this.targetPositionFetcher = null;
        if (this.connectedFrom.interpolationMethod >= 0) {
            this.targetPositionFetcher = interpolatedPositionFetcher;
        } else if (this.connectedFrom.position instanceof Array) {
            this.targetPositionFetcher = staticPositionFetcher;
        } else {
            this.targetPositionFetcher = masterPositionFetcher;
        }
        this.targetOffsetFetcher = noOffsetFetcher;
        if (this.connectedFrom.offset) {
            this.targetOffsetFetcher = staticOffsetFetcher;
        }
        this.endMaster = null;
    }

    getTargetPosition() {
        var time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
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
        if (this.growingReverse) {
            this.moveContext(ctx, target, 0);
            ctx.beginPath();
            ctx.moveTo(0, 0);
            ctx.lineTo(( 1 - percent) * (pos.x - target.x), percent * (pos.y - target.y));
        } else {
            this.moveContext(ctx, pos, 0);
            ctx.beginPath();
            ctx.moveTo(0, 0);
            ctx.lineTo(percent * (target.x - pos.x), percent * (target.y - pos.y));
        }
        
        ctx.lineWidth = (2 / animator.scale).toString();
        ctx.strokeStyle = this.color;
        ctx.stroke();
        ctx.restore();
    }
}
//// BACKGROUND
class BackgroundDrawable {
    constructor(params) {
        this.start = params.start;
        this.end = params.end;
        this.metadata = params._metadataContainer.get(params.metadataSignature);
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
    constructor(params) {
        super(start, end);
        this.positions = params.positions;
        if (this.positions.length > 1) {
            this.currentIndex = 0;
            this.currentStart = Number.NEGATIVE_INFINITY;
            this.currentEnd = positions[0][5];
        }
    }

    get image() {
        return this.metadata.image;
    }
    
    get imageUrl() {
        return this.metadata.imageUrl;
    }
    
    get height() {
        return this.metadata.height;
    }

    get width() {
        return this.metadata.width;
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
///
class IconMechanicDrawable extends MechanicDrawable {
    constructor(params) {
        super(params);
        this.canRotate = false;
    }

    get image() {
        return this.metadata.image;
    }

    get imageUrl() {
        return this.metadata.imageUrl;
    }
    
    get pixelSize() {
        return this.metadata.pixelSize;
    }

    get worldSize() {
        return this.metadata.worldSize;
    }

    getSize() {
        if (animator.displaySettings.useActorHitboxWidth && this.worldSize > 0) {
            return this.worldSize;
        } else if (this.pixelSize > 0){
            return this.pixelSize / animator.scale;
        } else if (this.worldSize > 0){
            return this.worldSize;
        }
    }

    getOpacity() {
        return this.metadata.opacity;
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
        ctx.globalAlpha = this.getOpacity();
        if (secondaryOffset) {        
            ctx.translate(secondaryOffset.x, secondaryOffset.y);
        }
        if(!this.canRotate) {
            // Don't rotate the icon
            ctx.rotate(-ToRadians(rot + this.rotationOffset));
        }
        ctx.drawImage(this.image, - size / 2, - size / 2, size, size);
        ctx.restore();
    }
}

class BackgroundIconMechanicDrawable extends IconMechanicDrawable {
    constructor(params) {
        super(params);
        this.canRotate = true;
        this.opacities = params.opacities;
        this.heights = params.heights;
    }

    getHeight() {
        let index = -1;
        const heights = this.heights;
        const totalPoints = heights.length / 2;
        const time = animator.reactiveDataStatus.time;
        for (var i = 0; i < totalPoints; i++) {
            let heightTime = heights[2 * i + 1];
            if (time < heightTime) {
                index = i - 1;
                break;
            }
            index = i;
        }
        if (index === -1) {
            return heights[0];
        } else if (index === totalPoints - 1) {
            return heights[2 * index]
        } else {
            return heights[2 * index];
        }
    }

    getOpacity() {
        let index = -1;
        const opacities = this.opacities;
        const totalPoints = opacities.length / 2;
        const time = animator.reactiveDataStatus.time;
        let interpolate = 0;
        for (var i = 0; i < totalPoints; i++) {
            let opacityTime = opacities[2 * i + 1];
            if (time < opacityTime) {
                if (opacityTime - time <= 1500) interpolate = opacityTime;
                index = i - 1;
                break;
            }
            index = i;
        }
        if (index === -1) {
            return opacities[0];
        } else if (interpolate > 0) {
            return opacities[2 * (index + 1)] - (interpolate - time) * (opacities[2 * (index + 1)] - opacities[2 * index ]) / 1500;
        } else {
            return opacities[2 * index];
        }
    }
}

class IconOverheadMechanicDrawable extends IconMechanicDrawable {
    constructor(params) {
        super(params);
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
        const scale = animator.displaySettings.useActorHitboxWidth ? 1/InchToPixel : animator.scale;
        let offset = {
            x: 0,
            y: 0,
        };
        offset.y -= masterSize/4 + this.getSize()/2 + 3 * overheadAnimationFrame/ maxOverheadAnimationFrame / scale;
        return offset;
    }
}
