async function fetchTelemetryData() {

    await fetch('/api/HomeTelemetry/data')
        .then(r => r.json())
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

function CheckNasHostStatus() {
    fetch("/nas/CheckConnection").then(r => {
        if (r.ok) {
            document.querySelector(".nas-status div").style.backgroundColor = "#54d359";
        }
        else {
            fetch("/nas/ScanForAvailibleHosts").then(r2 => {
                if (r2.ok) {
                    document.querySelector(".nas-status div").style.backgroundColor = "#54d359";
                }
            })
        }
    })
}

function PingPCs() {
    fetch("/api/wakeonlan/GetAvailiblePCs")
        .then(r => r.json())
        .then(async arr => {
            let promises = [];
            arr.forEach(x => {
                if (x.isHostPc || x.isNAS) {
                    let promise = fetch("/api/wakeonlan/PingPc?name=" + x.pcName)
                        .then(r => {
                            if (r.status === 200) {
                                const indicator = document.querySelector(`#${x.pcName} div.indicator`);
                                indicator.innerHTML = "";
                                indicator.style.backgroundColor = "green";
                                indicator.setAttribute("title", "Running");
                                indicator.classList.add("pcStatus");
                                indicator.classList.remove("clearbtn");
                            }
                            else {
                                const indicator = document.querySelector(`#${x.pcName} div.indicator`);

                                indicator.style.backgroundColor = "red";
                                indicator.setAttribute("title", "Turned Off");
                            }
                        });
                    promises.push(promise);

                }
            });
            await Promise.all(promises);

            loadToolTips();
        });
}

function Wake(e) {
    const id = e.parentElement.parentElement.id;
    fetch("/api/wakeonlan/WakePc?name=" + id).then(r => {
        const indicator=e.parentElement.parentElement.querySelector(".indicator");
        if (r.status === 200) {
            indicator.style.backgroundColor = "white";
            indicator.setAttribute("title", "PoweringOn");
            indicator.classList.add("clearbtn");
            indicator.classList.remove("pcStatus");
            indicator.innerHTML = `<img style="width:1em;" src="/svg/ChasingArrowsLoading.gif">`
        } else {
            indicator.style.backgroundColor = "red";
            indicator.setAttribute("title", "Could not send wake up packet");
        }
        loadToolTips();
    })
}

function Stop(e) {

}

document.addEventListener('DOMContentLoaded', (event) => {
    const wol = document.getElementById('wakeOnLanContainer');
    const nas = document.querySelector(".nas-status");
    if (wol) {
        fetch("/api/wakeonlan/GetAvailiblePCs")
            .then(r => r.json())
            .then(arr => {
                const div = document.getElementById("wakeOnLanContainer");

                arr.forEach(j => {
                    if (j.isHostPc) {
                        div.innerHTML += `				
                        <div class="pc" id="${j.pcName}">
					        <div class="identification">
						        <div type="button" class="btn pcStatus indicator" data-bs-toggle="tooltip" data-bs-placement="top" title="Checking Connection">
						        </div>
						        <div>
							        <p>${j.pcName}  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M64 0C28.7 0 0 28.7 0 64V352c0 35.3 28.7 64 64 64H240l-10.7 32H160c-17.7 0-32 14.3-32 32s14.3 32 32 32H416c17.7 0 32-14.3 32-32s-14.3-32-32-32H346.7L336 416H512c35.3 0 64-28.7 64-64V64c0-35.3-28.7-64-64-64H64zM512 64V288H64V64H512z"/></svg></p>
							        <p class="ip">${j.pcIp}</p>
						        </div>
					        </div>
                           
					        <div class="action-btns">
						        <button class="btn wake" onClick="Wake(this)">
							        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by  - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M73 39c-14.8-9.1-33.4-9.4-48.5-.9S0 62.6 0 80V432c0 17.4 9.4 33.4 24.5 41.9s33.7 8.1 48.5-.9L361 297c14.3-8.7 23-24.2 23-41s-8.7-32.2-23-41L73 39z" /></svg>
						        </button>
						        <button class="btn shutdown" onClick="Stop(this)">
							        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by  - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M0 128C0 92.7 28.7 64 64 64H320c35.3 0 64 28.7 64 64V384c0 35.3-28.7 64-64 64H64c-35.3 0-64-28.7-64-64V128z" /></svg>
						        </button>
					        </div>
				        </div>`
                    }
                    else if (j.isNAS) {
                        div.innerHTML += `				
                        <div class="pc" id="${j.pcName}">
					        <div class="identification">
						        <div type="button" class="btn pcStatus indicator" data-bs-toggle="tooltip" data-bs-placement="top" title="Checking Connection">
						        </div>
						        <div>
							        <p>${j.pcName}     <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M64 32C28.7 32 0 60.7 0 96v64c0 35.3 28.7 64 64 64H448c35.3 0 64-28.7 64-64V96c0-35.3-28.7-64-64-64H64zm280 72a24 24 0 1 1 0 48 24 24 0 1 1 0-48zm48 24a24 24 0 1 1 48 0 24 24 0 1 1 -48 0zM64 288c-35.3 0-64 28.7-64 64v64c0 35.3 28.7 64 64 64H448c35.3 0 64-28.7 64-64V352c0-35.3-28.7-64-64-64H64zm280 72a24 24 0 1 1 0 48 24 24 0 1 1 0-48zm56 24a24 24 0 1 1 48 0 24 24 0 1 1 -48 0z"/></svg></p>
							        <p class="ip">${j.pcIp}</p>
						        </div>
					        </div>
                     
					        <div class="action-btns">
						        <button class="btn wake"  onClick="Wake(this)">
							        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by  - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M73 39c-14.8-9.1-33.4-9.4-48.5-.9S0 62.6 0 80V432c0 17.4 9.4 33.4 24.5 41.9s33.7 8.1 48.5-.9L361 297c14.3-8.7 23-24.2 23-41s-8.7-32.2-23-41L73 39z" /></svg>
						        </button>
						        <button class="btn shutdown" onClick="Stop(this)">
							        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by  - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M0 128C0 92.7 28.7 64 64 64H320c35.3 0 64 28.7 64 64V384c0 35.3-28.7 64-64 64H64c-35.3 0-64-28.7-64-64V128z" /></svg>
						        </button>
					        </div>
				        </div>`
                    }
                    else {
                        div.innerHTML += `				
                        <div class="pc" id="${j.pcName}">
					        <div class="identification">
						        <div type="button" class="btn clearbtn" data-bs-toggle="tooltip" data-bs-placement="top" title="Status Not Available">
						        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M342.6 150.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L192 210.7 86.6 105.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L146.7 256 41.4 361.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0L192 301.3 297.4 406.6c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L237.3 256 342.6 150.6z"/></svg>
                                </div>
						        <div>
							        <p>${j.pcName}               <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M256 512A256 256 0 1 0 256 0a256 256 0 1 0 0 512zM169.8 165.3c7.9-22.3 29.1-37.3 52.8-37.3h58.3c34.9 0 63.1 28.3 63.1 63.1c0 22.6-12.1 43.5-31.7 54.8L280 264.4c-.2 13-10.9 23.6-24 23.6c-13.3 0-24-10.7-24-24V250.5c0-8.6 4.6-16.5 12.1-20.8l44.3-25.4c4.7-2.7 7.6-7.7 7.6-13.1c0-8.4-6.8-15.1-15.1-15.1H222.6c-3.4 0-6.4 2.1-7.5 5.3l-.4 1.2c-4.4 12.5-18.2 19-30.6 14.6s-19-18.2-14.6-30.6l.4-1.2zM224 352a32 32 0 1 1 64 0 32 32 0 1 1 -64 0z"/></svg></p>
						        </div>
					        </div>
          
					        <div class="action-btns">
						        <button class="btn wake" onClick="Wake(this)">
							        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by  - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M73 39c-14.8-9.1-33.4-9.4-48.5-.9S0 62.6 0 80V432c0 17.4 9.4 33.4 24.5 41.9s33.7 8.1 48.5-.9L361 297c14.3-8.7 23-24.2 23-41s-8.7-32.2-23-41L73 39z" /></svg>
						        </button>
					        </div>
				        </div>`
                    }
                });
                loadToolTips();

                PingPCs();
                setInterval(PingPCs, 60000);
            });
    }

    if (nas) {
        CheckNasHostStatus();
        setInterval(CheckNasHostStatus, 60000);
    }
});


fetchTelemetryData();
setInterval(fetchTelemetryData, 5 * 60 * 1000);

