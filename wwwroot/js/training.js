var video = document.getElementById('video');
var canvas = document.getElementById('motion');
var score = document.getElementById('score');
var captureCanvas = document.getElementById('captureCanvas');
var captureCanvasContext = captureCanvas.getContext('2d');
var tagsInput = document.getElementById("tagsInput");
var record = document.getElementById("record");
var threshold = document.getElementById("threshold")


function initSuccess() {
    DiffCamEngine.start();
    console.log("ready");
    // to prevent early triggers in Edge
    setTimeout(function () { triggered = false; connections = 0; }, 500);
}

function initError() {
    alert('Something went wrong. Maybe you did not use https:\n\rOr your camera is busy in another window');
}
var triggered = true;
var connections = 1;
var macConnections = 1;

function capture(payload) {
    score.textContent = payload.score;

    if (connections < macConnections && tagsInput.value.indexOf(";") > 0 && record.checked === true) {
        if (payload.score > threshold.value) {
            triggered = true;
            connections++;
            captureCanvasContext.drawImage(video, 0, 0, 1280, 720);
            trainImage(captureCanvas.toDataURL("image/jpeg", 0.9));
            }
    }
}

DiffCamEngine.init({
	video: video,
	motionCanvas: canvas,
	initSuccessCallback: initSuccess,
	initErrorCallback: initError,
	captureCallback: capture
});

function displaypredictionResults(results, dest) {
    var html = "";
    console.log(results);
    for (var i = 0; i < results.Predictions.length; i++) {
        if (results.Predictions[i].Probability > 0.05 && results.Predictions[i].Tag.indexOf("PID") === 0) {
            html = html + matchTagWithLink(results.Predictions[i].Tag, keywordLinks) + "&nbsp;" + Math.floor(results.Predictions[i].Probability * 100).toString() + "%<br/>";
        }
    }
    dest.innerHTML = html;
}

function trainImage(obj) {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            // OK
            //results = JSON.parse(this.responseText);
            console.log(this.responseText);
            //trainResults(results, document.getElementById("results"));
            triggered = false;
            connections--;
        }
    };
    xhttp.open("POST", "/api/training?tags=" + tagsInput.value, true);
    xhttp.send(obj);
}