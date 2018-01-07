var video = document.getElementById('video');
var canvas = document.getElementById('motion');
var score = document.getElementById('score');
var captureCanvas = document.getElementById('captureCanvas');
var captureCanvasContext = captureCanvas.getContext('2d');
var results = document.getElementById('results');
function initSuccess() {
    DiffCamEngine.start();
    console.log("ready");
    // to prevent early triggers in Edge
    setTimeout(function () { triggered = false; }, 500);
}

function initError() {
    alert('Something went wrong. Maybe you did not use https:\n\rOr your camera is busy in another window');
}
var triggered = true;


function capture(payload) {
    score.textContent = payload.score;
    
    if (!triggered) {
        if (payload.score > 30) {
            triggered = true;
            results.innerHTML = "";
            captureCanvasContext.drawImage(video, 0, 0, 1280, 720);
            //Wait 0.5 seconds before doing capture
            //setTimeout(function () { predictImage(captureCanvas.toDataURL("image/jpeg", 0.9) )}, 500);
            predictImage(captureCanvas.toDataURL("image/jpeg", 0.7));
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


//--------------------------------
function predictImage(obj) {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            // OK
            result = JSON.parse(this.responseText);
            console.log(results);

                displaypredictionResults(result, results);
                triggered = false;
         }
        else if (this.status === 429) {
            // Rate Limit Exeeded wait 1000ms
            setTimeout(function () { triggered = false; }, 1000);
            results.innerHTML = "Rate limit exeeded!!";
        }
    };

        xhttp.open("POST", "/api/prediction", true);
        xhttp.send(obj);
        console.log("Exception : " + err.toString());
        // Rate Limit Exeeded wait 1000ms
        setTimeout(function () { triggered = false; }, 1000);
        dest.innerHTML = "Rate limit exeeded!! waiting";
    
}
function displaypredictionResults(results, dest) {
    var html = "";
    console.log(results);
    for (var i = 0; i < results.Predictions.length; i++) {
        if (results.Predictions[i].Probability > 0.05/* && (results.Predictions[i].Tag.indexOf("PID") === 0) */) {
            html = html + results.Predictions[i].Tag + "&nbsp;" + Math.floor(results.Predictions[i].Probability * 100).toString() + "%<br/>";
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
        }
    };
    xhttp.open("POST", "training.aspx?tags=" + document.getElementById("tagsInput").value, true);
    xhttp.send(obj);
}