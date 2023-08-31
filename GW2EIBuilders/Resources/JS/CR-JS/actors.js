/*jshint esversion: 6 */
/* jshint node: true */
/*jslint browser: true */
/*global animator, animateCanvas, noUpdateTime, deadIcon, dcIcon, downIcon*/
"use strict";
//// ACTORS
class IconDrawable {
    constructor(pos, angles, start, end, imgSrc, pixelSize, dead, down, dc, breakbarActive, hitboxWidth) {
        this.pos = pos;
        this.angles = angles;
        this.start = start;
        this.end = end;
        this.img = new Image();
        this.img.src = imgSrc;
        this.img.onload = function () {
            animateCanvas(noUpdateTime);
        };
        this.pixelSize = pixelSize;
        this.group = null;
        this.dead = typeof dead !== "undefined" ? dead : null;
        this.down = typeof down !== "undefined" ? down : null;
        this.dc = typeof dc !== "undefined" ? dc : null;
        this.breakbarActive = typeof breakbarActive !== "undefined" ? breakbarActive : null;
        this.hitboxWidth = hitboxWidth;
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
        var time = animator.reactiveDataStatus.time;
        for (let i = 0; i < this.dead.length; i += 2) {
            if (this.dead[i] <= time && this.dead[i + 1] >= time) {
                return true;
            }
        }
        return false;
    }

    downed() {
        if (this.down === null || this.down.length === 0) {
            return false;
        }
        var time = animator.reactiveDataStatus.time;
        for (let i = 0; i < this.down.length; i += 2) {
            if (this.down[i] <= time && this.down[i + 1] >= time) {
                return true;
            }
        }
        return false;
    }

    disconnected() {
        if (this.dc === null || this.dc.length === 0) {
            return false;
        }
        var time = animator.reactiveDataStatus.time;
        for (let i = 0; i < this.dc.length; i += 2) {
            if (this.dc[i] <= time && this.dc[i + 1] >= time) {
                return true;
            }
        }
        return false;
    }

    isBreakbarActive() {
        if (this.breakbarActive === null || this.breakbarActive.length === 0) {
            return false;
        }
        var time = animator.reactiveDataStatus.time;
        for (let i = 0; i < this.breakbarActive.length; i += 2) {
            if (this.breakbarActive[i] <= time && this.breakbarActive[i + 1] >= time) {
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
        const positionX = this.pos[2 * offsetedIndex];
        const positionY = this.pos[2 * offsetedIndex + 1];
        const timeValue = animator.times[currentIndex];
        var pt = {
            x: 0,
            y: 0
        };
        var time = animator.reactiveDataStatus.time;
        if (time - timeValue > 0 && offsetedIndex < 0.5 * this.pos.length - 1) {
            const nextTimeValue = animator.times[currentIndex + 1];
            const nextPositionX = this.pos[2 * offsetedIndex + 2];
            const nextPositionY = this.pos[2 * offsetedIndex + 3];
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

    getPosition() {
        if (this.pos === null || this.pos.length === 0 || this.disconnected()) {
            return null;
        }
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start > time || this.end < time)) {
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
        const currentIndex = Math.floor((animator.times.length - 1) * time / lastTime);
        return this.getInterpolatedPosition(startIndex, Math.max(currentIndex, startIndex));
    }

    getSize() {
        if (animator.displaySettings.useActorHitboxWidth && this.hitboxWidth > 0) {
            return this.hitboxWidth;
        } else {
            return this.pixelSize / animator.scale;
        }
    }

    draw() {
        if (!this.canDraw()) {
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
                ctx.arc(pos.x, pos.y, animator.inchToPixel * element.radius, 0, 2 * Math.PI);
                ctx.stroke();
            });
        }
        ctx.drawImage(this.getIcon(),
            pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
    }

}

class SquadIconDrawable extends IconDrawable {
    constructor(start, end, imgSrc, pixelSize, group, pos, angles, dead, down, dc, breakbarActive, hitboxWidth) {
        super(pos, angles, start, end, imgSrc, pixelSize, dead, down, dc, breakbarActive, hitboxWidth);
        this.group = group;
    }

    inSelectedGroup() {
        return animator.selectedActor !== null && animator.selectedActor.group === this.group;
    }

}

class NonSquadIconDrawable extends IconDrawable {
    constructor(start, end, imgSrc, pixelSize, pos, angles, dead, down, dc, breakbarActive, masterID, hitboxWidth) {
        super(pos, angles, start, end, imgSrc, pixelSize, dead, down, dc, breakbarActive, hitboxWidth);
        this.masterID = typeof masterID === "undefined" ? -1 : masterID;
        this.master = null;
    }

    canDraw() {
        if (this.master === null) {
            this.master = animator.getActorData(this.masterID);
        }
        if (this.master && !animator.displaySettings.displayAllMinions) {
            return this.master.isSelected() && animator.displaySettings.displaySelectedMinions;
        }
        return true;
    }
}