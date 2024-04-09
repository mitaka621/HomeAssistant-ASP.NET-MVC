let timer = document.querySelector("h1.timer");

var x = document.getElementById("myAudio");
x.loop = true;
var promise = x.play();

if (promise !== undefined) {
    document.getElementById("myAudio").pause();
    promise.catch(function (error) {
        alert('Please enable audio autoplay to hear the timer go off.');
    });
} else {
    document.getElementById("myAudio").pause();
}

let min, sec;

if (timer !== null) {
    let arr = timer.textContent.split(":");
    min = parseInt(arr[0]);
    sec = parseInt(arr[1]);

    Timer(min, sec);

    let btn = document.querySelector("button.done");
    btn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M464 256A208 208 0 1 1 48 256a208 208 0 1 1 416 0zM0 256a256 256 0 1 0 512 0A256 256 0 1 0 0 256zM294.6 135.1c-4.2-4.5-10.1-7.1-16.3-7.1C266 128 256 138 256 150.3V208H160c-17.7 0-32 14.3-32 32v32c0 17.7 14.3 32 32 32h96v57.7c0 12.3 10 22.3 22.3 22.3c6.2 0 12.1-2.6 16.3-7.1l99.9-107.1c3.5-3.8 5.5-8.7 5.5-13.8s-2-10.1-5.5-13.8L294.6 135.1z"/></svg>`;
    btn.querySelector("svg").style.fill = "red";

}

function Timer(min, sec) {

    if (min === 0 && sec === 0) {
      
        x.play();


        let btn2 = document.querySelector("button.done");
        btn2.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M256 512A256 256 0 1 0 256 0a256 256 0 1 0 0 512zM369 209L241 337c-9.4 9.4-24.6 9.4-33.9 0l-64-64c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0l47 47L335 175c9.4-9.4 24.6-9.4 33.9 0s9.4 24.6 0 33.9z" /></svg>`;  
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

