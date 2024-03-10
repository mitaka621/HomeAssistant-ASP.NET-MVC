//getting the user's geolocation, otherwise using default location (Sofia) or last known location if availible
if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(
        function (position) {

            const formData = {
                Latitude: position.coords.latitude,
                Longitude: position.coords.longitude
            };

            fetch('/api/Weather/UserLocation', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData)
            });
        },
    );
} 

fetch('/api/Weather/Weather')
    .then(r => r.json())
    .then(json => {
        console.log(json);
        document.querySelector(".temp").innerHTML = Math.round(json.main.temp);
        document.querySelector(".weather-icon").src = `https://openweathermap.org/img/wn/${json.weather[0].icon}@2x.png`
        document.querySelector(".location").innerHTML = json.name;
        document.querySelector(".desc").innerHTML = json.weather[0].main;

        let hatSvg = document.querySelector(".hat-svg");
        let shirtSvg = document.querySelector(".shirt-svg");
        let umbrella = document.querySelector(".umbrella-svg");

        if (json.main.temp < 0) {
            hatSvg.src = "/svg/winter-hat.svg";
            shirtSvg.src = "/svg/heavy-jacket.svg";
        }
        else if (json.main.temp < 10) {
            hatSvg.src = "/svg/winter-hat-light.svg";
            shirtSvg.src = "/svg/jacket.svg";
        }
        else if (json.main.temp < 20) {
            hatSvg.src = "/svg/winter-hat-light.svg";
            shirtSvg.src = "/svg/Sweater.svg";
        }
        else if (json.main.temp < 25) {
            hatSvg.src = "/svg/cap.svg";
            shirtSvg.src = "/svg/shirt.svg";
        }
        else if (json.main.temp < 30) {
            hatSvg.src = "~/svg/cap.svg";
            shirtSvg.src = "/svg/t-shirt.svg";
        }
        else {
            hatSvg.src = "";
            shirtSvg.src = "/svg/tank-top.svg";
        }
    });
    

fetch('/api/Weather/UserLocation')
    .then(r => r.json())
    .then(json => console.log(json));
