let state = 0;
var div = document.querySelector("div.container-animation");
var btn = document.querySelector(".svg-btn");

function TogleFridgeAnimation() {

    if (state === 1) {
        state = 0;

        div.style.maxHeight = "0px";
        btn.style.transform = 'rotate(180deg)';
    } else {
        state = 1;
        div.style.maxHeight = "300px";
        btn.style.transform = 'rotate(0deg)';
    }
}

function scrollToContainer(e) {
    e.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

if (document.getElementById("recipe-steps")) {
    scrollToContainer(document.getElementById("recipe-steps"));
}

if (document.getElementById("messageToSend")) {
    scrollToContainer(document.getElementById("messageToSend"));
}