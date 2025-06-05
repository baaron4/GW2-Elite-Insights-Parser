/*jshint esversion: 6 */
/* jshint node: true */
/*jslint browser: true */
/*global animator, ToRadians, overheadAnimationFrame, maxOverheadAnimationFrame, facingIcon, animateCanvas, noUpdateTime, SkillDecorationCategory*/
"use strict";

class GenericMetadata {
    constructor(params) {

    }
}

class TextMetadata extends GenericMetadata{
    constructor(params) {
        super(params);
        this.color = params.color;
        this.backgroundColor = params.backgroundColor;
    }
}

class GenericAttachedMetadata extends GenericMetadata{
    constructor(params) {
        super(params);
    }
}

class ActorOrientationMetadata extends GenericAttachedMetadata {
    constructor(params) {
        super(params);
    }
}

class FormMetadata extends GenericAttachedMetadata {
    constructor(params) {
        super(params);
        this.color = params.color;
    }
}

class CircleMetadata extends FormMetadata {
    constructor(params) {
        super(params);
        this.radius = InchToPixel * params.radius;
        this.minRadius = InchToPixel * params.minRadius;
    }
}

class DoughnutMetadata extends FormMetadata {
    constructor(params) {
        super(params);
        this.outerRadius = InchToPixel * params.outerRadius;
        this.innerRadius = InchToPixel * params.innerRadius;
    }
}

class LineMetadata extends FormMetadata {
    constructor(params) {
        super(params);
        this.thickness = params.thickness;
        this.worldSizeThickness = params.worldSizeThickness;
        if (this.worldSizeThickness) {
            this.thickness *= InchToPixel;
        }
    }
}

class PieMetadata extends CircleMetadata {
    constructor(params) {
        super(params);
        this.openingAngle = params.openingAngle;
        this.openingAngleRadians = ToRadians(this.openingAngle);
        this.halfOpeningAngle = 0.5 * this.openingAngle;
    }
}

class RectangleMetadata extends FormMetadata {
    constructor(params) {
        super(params);
        this.width = InchToPixel * params.width;
        this.height = InchToPixel * params.height;
    }
}

class ProgressBarMetadata extends RectangleMetadata {
    constructor(params) {
        super(params);
        this.secondaryColor = params.secondaryColor;
    }
}

class OverheadProgressBarMetadata extends ProgressBarMetadata {
    constructor(params) {
        super(params);
        this.pixelWidth = params.pixelWidth;
        this.pixelHeight = params.pixelHeight;
    }
}

class GenericIconMetadata extends GenericAttachedMetadata{
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

class BackgroundIconMetadata extends GenericIconMetadata {
    constructor(params) {
        super(params);
    }
}

class IconMetadata extends GenericIconMetadata {
    constructor(params) {
        super(params);
        this.opacity = params.opacity;
    }
}

class IconOverheadMetadata extends IconMetadata {
    constructor(params) {
        super(params);
    }
}

class BackgroundMetadata extends GenericMetadata{
    constructor(params) {
        super(params);
    }
}

class MovingPlatformMetadata extends BackgroundMetadata{
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

const InterpolationMethod = {
    LINEAR: 0,
    STEP: 1,
};

function interpolatedPositionFetcher(connection, master, start, end) {
    let index = -1;
    const totalPoints = connection.positions.length / 3;
    const time = animator.reactiveDataStatus.time;
    for (let i = 0; i < totalPoints; i++) {
        const posTime = connection.positions[3 * i + 2];
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
        const cur = {
            x: connection.positions[3 * index],
            y: connection.positions[3 * index + 1]
        };
        switch (connection.interpolationMethod) {
            case InterpolationMethod.LINEAR:
                const curTime = connection.positions[3 * index + 2];
                const next = {
                    x: connection.positions[3 * (index + 1)],
                    y: connection.positions[3 * (index + 1) + 1]
                };
                const nextTime = connection.positions[3 * (index + 1) + 2];
                const pt = {
                    x: 0,
                    y: 0
                };
                pt.x = cur.x + (time - curTime) / (nextTime - curTime) * (next.x - cur.x);
                pt.y = cur.y + (time - curTime) / (nextTime - curTime) * (next.y - cur.y);
                return pt;
            case InterpolationMethod.STEP:
                return cur;
            default:
                return null;
        }
    }
}

function staticPositionFetcher(connection, master, start, end) {
    const factor = connection.isScreenSpace ? resolutionMultiplier : 1;
    return {
        x: factor * connection.position[0],
        y: factor * connection.position[1]
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

function positionToMasterPositionFetcher(connection, master, start, end) {
    if (!master) {
        return null;
    }
    const initialPosition = {
        x: connection.position[0],
        y: connection.position[1],
    }
    const initialTime = connection.position[2];
    const time = animator.reactiveDataStatus.time;
    if (time <= initialTime) {
        return null;
    }
    if (!connection._positions) { 
        const pollingRate = PollingRate / 2;
        const velocity = InchToPixel * connection.velocity;
        let currentPosition = initialPosition;
        connection._positions = [
            {
                x: currentPosition.x, 
                y: currentPosition.y, 
                time: initialTime
            }
        ];
        for (let i = 1; i < (end - start)/ pollingRate + 1; i++) {
            let nextTime = initialTime + i*pollingRate;
            const targetPosition = master._getPosition(nextTime);
            if (!targetPosition) {
                connection._positions.push({
                    x: currentPosition.x, 
                    y: currentPosition.y, 
                    time: nextTime
                });
            } else {
                const vector = {
                    x: targetPosition.x - currentPosition.x,
                    y: targetPosition.y - currentPosition.y,
                }
                const length = Math.sqrt(vector.x * vector.x +vector.y * vector.y );
                vector.x /= Math.max(length, 1e-6);
                vector.y /= Math.max(length, 1e-6);
                const factor = pollingRate * velocity;
                connection._positions.push({
                    x: currentPosition.x + factor * vector.x, 
                    y: currentPosition.y + factor * vector.y, 
                    time: nextTime
                });
                currentPosition = connection._positions[i];
            }
        }  
    }
    const positions = connection._positions;
    // TODO: optimize if necessary
    let i = 0;
    for (i = 0; i < positions.length; i++) {
        let cur = positions[i];
        if (cur.time > time) {
            break;
        }
    }
    const cur = positions[i - 1];
    const next = positions[i]
    const factor = (time - cur.time) / (next.time - cur.time);
    return {
        x:  factor* (next.x - cur.x) + cur.x,
        y:  factor* (next.y - cur.y) + cur.y
    };
}

function masterPositionFetcher(connection, master, start, end) {
    if (!master) {
        return null;
    }
    return master.getPosition();
}

function noAngleFetcher(connection, master, start, end) {
    return 0;
}

function interpolatedAngleFetcher(connection, master, dstMaster, start, end) {
    let index = -1;
    const totalPoints = connection.angles.length / 2;
    const time = animator.reactiveDataStatus.time;
    for (let i = 0; i < totalPoints; i++) {
        const posTime = connection.angles[2 * i + 1];
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
        const cur = connection.angles[2 * index];
        switch (connection.interpolationMethod) {
            case InterpolationMethod.LINEAR:
                const curTime = connection.angles[2 * index + 1];
                let next = connection.angles[2 * (index + 1)];
                const nextTime = connection.angles[2 * (index + 1) + 1];
                // Make sure the interpolation is only done on the shortest path to avoid big flips around PI or -PI radians
                if (next - cur < -180) {
                    next += 360.0;
                } else if (next - cur > 180) {
                    next -= 360.0;
                }
                const interpolatedAngle = cur + (time - curTime) / (nextTime - curTime) * (next - cur);
                return interpolatedAngle;
            case InterpolationMethod.STEP:
                return cur;
            default:
                return 0;
        }
    }
}

function spinningAngleFetcher(connection, master, dstMaster, start, end) {
    const time = animator.reactiveDataStatus.time;
    const factor = Math.max((time - start) / (end - start), 0.0);
    return connection.angle + factor * connection.spinAngle;
}

function staticAngleFetcher(connection, master, dstMaster, start, end) {
    return connection.angle;
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
    const origin = master.getPosition();
    const dst = dstMaster.getPosition();
    if (!origin || !dst) {
        return null;
    }
    const vector = {
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
        if (this.connectedTo.velocity >= 0) {
            this.positionFetcher = positionToMasterPositionFetcher;
        } else if (this.connectedTo.interpolationMethod >= 0) {
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
            } else if (this.rotationConnectedTo.spinAngle !== undefined) {
                this.rotationFetcher = spinningAngleFetcher;
            } else if (this.rotationConnectedTo.angle !== undefined) {
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
            this.ownerID = params.skillMode.owner.ownerID;
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
        const time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
            return null;
        }
        return this.offsetFetcher(this.connectedTo);
    }

    getRotation() {
        const time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
            return null;
        }
        return this.rotationFetcher(this.rotationConnectedTo, this.rotationMaster, this.dstRotationMaster, this.start, this.end);
    }

    getPosition() {
        const time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
            return null;
        }
        return this.positionFetcher(this.connectedTo, this.master, this.start, this.end);
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
        if (this.positionFetcher === masterPositionFetcher || this.positionFetcher === positionToMasterPositionFetcher) {
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
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        const facingFullSize = 5 * this.master.getSize() / 3;
        const facingHalfSize = facingFullSize / 2;
        if (this.master !== null && animator.coneControl.enabled && this.master.isSelected()) {           
            ctx.save(); 
            const coneOpening = ToRadians(animator.coneControl.openingAngle);
            ctx.rotate(0.5 * coneOpening);
            const coneRadius = InchToPixel * animator.coneControl.radius;
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
        this.growingEnd = params.growingEnd;
        this.growingReverse = !!params.growingReverse;
    }

    get color() {
        return this.metadata.color;
    }

    getPercent() {
        if (this.growingEnd <= this.start) {
            return 1.0;
        }
        const time = animator.reactiveDataStatus.time;
        let value = Math.min((time - this.start) / (this.growingEnd - this.start), 1.0);
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
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        ctx.beginPath();
        ctx.arc(0, 0, this.getPercent() * (this.radius - this.minRadius) + this.minRadius, 0, 2 * Math.PI);
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

    drawOuterCircle(percent) {      
        const ctx = animator.mainContext; 
        if (this.growingReverse) {    
            ctx.arc(0, 0, this.outerRadius , 2 * Math.PI, 0, false);
        }  else {
            ctx.arc(0, 0, this.innerRadius + percent * (this.outerRadius - this.innerRadius), 2 * Math.PI, 0, false);
        }
    }

    drawInnerCircle(percent) {      
        const ctx = animator.mainContext; 
        if (this.growingReverse) {    
            ctx.arc(0, 0, this.innerRadius + percent * (this.outerRadius - this.innerRadius), 0, 2 * Math.PI, true);
        }  else {
            ctx.arc(0, 0, this.innerRadius, 0, 2 * Math.PI, true);
        }
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
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        if (this.fill) {
            ctx.fillStyle = this.color;
            
            ctx.beginPath();
            this.drawOuterCircle(percent);
            this.drawInnerCircle(percent);
            ctx.closePath();
            ctx.fill();
        } else {  
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = this.color;

            ctx.beginPath();
            this.drawOuterCircle(percent);
            ctx.closePath();
            ctx.stroke();

            ctx.beginPath();     
            this.drawInnerCircle(percent);
            ctx.closePath();
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
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        ctx.beginPath();
        ctx.rect( - 0.5 * percent * this.width, - 0.5 * percent * this.height, percent * this.width, percent * this.height);
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

class ProgressBarMechanicDrawable extends RectangleMechanicDrawable {
    constructor(params) {
        super(params);
        this.interpolationMethod = params.interpolationMethod;
        this.progress = params.progress;
    }

    get secondaryColor() {
        return this.metadata.secondaryColor;
    }

    getSecondaryOffset() {
        return null;
    }

    computeProgress() {
        const progress = this.progress;
        let index = -1;
        const totalPoints = progress.length;
        const time = animator.reactiveDataStatus.time;
        for (let i = 0; i < totalPoints; i++) {
            const posTime = progress[i][0];
            if (time < posTime) {
                break;
            }
            index = i;
        }
        if (index === -1) {
            return progress[0][1];
        } else if (index === totalPoints - 1) {
            return progress[index][1];
        } else {
            const cur = progress[index][1];
            switch (this.interpolationMethod) {
                case InterpolationMethod.LINEAR:
                    const curTime = progress[index][0];
                    const next = progress[index + 1][1];
                    const nextTime = progress[index + 1][0];
                    const interpolated = cur + (time - curTime) / (nextTime - curTime) * (next - cur);
                    return interpolated;
                case InterpolationMethod.STEP:
                    return cur;
                default:
                    return 0;
            }
        }
    }

    getSize() {
        return {
            h: this.height,
            w: this.width,
        }
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
        const size = this.getSize();
        const progressPercent = this.computeProgress() / 100.0;
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        const secondaryOffset = this.getSecondaryOffset();
        if (secondaryOffset) {
            ctx.translate(secondaryOffset.x, secondaryOffset.y);
        }
        const normalizedRot = Math.abs((ToRadians(rot + this.rotationOffset) / Math.PI) % 2);
        if (0.5 < normalizedRot && normalizedRot < 1.5) {
            // make sure the progress bar remains upright
            ctx.rotate(-ToRadians(180));
        }
        if (progressPercent > 0) {
            ctx.beginPath();
            ctx.rect(- 0.5 * size.w, - 0.5 * size.h, progressPercent * size.w, size.h);
            ctx.closePath();
            ctx.fillStyle = this.color;
            ctx.fill();     
            //
            ctx.beginPath();
            ctx.rect(- 0.5 * size.w, - 0.5 * size.h, progressPercent * size.w, size.h);
            ctx.closePath();
            ctx.lineWidth = (3 / animator.scale).toString();
            ctx.strokeStyle = this.color;
            ctx.stroke();
            //
            ctx.beginPath();
            ctx.rect(- 0.5 * size.w, - 0.5 * size.h, size.w, size.h);
            ctx.closePath();
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = this.color;
            ctx.stroke();
            if (progressPercent < 1) {
                const reverseProgressPercent = 1.0 - progressPercent;
                ctx.beginPath();
                ctx.rect((- 0.5 + progressPercent) * size.w, - 0.5 * size.h, reverseProgressPercent * size.w, size.h);
                ctx.closePath();
                ctx.fillStyle = this.secondaryColor;
                ctx.fill();
            }
        }
        ctx.restore();
    }
}

class OverheadProgressBarMechanicDrawable extends ProgressBarMechanicDrawable {
    constructor(params) {
        super(params);
    }
    get pixelHeight() {
        return this.metadata.pixelHeight;
    }

    get pixelWidth() {
        return this.metadata.pixelWidth;
    }
    getSecondaryOffset() {
        if (!this.master) {
            console.error('Invalid OverheadProgressBar decoration');
            return null;
        }
        const masterSize = this.master.getSize();
        let offset = {
            x: 0,
            y: 0,
        };
        offset.y -= masterSize / 4 + this.getSize().h;
        return offset;
    }

    getSize() {
        if (animator.displaySettings.useActorHitboxWidth) {
            return {
                h: this.height,
                w: this.width,
            }
        } else {
            return {
                h: this.pixelHeight / animator.scale,
                w: this.pixelWidth / animator.scale,
            }
        }
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
        const ctx = animator.mainContext;
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

    get thickness() {
        return this.metadata.thickness;
    }

    get worldSizeThickness() {
        return this.metadata.worldSizeThickness;
    }

    getTargetPosition() {
        const time = animator.reactiveDataStatus.time;
        if (this.start > time || this.end < time) {
            return null;
        }
        const pos = this.targetPositionFetcher(this.connectedFrom, this.endMaster);
        if (!pos) {
            return null;
        }
        const offset = this.targetOffsetFetcher(this.connectedFrom);
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
        const ctx = animator.mainContext;
        ctx.save();
        if (this.growingReverse) {
            this.moveContext(ctx, target, 0);
            ctx.beginPath();
            ctx.moveTo(0, 0);
            ctx.lineTo(( 1 - percent) * (pos.x - target.x), percent * (pos.y - target.y));
            ctx.closePath();
        } else {
            this.moveContext(ctx, pos, 0);
            ctx.beginPath();
            ctx.moveTo(0, 0);
            ctx.lineTo(percent * (target.x - pos.x), percent * (target.y - pos.y));
            ctx.closePath();
        }
        let thickness = this.thickness;
        if (!this.worldSizeThickness) {
            thickness /= animator.scale;
        }
        ctx.lineWidth = (thickness).toString();
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
        super(params);
        this.positions = params.positions;
        if (this.positions.length > 1) {
            this.currentIndex = 0;
            this.currentStart = Number.NEGATIVE_INFINITY;
            this.currentEnd = this.positions[0][5];
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
        const ctx = animator.mainContext;
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
        
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        ctx.globalAlpha = this.getOpacity();
        const secondaryOffset = this.getSecondaryOffset();
        if (secondaryOffset) {        
            ctx.translate(secondaryOffset.x, secondaryOffset.y);
        }
        if(!this.canRotate) {
            // Don't rotate the icon
            ctx.rotate(-ToRadians(rot + this.rotationOffset));
        }
        const size = this.getSize();   
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
        for (let i = 0; i < totalPoints; i++) {
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
        for (let i = 0; i < totalPoints; i++) {
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

//

class TextDrawable extends MechanicDrawable {
    constructor(params) {
        super(params);
        this.text = params.text;
        const bold = !!params.bold;
        const fontSize = params.fontSize * resolutionMultiplier + "px";
        const fontType = params.fontType || "Comic Sans MS";
        this.font  = (bold ? "bold " : "") + fontSize + " " + fontType;
    }
    get color() {
        return this.metadata.color;
    }
    get backgroundColor() {
        return this.metadata.backgroundColor;
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
        
        const ctx = animator.mainContext;
        ctx.save();
        this.moveContext(ctx, pos, rot);
        const normalizedRot = Math.abs((ToRadians(rot + this.rotationOffset) / Math.PI) % 2);
        if (0.5 < normalizedRot && normalizedRot < 1.5) {
            // make sure the text remains upright
            ctx.rotate(-ToRadians(180));
        }
        ctx.font = this.font;
        ctx.fillStyle = this.color;
        ctx.textAlign = "center";
        ctx.fillText(this.text, 0, 0);
        ctx.restore();
    }
}
