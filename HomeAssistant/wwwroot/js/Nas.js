let skip = 100;
let take = 100;


let notificationsContainer = document.querySelector(".notifications");
function LoadMoreMessages() {
    fetch(`NAS/GetFilesJson?skip=${skip}&take=${take}&path=${document.getElementById("path").textContent}`)
        .then(r => r.json())
        .then(data => {

            if (document.querySelector(".spinner-container")) {
                document.querySelector(".spinner-container").remove();
            }
          
            if (data.length === 0 || !document.getElementById("path").textContent) {
                return;
            }

            skip += take;



            data.forEach(item => {

                const options = {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit',
                    second: '2-digit',
                    hour12: false
                };

                const formattedDateTime = item.dateModified.toLocaleString('en-GB', options);

                var tbody = document.querySelector("tbody");

                if (item.isFile === 1) {
                    const row = document.createElement('tr');

                    const cell1 = document.createElement('td');
                    cell1.style.overflow = 'hidden';
                    cell1.style.textOverflow = 'ellipsis';
                    cell1.innerHTML = `<img src="/svg/file.svg" width="50" height="50"> <a href="/NAS/DownloadFile?path=${item.path}">${item.displayName}</a>`;
                    row.appendChild(cell1);

                    const cell2 = document.createElement('td');
                    cell2.style.textAlign = 'center';
                    cell2.textContent = `${(Math.round(item.size * 100) / 100).toFixed(2)} MB`;
                    row.appendChild(cell2);

                    const cell3 = document.createElement('td');
                    cell3.style.textAlign = 'center';
                    cell3.textContent = formattedDateTime;
                    row.appendChild(cell3);

                    tbody.appendChild(row);
                } else {
                    const row = document.createElement('tr');

                    const cell1 = document.createElement('td');
                    cell1.style.overflow = 'hidden';
                    cell1.style.textOverflow = 'ellipsis';
                    cell1.innerHTML = `<img src="/svg/folder.svg" width="50" height="50"><a href="/NAS?path=${item.path}}">${item.displayName}</a>`;
                    row.appendChild(cell1);

                    const cell2 = document.createElement('td');
                    row.appendChild(cell2);
                    const cell3 = document.createElement('td');
                    row.appendChild(cell3);

                    tbody.appendChild(row);
                }
            });
            document.querySelector("main").innerHTML += `<div class="spinner-container">
				<div class="spinner-border" role="status">
					<span class="visually-hidden">Loading...</span>
				</div>
			</div>`;

            let observer = new IntersectionObserver(handleIntersection, options);
            observer.observe(document.querySelector(".spinner-container"));
        })
        .catch(() => {
            if (document.querySelector(".spinner-container")) {
                document.querySelector(".spinner-container").remove();
            }
        });
}

let options = {
    root: null,
    rootMargin: '5000px',
    threshold: 0.1
};

let observer = new IntersectionObserver(handleIntersection, options);
observer.observe(document.querySelector(".spinner-container"));

function handleIntersection(entries, observer) {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            LoadMoreMessages();
        }
    });
}

function GetPhotos() {
    document.querySelector("table").remove();

    document.querySelector("main").innerHTML += `<div class="main-photo-container"></div>`;

    skip = 0;


    fetch(`NAS/GetFilesJson?skip=${skip}&take=${take}&path=${document.getElementById("path").textContent}`)
        .then(r => r.json())
        .then(async data => {
            if (document.querySelector(".spinner-container")) {
                document.querySelector(".spinner-container").remove();
            }
        

            if (data.length === 0 || !document.getElementById("path").textContent) {
                return;
            }

            skip += take;
            let promises = [];
            for (var i = 0; i < data.length; i++) {
                await loadImage(data[i]);           
            }

            document.querySelectorAll("img.photo").forEach(x => {
                x.src = "/nas/getimage?path=" + x.id;
            })
          
        });

   

}

function loadImage(item) {
    return new Promise((resolve, reject) => {
        const validFormats = ['.jpg', '.jpeg', '.png', '.gif', '.svg', '.webp'];
        const ext = item.path.slice(item.path.lastIndexOf('.')).toLowerCase();

        if (item.isFile !== 1 || !validFormats.includes(ext)) {
            resolve(); // Skip loading invalid or non-file items
        } else {
            const currentImg = document.createElement("img");
            currentImg.id = item.path;
            currentImg.classList.add("photo");

            currentImg.onload = () => {
                resolve(); // Resolve the promise when the image is loaded
            };

            currentImg.onerror = () => {
                console.error(`Failed to load image: ${item.path}`);
                resolve(); // Resolve the promise even if the image fails to load
            };

            currentImg.src = "/svg/loading.gif";
            document.querySelector(".main-photo-container").appendChild(currentImg);
        }
    });
}

function DownloadImg(path) {
        return fetch(`/NAS/GetImage?path=${path}`);
    
}