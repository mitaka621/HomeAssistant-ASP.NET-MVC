function GetIpDetails(e) {
	fetch(`/api/userconfiguration/GetIpDetailsJson?ip=${e.id}`)
		.then(r => r.json())
		.then(j => {
			let lat = j.loc.split(",")[0];
			let lon = j.loc.split(",")[1];

			document.getElementById("country").textContent = j.country;
			document.getElementById("city").textContent = j.city;
			document.getElementById("lat").textContent = lat;
			document.getElementById("lon").textContent = lon;
			document.getElementById("timezone").textContent = j.timezone;
			document.getElementById("postal").textContent = j.postal;
			document.getElementById("org").textContent = j.org;
			document.getElementById("ipSpan").textContent = e.id;

			document.getElementById("map").remove();

			let newMap = document.createElement("div");
			newMap.id = "map";

			document.querySelector(".modal-body").insertBefore(newMap, document.querySelector(".ip-details"));

			const map = L.map('map').setView([lat, lon], 12);

			L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
				attribution: '© OpenStreetMap contributors'
			}).addTo(map);

			L.circle([lat, lon], {
				color: 'red',
				fillColor: '#f03',
				fillOpacity: 0.5,
				radius: 5000
			}).addTo(map);

			document.getElementById("open-modal").click();

			setTimeout(() => {
				map.invalidateSize();
			}, 500);
		})
		.catch(e=>console.error(e));
}