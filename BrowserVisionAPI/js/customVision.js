// Grab elements, create settings, etc.
var video = document.getElementById('video');
var context = document.getElementById('myCanvas').getContext('2d');
var motionDetectContext = document.getElementById('motionDetectCanvas').getContext('2d');
var motionDetectCanvas = document.getElementById('motionDetectCanvas');
var canvas = document.getElementById('myCanvas');

// source : https://davidwalsh.name/browser-camera


// Get access to the camera!
if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
    // Not adding `{ audio: true }` since we only want video now
    navigator.mediaDevices.getUserMedia({
        video: true
    }).then(function (stream) {
        video.src = window.URL.createObjectURL(stream);
        video.play();
    });
} else if (navigator.getUserMedia) { // Standard
    navigator.getUserMedia({ video: true }, function (stream) {
        video.src = stream;
        video.play();
    }, errBack);
} else if (navigator.webkitGetUserMedia) { // WebKit-prefixed
    navigator.webkitGetUserMedia({ video: true }, function (stream) {
        video.src = window.webkitURL.createObjectURL(stream);
        video.play();
    }, errBack);
} else if (navigator.mozGetUserMedia) { // Mozilla-prefixed
    navigator.mozGetUserMedia({ video: true }, function (stream) {
        video.src = window.URL.createObjectURL(stream);
        video.play();
    }, errBack);
}

function predictImage(obj) {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            // OK
            results = JSON.parse(this.responseText);
            console.log(results);
            displayResults(results, document.getElementById("results"));
        }
    };
    xhttp.open("POST", "upload.aspx", true);
    xhttp.send(obj);
}
function displaypredictionResults(results, dest) {
    var html = "";
    for (var i = 0; i < results.Predictions.length; i++) {
        if (results.Predictions[i].Probability > 0.05 && (results.Predictions[i].Tag.indexOf("PID") === 0)) {
            html = html + matchTagWithLink(results.Predictions[i].Tag, keywordLinks) + "&nbsp;" + Math.floor(results.Predictions[i].Probability * 100).toString() + "%<br/>"
        }
    }
    dest.innerHTML = html;
}

function trainImage(obj) {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            // OK
            results = JSON.parse(this.responseText);
            console.log(results);
            trainResults(results, document.getElementById("results"));
        }
    };
    xhttp.open("POST", "training.aspx", true);
    xhttp.send(obj);
}
function trainResults(results, dest) {

}
motionDetectContext.globalCompositeOperation = 'difference';
//motionDetectContext.drawImage(video, 0, 0, 160, 120);
var tempMD0;// = new ImageData();
var tempMD1; // = new ImageData();
//tempMD0=motionDetectContext.getImageData(0, 0, 160, 120);

//var tempMD_num = 0;
setInterval(capture, 300);

function capture() {
    // Clear
    //motionDetectContext.clearRect(0, 0, canvas.width, canvas.height);

    // Draw new image
    motionDetectContext.drawImage(video, 0, 0, 160, 120);
    // Save Canvas
    
    /*switch (tempMD_num) {
        case 0:
            tempMD1 = motionDetectContext.getImageData(0, 0, 160, 120);
            motionDetectContext.putImageData(tempMD0, 0, 0);
            tempMD_num = 1;
            break;
        case 1:
            tempMD0 = motionDetectContext.getImageData(0, 0, 160, 120);
            motionDetectContext.putImageData(tempMD1, 0, 0);
            tempMD_num = 0;
            break;
        default:
    }*/

    motionDetectCalc(motionDetectContext.getImageData(0, 0, 160, 120));

}
function motionDetectCalc(imageData) {
    var imageScore = 0;

    for (var i = 0; i < imageData.data.length; i += 4) {
        var r = imageData.data[i] / 3;
        var g = imageData.data[i + 1] / 3;
        var b = imageData.data[i + 2] / 3;
        var pixelScore = r + g + b;

        if (pixelScore >= 100) {
            imageScore++;
        }
    }
    document.getElementById("score").innerHTML = imageScore;
    if (imageScore >= 1000) {
        
        // we have motion!
    }
    // do other stuff

}