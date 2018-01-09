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
            console.log(result);

                displaypredictionResults(result, results);
                triggered = false;
         }
        else /*if (this.status === 429)*/ {
            // Rate Limit Exeeded wait 1000ms
            setTimeout(function () { triggered = false; }, 1000);
            console.log("Rate limit exceeded");
            //results.innerHTML = "Rate limit exeeded!!";
        }
    };

        xhttp.open("POST", "/api/prediction", true);
        xhttp.send(obj);
    
}
function displaypredictionResults(result, dest) {
    var html = "";
    if (result.Predictions) {

    
    for (var i = 0; i < result.Predictions.length; i++) {
        if (result.Predictions[i].Probability > 0.25) {
            html = html + result.Predictions[i].Tag + "&nbsp;" + Math.floor(result.Predictions[i].Probability * 100).toString() + "%<br/>";
        }
    }
    results.innerHTML = html;
    }
    else {
        setTimeout(function () { triggered = false; }, 1000);
        console.log("Undefined response");
    }
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