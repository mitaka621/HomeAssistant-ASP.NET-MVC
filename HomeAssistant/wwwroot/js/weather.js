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
    });
    

fetch('/api/Weather/UserLocation')
    .then(r => r.json())
    .then(json => console.log(json));
