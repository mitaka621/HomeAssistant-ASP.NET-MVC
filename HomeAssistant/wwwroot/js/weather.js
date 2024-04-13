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
        document.querySelector(".temp").innerHTML = Math.round(json.main.temp);
        document.querySelector(".weather-icon").src = `https://openweathermap.org/img/wn/${json.weather[0].icon}@2x.png`
        document.querySelector(".location").innerHTML = json.name;
        document.querySelector(".desc").innerHTML = json.weather[0].main;

        let hatSvg = document.querySelector(".hat-svg");
        let hatDesc = document.querySelector(".hat-desc");
        let shirtSvg = document.querySelector(".shirt-svg");
        let shirtDesc = document.querySelector(".shirt-desc");
        let umbrella = document.querySelector(".umbrella-svg");
        let umbrellaDesc = document.querySelector(".umbrella-desc");

        if (json.weather[0].id<=531) {
            document.querySelector("div.outfit-container > .right-side").style.display = "flex";
            umbrella.src = "/svg/umbrella-opened.svg";
            umbrellaDesc.innerHTML = "Bring an umbrella";
        }

        if (json.main.temp < 0) {
            hatSvg.src = "/svg/winter-hat.svg";
            shirtSvg.src = "/svg/heavy-jacket.svg";
            hatDesc.innerHTML = "Winter Hat"
            shirtDesc.innerHTML = "Heavy Jacket"
        }
        else if (json.main.temp < 10) {
            hatSvg.src = "/svg/winter-hat-light.svg";
            shirtSvg.src = "/svg/jacket.svg";
            hatDesc.innerHTML = "Light Winter Hat"
            shirtDesc.innerHTML = "Jacket"
        }
        else if (json.main.temp < 20) {
            hatSvg.src = "/svg/winter-hat-light.svg";
            shirtSvg.src = "/svg/Sweater.svg";
            hatDesc.innerHTML = "Light Winter Hat"
            shirtDesc.innerHTML = "Sweater"
        }
        else if (json.main.temp < 25) {
            hatSvg.src = "/svg/cap.svg";
            shirtSvg.src = "/svg/light-sweater.svg";
            hatDesc.innerHTML = "Cap"
            shirtDesc.innerHTML = "Long Sleeved T-Shirt"
        }
        else if (json.main.temp < 30) {
            hatSvg.src = "/svg/cap.svg";
            shirtSvg.src = "/svg/t-shirt.svg";
            hatDesc.innerHTML = "Cap"
            shirtDesc.innerHTML = "T-Shirt"
        }
        else {
            hatSvg.src = "";
            shirtSvg.src = "/svg/tank-top.svg";
            hatDesc.innerHTML = ""
            shirtDesc.innerHTML = "Tank-Top"
        }
    });
    