async function fetchTelemetryData() {

    await fetch('/api/HomeTelemetry/data')
        .then(r =>r.json())
        .then(json => {
            if (json.status == 501) {
                return;
            }
            document.querySelector("div.telemetry div.status").innerHTML = `<svg class="ok" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M256 48a208 208 0 1 1 0 416 208 208 0 1 1 0-416zm0 464A256 256 0 1 0 256 0a256 256 0 1 0 0 512zM369 209c9.4-9.4 9.4-24.6 0-33.9s-24.6-9.4-33.9 0l-111 111-47-47c-9.4-9.4-24.6-9.4-33.9 0s-9.4 24.6 0 33.9l64 64c9.4 9.4 24.6 9.4 33.9 0L369 209z"/></svg>`;

            document.getElementById("house-temp").textContent = json.Temperature;
            document.getElementById("house-humidity").textContent = json.Humidity;
            document.getElementById("house-rad").textContent = json.Radiation;
            document.getElementById("house-cpm").textContent = json.CPM;

            if (document.querySelector(".info")) {
                document.querySelector(".info").remove();
            }
           
        });
}

fetchTelemetryData();

setInterval(fetchTelemetryData, 5 * 60 * 1000);