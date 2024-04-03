let timer = document.querySelector("h1.timer");

var x = document.getElementById("myAudio");
x.loop = true;
var promise = x.play();

if (promise !== undefined) {
    promise.catch(function (error) {
        alert('Please enable audio autoplay to hear the timer go off.');
    });
} else {
    x.pause();
}

let min, sec;

if (timer !== null) {
    let arr = timer.textContent.split(":");
    min = parseInt(arr[0]);
    sec = parseInt(arr[1]);

    Timer(min, sec);

    let btn = document.querySelector("button.btn-success");
    btn.classList.remove("btn-success")
    btn.classList += " btn-danger";
    btn.textContent = "Skip Timer";
}

function Timer(min, sec) {

    if (min === 0 && sec === 0) {
      
        x.play();

        let btn = document.querySelector("button.btn-danger");
        btn.classList.remove("btn-danger")
        btn.classList += " btn-success";
        btn.textContent = "Next Step";

        return;
    }

    if (sec === 0) {
        min--;
        sec = 59;
    }
    else {
        sec--;
    }

    timer.innerHTML = `${min.toLocaleString('en-US', { minimumIntegerDigits: 2, useGrouping: false })}:${sec.toLocaleString('en-US', { minimumIntegerDigits: 2, useGrouping: false })}`;

    setTimeout(function () {
        Timer(min, sec);
    }, 1000);
}

