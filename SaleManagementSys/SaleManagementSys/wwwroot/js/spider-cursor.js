(function () {
    'use strict';
    var canvas = document.getElementById('spiderCanvas');
    if (!canvas) return;

    canvas.setAttribute('aria-hidden', 'true');
    canvas.style.pointerEvents = 'none';

    var ctx = canvas.getContext('2d');
    var mouse = { x: 0, y: 0 };
    var legs = [];
    var numLegs = 8;
    var legSegments = 5;
    var stars = [];
    var starCount = 120;

    function resize() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        initStars();
    }

    function initStars() {
        stars = [];
        for (var i = 0; i < starCount; i++) {
            stars.push({
                x: Math.random() * canvas.width,
                y: Math.random() * canvas.height,
                r: Math.random() * 1.2 + 0.3,
                twinkle: Math.random() * Math.PI * 2
            });
        }
    }

    function drawStars(time) {
        ctx.fillStyle = 'rgba(10, 10, 20, 0.15)';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        for (var i = 0; i < stars.length; i++) {
            var s = stars[i];
            var alpha = 0.4 + 0.6 * Math.sin(time * 0.002 + s.twinkle);
            ctx.beginPath();
            ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
            ctx.fillStyle = 'rgba(255, 255, 255, ' + alpha + ')';
            ctx.fill();
        }
    }

    function drawSpider() {
        var cx = mouse.x;
        var cy = mouse.y;
        var radius = 45;
        var spread = 0.4;

        ctx.strokeStyle = 'rgba(255, 255, 255, 0.7)';
        ctx.lineWidth = 1.2;
        ctx.lineCap = 'round';

        for (var leg = 0; leg < numLegs; leg++) {
            var angle = (leg / numLegs) * Math.PI * 2 + Date.now() * 0.0008;
            ctx.beginPath();
            ctx.moveTo(cx, cy);

            var prevX = cx;
            var prevY = cy;
            for (var seg = 1; seg <= legSegments; seg++) {
                var t = seg / legSegments;
                var r = radius * t + 15 * Math.sin(Date.now() * 0.003 + leg) * spread;
                var x = cx + Math.cos(angle + seg * 0.15) * r * (0.9 + 0.2 * Math.sin(Date.now() * 0.002));
                var y = cy + Math.sin(angle + seg * 0.15) * r * (0.9 + 0.2 * Math.sin(Date.now() * 0.002));
                ctx.lineTo(x, y);
                prevX = x;
                prevY = y;
            }
            ctx.stroke();
        }

        ctx.beginPath();
        ctx.arc(cx, cy, 6, 0, Math.PI * 2);
        ctx.fillStyle = 'rgba(255, 255, 255, 0.9)';
        ctx.fill();
        ctx.strokeStyle = 'rgba(255, 255, 255, 0.6)';
        ctx.lineWidth = 1;
        ctx.stroke();
    }

    function animate(time) {
        if (!canvas.width || !canvas.height) {
            requestAnimationFrame(animate);
            return;
        }
        drawStars(time || 0);
        drawSpider();
        requestAnimationFrame(animate);
    }

    function onMouseMove(e) {
        mouse.x = e.clientX;
        mouse.y = e.clientY;
    }

    window.addEventListener('resize', resize);
    document.addEventListener('mousemove', onMouseMove);
    resize();
    animate(0);
})();
