function setCookie(name, value) {
    var expires = new Date();
    expires.setTime(expires.getTime() + (1 * 60 * 1000));
    document.cookie = name + "=" + value + ";expires=" + expires.toUTCString() + ";path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var cookies = document.cookie.split(';');
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1, cookie.length);
        }
        if (cookie.indexOf(nameEQ) === 0) {
            return cookie.substring(nameEQ.length, cookie.length);
        }
    }
    return null;
}

if (!getCookie("clicked")) {
    setCookie("clicked", false);
}

let clicked = getCookie("clicked");

if (clicked !== "false") {
    document.getElementById("tempUser").checked = true;
    document.getElementById("Input_ExpiresOn").removeAttribute("disabled");

    document.querySelector(`span[data-valmsg-for="Input.ExpiresOn"]`).style.display = "initial";
}
else {
    document.getElementById("Input_ExpiresOn").setAttribute("disabled", " ");
    document.querySelector(`span[data-valmsg-for="Input.ExpiresOn"]`).style.display = "none";
}

function togle() {
    if (clicked === "false") {
        clicked = "true";
        setCookie("clicked", true);
        document.getElementById("Input_ExpiresOn").removeAttribute("disabled");
        document.querySelector(`span[data-valmsg-for="Input.ExpiresOn"]`).style.display = "initial";


    }
    else {
        clicked = "false";
        setCookie("clicked", false);
        document.getElementById("Input_ExpiresOn").setAttribute("disabled", " ");

        document.querySelector(`span[data-valmsg-for="Input.ExpiresOn"]`).style.display = "none";


    }

}
