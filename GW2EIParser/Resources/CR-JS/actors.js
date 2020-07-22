"use strict";
//// ACTORS
class IconDrawable {
    constructor(start, end, imgSrc, pixelSize) {
        this.pos = null;
        this.start = start;
        this.end = end;
        this.img = new Image();
        this.img.src = imgSrc;
        this.img.onload = function () {
            animateCanvas(noUpdateTime);
        };
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

    getPosition() {
        if (this.pos === null || this.pos.length === 0) {
            return null;
        }
        var time = animator.reactiveDataStatus.time;
        if (this.start !== -1 && (this.start >= time || this.end <= time)) {
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

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        const fullSize = this.pixelSize / animator.scale;
        const halfSize = fullSize / 2;
        animator.mainContext.drawImage(this.img,
            pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
    }

}

class PlayerIconDrawable extends IconDrawable {
    constructor(imgSrc, pixelSize, group, pos, dead, down, dc) {
        super(-1, -1, imgSrc, pixelSize);
        this.pos = pos;
        this.dead = dead;
        this.down = down;
        this.dc = dc;
        this.selected = false;
        this.group = group;
    }

    draw() {
        const pos = this.getPosition();
        if (pos === null) {
            return;
        }
        var ctx = animator.mainContext;
        const fullSize = this.pixelSize / animator.scale;
        const halfSize = fullSize / 2;
        if (animator.highlightSelectedGroup && !this.selected && this.group === animator.selectedGroup) {
            ctx.beginPath();
            ctx.lineWidth = (2 / animator.scale).toString();
            ctx.strokeStyle = 'blue';
            ctx.rect(pos.x - halfSize, pos.y - halfSize, fullSize, fullSize);
            ctx.stroke();
        } else if (this.selected) {
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
                ctx.arc(pos.x, pos.y, animator.inch * element.radius, 0, 2 * Math.PI);
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

}

class EnemyIconDrawable extends IconDrawable {
    constructor(start, end, imgSrc, pixelSize, pos) {
        super(start, end, imgSrc, pixelSize);
        this.pos = pos;
    }
}