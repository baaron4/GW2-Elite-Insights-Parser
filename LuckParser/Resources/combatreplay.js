// Players and boss
var mainActor = function(group, imgSrc) {
	this.group = group;
	this.pos = [];
	this.start = 0;
	this.dead = [];
	this.down = [];
	this.selected = false;
	this.img = new Image();
	this.img.src = imgSrc;
};

mainActor.prototype.died = function(timeToUse) {
	for (var i = 0; i < this.dead.length; i++) {
		if (!this.dead[i]) continue;
		if (this.dead[i][0] <= timeToUse && this.dead[i][1] >= timeToUse) {
			return true;
		}
	}
	return false;
};

mainActor.prototype.downed = function(timeToUse) {
	for (var i = 0; i < this.down.length; i++) {
		if (!this.down[i]) continue;
		if (this.down[i][0] <= timeToUse && this.down[i][1] >= timeToUse) {
			return true;
		}
	}
	return false;
};

mainActor.prototype.draw = function(ctx,timeToUse, pixelSize) {
	if (!this.pos.length) {
		return;
	}
	var halfSize = pixelSize / 2;
	var x = this.pos.length > 2 ? this.pos[2*timeToUse] : this.pos[0];
	var y = this.pos.length > 2 ? this.pos[2*timeToUse + 1] : this.pos[1];
	// the player is in the selected's player group
	if (!this.selected && this.group === selectedGroup) {
		ctx.beginPath();
		ctx.lineWidth='2';
		ctx.strokeStyle='blue';
		ctx.rect(x-halfSize,y-halfSize,pixelSize,pixelSize);
		ctx.stroke();
	} else if (this.selected){
		// this player is selected
		ctx.beginPath();
		ctx.lineWidth='4';
		ctx.strokeStyle='green';
		ctx.rect(x-halfSize,y-halfSize,pixelSize,pixelSize);
		ctx.stroke();
		var _this = this;
		// draw range markers
		rangeControl.forEach(function(enabled,radius,map) {
			if (!enabled) return;
			ctx.beginPath();
			ctx.lineWidth='2';
			ctx.strokeStyle='green';
			ctx.arc(x,y,inch * radius,0,2*Math.PI);
			ctx.stroke();
		});
	}
	if (this.died(timeToUse)) {
		ctx.drawImage(deadIcon,
			x-1.5*halfSize,
			y-1.5*halfSize,1.5*pixelSize,1.5*pixelSize);
	} else if (this.downed(timeToUse)) {
		ctx.drawImage(downIcon,
			x-1.5*halfSize,
			y-1.5*halfSize,1.5*pixelSize,1.5*pixelSize);
	} else {
		ctx.drawImage(this.img,
			x-halfSize,
			y-halfSize,pixelSize,pixelSize);
	}
};

// trash mobs
var secondaryActor = function(imgSrc, start, end) {
	this.pos = [];
	this.start = start;
	this.end = end;
	this.img = new Image();
	this.img.src = imgSrc;
};

secondaryActor.prototype.draw = function(ctx,timeToUse,pixelSize) {
	if (!(this.start > timeToUse || this.end < timeToUse) && this.pos.length) {
		var x = this.pos.length > 2 ? this.pos[2*(timeToUse - this.start)] : this.pos[0];
		var y = this.pos.length > 2 ? this.pos[2*(timeToUse - this.start) + 1] : this.pos[1];
		ctx.drawImage(this.img,
			x-pixelSize/2,y-pixelSize/2,
			pixelSize,pixelSize);
	}
};

// Circle actors
            var circleActor = function(radius,fill,growing, color, start, end) {
                    this.pos = null;
                    this.master = null;
                    this.start = start;
                    this.radius = radius;
                    this.end = end;
                    this.growing = growing;
                    this.fill = fill;
                    this.color = color;
                };
            circleActor.prototype.draw = function(ctx,timeToUse){
                    if (!(this.start > timeToUse || this.end < timeToUse)) {
                        var x,y;
                        if (this.pos instanceof Array) {
                            x = this.pos[0];
                            y = this.pos[1];
                        } else {
                            if (!this.master) {
                                var playerID = parseInt(this.pos);
                                this.master = data.has(playerID) ? data.get(playerID) : (secondaryData.has(this.pos) ? secondaryData.get(this.pos): boss);
                            }
                            var start = this.master.start ? this.master.start : 0;
                            x = this.master.pos.length > 2 ? this.master.pos[2*(timeToUse - start)] : this.master.pos[0];
                            y = this.master.pos.length > 2 ? this.master.pos[2*(timeToUse - start) + 1] : this.master.pos[1];
                        }
                        if (this.growing) {
                            var percent = Math.min((timeToUse - this.start)/(this.growing - this.start),1.0);
                            ctx.beginPath();
                            ctx.arc(x,y,percent*inch * this.radius,0,2*Math.PI);
                            if (this.fill) {
                                ctx.fillStyle=this.color;
                                ctx.fill();
                            } else {
                                ctx.lineWidth='2';
                                ctx.strokeStyle=this.color;
                                ctx.stroke();
                            }
                        } else {
                            ctx.beginPath();
                            ctx.arc(x,y,inch * this.radius,0,2*Math.PI);
                            if (this.fill) {
                                ctx.fillStyle=this.color;
                                ctx.fill();
                            } else {
                                ctx.lineWidth='2';
                                ctx.strokeStyle=this.color;
                                ctx.stroke();
                            }
                        }
                    }
};

// .... etc, move all other static methods here