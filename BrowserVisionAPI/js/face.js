var video = document.getElementById('video');
var canvas = document.getElementById('motion');
var score = document.getElementById('score');
var captureCanvas = document.getElementById('captureCanvas');
var captureCanvasContext = captureCanvas.getContext('2d');
var results = document.getElementById('results');
var record = document.getElementById("record");



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
    
    if (!triggered && record.checked === true) {
        if (payload.score > 30) {
            triggered = true;
            results.innerHTML = "";
            captureCanvasContext.drawImage(video, 0, 0, 1280, 720);
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
        console.log(this);
        if (this.readyState == 4 && this.status == 200) {
            // OK
            result = JSON.parse(this.responseText);
            console.log(results);
            displayFaceResults(result, results);
            triggered = false;
        }
        else if (this.status == 429 || this.status == 400) {
            // Rate Limit Exeeded wait 1000ms
            console.log("HAndle 429");
            setTimeout(function () { triggered = false; }, 1000);
            results.innerHTML = "Too many requests waiting 1 sec";
        }
    };
    try {
        xhttp.open("POST", "/api/face", true);
        xhttp.send(obj);
    } catch (err) {
        console.log("Handle 429");

        console.log("Exception " + err);
        // Rate Limit Exeeded wait 1000ms
        setTimeout(function () { triggered = false; }, 1000);
        dest.innerHTML = "Rate limit exeeded!! waiting";
    }
}
function displayFaceResults(results, dest) {
    console.log(results);
    face = results[0];
    var html = "";
    html += "FaceID : " + face.faceId + "<br/>";
    html += "Gender : " + face.faceAttributes.gender + "<br/>";
    html += "Age : " + face.faceAttributes.age + "<br/>";
    html += "Smile : " + face.faceAttributes.smile + "<br/>";

    html += "Facial Hair :" + "Moustache : " + face.faceAttributes.facialHair.moustache + "beard : " + face.faceAttributes.facialHair.beard + "sideburns : " + face.faceAttributes.facialHair.sideburns + "<br/>";

    html += "Glasses : " + face.faceAttributes.glasses + "<br/>";

    html += "Anger : " + face.faceAttributes.emotion.anger + "<br/>";
    html += "contempt : " + face.faceAttributes.emotion.contempt + "<br/>";
    html += "disgust : " + face.faceAttributes.emotion.disgust + "<br/>";
    html += "fear : " + face.faceAttributes.emotion.fear + "<br/>";
    html += "happiness : " + face.faceAttributes.emotion.happiness + "<br/>";
    html += "neutral : " + face.faceAttributes.emotion.neutral + "<br/>";
    html += "sadness : " + face.faceAttributes.emotion.sadness + "<br/>";

    html += "eyeMakeup : " + face.faceAttributes.makeup.eyeMakeup + "<br/>";
    html += "lipMakeup : " + face.faceAttributes.makeup.lipMakeup + "<br/>";
    html += "Hair Bald : " + face.faceAttributes.hair.bald + "<br/>";
    html += "Hair Hidden : " + face.faceAttributes.hair.invisible + "<br/>";



    ctx = captureCanvas.getContext("2d");    
    //------ Draw Rect

    ctx.beginPath();
    ctx.lineWidth = "3";
    ctx.strokeStyle = "red";

    ctx.rect(face.faceRectangle.top + face.faceRectangle.height / 2, face.faceRectangle.left - face.faceRectangle.width/2, face.faceRectangle.width, face.faceRectangle.height);
    ctx.stroke();
//----- Draw Eyes
    if (face.faceLandmarks) {
        //Left Pupil
        ctx.beginPath(); 
        ctx.arc(face.faceLandmarks.pupilLeft.x, face.faceLandmarks.pupilLeft.y, 10, 0, 2 * Math.PI);
        ctx.stroke();

        //Right Pupil
        ctx.beginPath(); 
        ctx.arc(face.faceLandmarks.pupilRight.x, face.faceLandmarks.pupilRight.y, 10, 0, 2 * Math.PI);
        ctx.stroke();

        //noseTip
        ctx.beginPath(); 
        ctx.arc(face.faceLandmarks.noseTip.x, face.faceLandmarks.noseTip.y, 10, 0, 2 * Math.PI);
        ctx.stroke();

        //mouthLeft
        ctx.beginPath(); 
        ctx.arc(face.faceLandmarks.mouthLeft.x, face.faceLandmarks.mouthLeft.y, 10, 0, 2 * Math.PI);
        ctx.stroke();

        //mouthRight
        ctx.beginPath(); 
        ctx.arc(face.faceLandmarks.mouthRight.x, face.faceLandmarks.mouthRight.y, 10, 0, 2 * Math.PI);
        ctx.stroke();




    }


    dest.innerHTML = html;
}