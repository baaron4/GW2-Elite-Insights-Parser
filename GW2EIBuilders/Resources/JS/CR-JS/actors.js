/*jshint esversion: 6 */
/* jshint node: true */
/*jslint browser: true */
/*global animator, animateCanvas, noUpdateTime, deadIcon, dcIcon, downAllyIcon, downEnemyIcon*/
"use strict";
//// ACTORS

function IsPresentInArray(array) {
    var time = animator.reactiveDataStatus.time;
    for (let i = 0; i < array.length; i += 2) {
        if (array[i] <= time && array[i + 1] >= time) {
            return true;
        }
    }
    return false;
}

class IconDrawable {
    constructor(params, pixelSize) {
        this.positions = params.positions;
        this.angles = params.angles;
        this.start = params.start;
        this.end = params.end;
        this.img = new Image();
        this.img.src = params.img;
        this.img.onload = function () {
            animateCanvas(noUpdateTime);
        };
        this.pixelSize = pixelSize;
        this.group = null;
        this.dead = typeof params.dead !== "undefined" ? params.dead : null;
        this.down = typeof params.down !== "undefined" ? params.down : null;
        this.dc = typeof params.dc !== "undefined" ? params.dc : null;
        this.hide = typeof params.hide !== "undefined" ? params.hide : null;
        this.breakbarActive = typeof params.breakbarActive !== "undefined" ? params.breakbarActive : null;
        this.hitboxWidth = InchToPixel * params.hitboxWidth;
        //
        this.id = params.id;
        this.parentID = params.parentID;
        uint32[0] = params.id;
        this.pickingColor = params.img && params.img.length > 0 ? `rgba(${uint32ToUint8[0]}, ${uint32ToUint8[1]}, ${uint32ToUint8[2]}, 1)` : null;
    }

    isSelected() {
        return animator.selectedActor === this;
    }

    inSelectedGroup() {
        return false;
    }

    died() {
        if (this.dead === null || this.dead.length === 0) {
            return false;
        }
        return IsPresentInArray(this.dead);
    }

    downed() {
        if (this.down === null || this.down.length === 0) {
            return false;
        }
        return IsPresentInArray(this.down);
    }

    disconnected() {
        if (this.dc === null || this.dc.length === 0) {
            return false;
        }
        return IsPresentInArray(this.dc);
    }

    isBreakbarActive() {
        if (this.breakbarActive === null || this.breakbarActive.length === 0) {
            return false;
        }
        return IsPresentInArray(this.breakbarActive);
    }

    _isFriendly() {
        return animator.playerData.has(this.id) || animator.friendlyMobData.has(this.id) || animator.friendlyPlayerData.has(this.id);
    }

    getIcon() {
        if (this.died()) {
            return deadIcon;
        }
        if (this.downed()) {
            return this._isFriendly() ? downAllyIcon : downEnemyIcon;
        }
        if (this.disconnected()) {
            return dcIcon;
        }
        return this.img;
    }

    getInterpolatedRotation(startIndex, currentIndex) {
        const offsetedIndex = currentIndex - startIndex;
        const initialAngle = this.angles[offsetedIndex];
        const timeValue = animator.times[currentIndex];
        var angle = 0;
        var time = animator.reactiveDataStatus.time;
        if (time - timeValue > 0 && offsetedIndex < this.angles.length - 1) {
            const nextTimeValue = animator.times[currentIndex + 1];
            let nextAngle = this.angles[offsetedIndex + 1];
            // Make sure the interpolation is only done on the shortest path to avoid big flips around PI or -PI radians
            if (nextAngle - initialAngle < -180) {
                nextAngle += 360.0;
            } else if (nextAngle - initialAngle > 180) {
                nextAngle -= 360.0;
            }
            angle = initialAngle + (time - timeValue) / (nextTimeValue - timeValue) * (nextAngle - initialAngle);
        } else {
            angle = initialAngle;
        }
        return angle;
    }

    getInterpolatedPosition(startIndex, currentIndex) {
        const offsetedIndex = currentIndex - startIndex;
        const positionX = this.positions[2 * offsetedIndex];
        const positionY = this.positions[2 * offsetedIndex + 1];
        const timeValue = animator.times[currentIndex];
        var pt = {
            x: 0,
            y: 0
        };
        var time = animator.reactiveDataStatus.time;
        if (time - timeValue > 0 && offsetedIndex < 0.5 * this.positions.length - 1) {
            const nextTimeValue = animator.times[currentIndex + 1];
            const nextPositionX = this.positions[2 * offsetedIndex + 2];
            const nextPositionY = this.positions[2 * offsetedIndex + 3];
            pt.x = positionX + (time - timeValue) / (nextTimeValue - timeValue) * (nextPositionX - positionX);
            pt.y = positionY + (time - timeValue) / (nextTimeValue - timeValue) * (nextPositionY - positionY);
        } else {
            pt.x = positionX;
            pt.y = positionY;
        }
        pt.x = Math.round(10 * pt.x * animator.scale) / (10 * animator.scale);
        pt.y = Math.round(10 * pt.y * animator.scale) / (10 * animator.scale);
        return pt;
    }

    canDraw() {
        if (this.hide && this.hide.length > 0 && IsPresentInArray(this.hide)) {        
            return false;
        }
        return true;
    }

    getRotation() {
        if (this.angles === null || this.angles.length === 0 || this.disconnected()) {
            return null;
        }
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start > time || this.end < time)) {
            return null;
        }
        if (this.angles.length === 1) {
            return this.angles[0];
        }
        const lastTime = animator.times[animator.times.length - 1];
        const startIndex = Math.ceil((animator.times.length - 1) * Math.max(this.start, 0) / lastTime);
        const currentIndex = Math.floor((animator.times.length - 1) * time / lastTime);
        return this.getInterpolatedRotation(startIndex, Math.max(currentIndex, startIndex));
    }

    _getPosition(time) {
        if (this.positions === null || this.positions.length === 0 || this.disconnected()) {
            return null;
        }
        if (this.start !== -1 && (this.start > time || this.end < time)) {
            return null;
        }
        if (this.positions.length === 2) {
            return {
                x: this.positions[0],
                y: this.positions[1]
            };
        }
        const lastTime = animator.times[animator.times.length - 1];
        const startIndex = Math.ceil((animator.times.length - 1) * Math.max(this.start, 0) / lastTime);
        const currentIndex = Math.floor((animator.times.length - 1) * time / lastTime);
        return this.getInterpolatedPosition(startIndex, Math.max(currentIndex, startIndex));
    }

    getPosition(_time) {
        return this._getPosition(animator.reactiveDataStatus.time);
    }

    getSize() {
        if (animator.displaySettings.useActorHitboxWidth && this.hitboxWidth > 0) {
            return this.hitboxWidth;
        } else {
            return this.pixelSize / animator.scale;
        }
    }

    draw() {
        if (!this.canDraw() || this.pickingColor == null) {
            return;
        }
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.mainContext;
        const fullSize = this.getSize();
        const halfSize = fullSize / 2;
        var isSelected = this.isSelected();
        var inSelectedGroup = this.inSelectedGroup();
        if (animator.displaySettings.highlightSelectedGroup && !isSelected && inSelectedGroup) {
            ctx.beginPath();
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = 'blue';
            ctx.rect(pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
            ctx.stroke();
        } else if (isSelected) {
            ctx.beginPath();
            ctx.lineWidth = (4 / animator.scale).toString();
            ctx.strokeStyle = 'green';
            ctx.rect(pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
            ctx.stroke();
            animator.rangeControl.forEach(function (element) {
                if (!element.enabled) {
                    return;
                }
                ctx.beginPath();
                ctx.lineWidth = (2 / animator.scale).toString();
                ctx.strokeStyle = 'green';
                ctx.arc(pos.x, pos.y, InchToPixel * element.radius, 0, 2 * Math.PI);
                ctx.stroke();
            });
        }      
        ctx.drawImage(this.getIcon(), pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
    }

    drawPicking() {
        if (!this.canDraw() || !this.pickingColor) {
            return;
        }
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.pickContext;
        
        ctx.save();
        ctx.translate(pos.x, pos.y);
        const fullSize = this.getSize();
        const halfSize = fullSize / 2;
        ctx.beginPath();
        ctx.arc(0, 0, halfSize, 0, 2 * Math.PI);
        ctx.fillStyle = this.pickingColor;
        ctx.fill();
        ctx.restore();
    }
}

class PlayerIconDrawable extends IconDrawable {
    constructor(params, pixelSize) {
        super(params, pixelSize);
        this.group = params.group;
        this.img.crossOrigin = "Anonymous";
    }

    inSelectedGroup() {
        return animator.selectedActor !== null && animator.selectedActor.group === this.group;
    }

}

class NonSquadIconDrawable extends IconDrawable {
    constructor(params, pixelSize) {
        super(params, pixelSize);
        this.masterID = typeof params.masterID !== "undefined" && params.masterID >= 0 ? params.masterID : -1;
        this.master = null;
    }

    canDraw() {
        if (!super.canDraw()) {
            return false;
        }
        const perParentArray = animator.agentDataPerParentID.get(this.masterID);
        if (perParentArray) {     
            this.master = perParentArray.filter(x => x.getPosition() != null)[0];
        } else if (this.master === null) {
            this.master = animator.getActorData(this.masterID);
        }
        if (this.master && !animator.displaySettings.displayAllMinions) {
            return (this.master.isSelected() || this.isSelected()) && animator.displaySettings.displaySelectedMinions;
        }
        return true;
    }
}

class NPCIconDrawable extends NonSquadIconDrawable {
    constructor(params, pixelSize) {
        super(params, pixelSize);
    }
}

let adjustedEnemyDeadIcon = null;
let adjustedFriendlyDeadIcon = null;
let adjustedFriendlyDownIcon = null;

function adjustImageColor(image, colorAdjuster) {

    const imageWidth = image.width;
    const imageHeight = image.height;
    const offscreen = new OffscreenCanvas(imageWidth, imageHeight);
    const ctx = offscreen.getContext("2d");
    ctx.drawImage(image, 0, 0);
    const imageData = ctx.getImageData(0, 0, imageWidth, imageHeight);
    for (let i = 0; i < imageData.data.length; i += 4) {
        let color = colorAdjuster(imageData.data[i + 0], imageData.data[i + 1], imageData.data[i + 2], imageData.data[i + 3]);
        imageData.data[i + 0] = color.r;
        imageData.data[i + 1] = color.g;
        imageData.data[i + 2] = color.b;
        imageData.data[i + 3] = color.a;
    }
    ctx.putImageData(imageData, 0, 0);
    offscreen.complete = true;
    offscreen.naturalWidth = imageWidth;
    offscreen.naturalHeight = imageHeight;
    return offscreen;
}

class NonSquadPlayerDrawable extends NonSquadIconDrawable {  
    constructor(params, pixelSize) {
        super(params, pixelSize);
        this.img.crossOrigin = "Anonymous";
        this.adjustedImg = null;
        this.colorAdjuster = null;
    }

    _getDownIcon() {
        return null;
    }

    _getDeadIcon() {
        return null;
    }

    getIcon() {
        if (this.died()) {
            return this._getDeadIcon();
        }
        if (this.downed()) {   
            return this._getDownIcon();
        }
        if (this.disconnected()) {
            return dcIcon;
        }
        if (!this.adjustedImg && this.img.complete) {
            this.adjustedImg = adjustImageColor(this.img, this.colorAdjuster)
        }
        return this.adjustedImg;
    }
}

class EnemyPlayerDrawable extends NonSquadPlayerDrawable {
    constructor(params, pixelSize) {
        super(params, pixelSize);
        this.colorAdjuster = (r, g, b, a) => {
            let grayScale = 0.299 * r + 0.587 * g + 0.114*b;
            return {
                r: grayScale,
                g: 0.3 * grayScale,
                b: 0.3 * grayScale,
                a: a
            };
        }
    }
    _getDownIcon() {
        return downEnemyIcon;
    }

    _getDeadIcon() {
        if (!adjustedEnemyDeadIcon && deadIcon.complete) {
            adjustedEnemyDeadIcon = adjustImageColor(deadIcon, this.colorAdjuster);
        }
        return adjustedEnemyDeadIcon;
    }
}

class FriendlyPlayerDrawable extends NonSquadPlayerDrawable {
    constructor(params, pixelSize) {
        super(params, pixelSize);
        this.colorAdjuster = (r, g, b, a) => {
            let grayScale = 0.299 * r + 0.587 * g + 0.114*b;
            return {
                r: 0.3 * grayScale,
                g: grayScale,
                b: 0.3 * grayScale,
                a: a
            };
        }
    }
    _getDownIcon() {
        if (!adjustedFriendlyDownIcon && downAllyIcon.complete) {
            adjustedFriendlyDownIcon = adjustImageColor(downAllyIcon, this.colorAdjuster);
        }
        return adjustedFriendlyDownIcon;
    }
    _getDeadIcon() {
        if (!adjustedFriendlyDeadIcon && deadIcon.complete) {
            adjustedFriendlyDeadIcon = adjustImageColor(deadIcon, this.colorAdjuster);
        }
        return adjustedFriendlyDeadIcon;
    }
}