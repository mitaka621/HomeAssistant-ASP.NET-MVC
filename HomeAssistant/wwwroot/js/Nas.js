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
                    cell1.innerHTML = `<img src="/svg/folder.svg" width="50" height="50"><a href="/NAS?path=${item.path}">${item.displayName}</a>`;
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

let options2 = {
    root: null,
    rootMargin: '2000px',
    threshold: 0.1
};


function handleIntersectionForPhoto(entries, observer) {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            var photosToSkip = skip + take;
            GetPhotos(photosToSkip);
        }
    });
}

let photosOnRow = [];
function GetPhotos(skipnum = 0) {
    if (document.querySelector("button.viewer")) {
        document.querySelector("button.viewer").remove();
        document.querySelector("table").remove();
        document.querySelector("main").innerHTML += `<button class="btn btn-primary" type="button" onclick="location.reload()">Go to File Explorer</button>
    <div class="main-photo-container"></div>`;
       
    }

    skip = skipnum;
   

    fetch(`NAS/GetFilesJson?skip=${skip}&take=${take}&path=${document.getElementById("path").textContent}`)
        .then(r => r.json())
        .then(async data => {
            if (document.querySelector(".spinner-container")) {
                document.querySelector(".spinner-container").remove();
            }
        
            if (data.length === 0 || !document.getElementById("path").textContent) {
                return;
            }
            for (var i = 0; i < data.length; i++) {
                await loadImage(data[i]);    
               
            }

            let promises = [];

            document.querySelectorAll("img.photo").forEach(x => {
                x.src = "/nas/getimage?path=" + x.id;

                promises.push(new Promise((resolve, reject) => {
                    x.onload = () => {
                        x.classList.remove("loading");
                        resolve();
                    }
                }));
            });

            await Promise.all(promises);

         

            document.querySelector("main").innerHTML += `<div class="spinner-container">
				<div class="spinner-border" role="status">
					<span class="visually-hidden">Loading...</span>
				</div>
			</div>`;

            let observer2 = new IntersectionObserver(handleIntersectionForPhoto, options2);
            observer2.observe(document.querySelector(".spinner-container"));
        });
}

let counter = 0;
function loadImage(item) {
    

    return new Promise((resolve, reject) => {
       
        const validFormats = ['.jpg', '.jpeg', '.png', '.gif', '.svg', '.webp'];
        const ext = item.path.slice(item.path.lastIndexOf('.')).toLowerCase();

        if (item.isFile !== 1 || !validFormats.includes(ext)) {
            resolve();
        } else {
            const currentImg = document.createElement("img");
            
            currentImg.id = item.path;
            currentImg.classList = "photo loading";

            currentImg.onload = () => {
                resolve();
            };

            currentImg.onerror = () => {
                console.error(`Failed to load image: ${item.path}`);
                resolve(); 
            };

            currentImg.src = "/svg/loading.gif";

            const btn = document.createElement("button");

            btn.id = `b${counter++}`;

            btn.classList = "btn btn-add";
            let reducePercentage = (20000.0 / item.height) / 100;

            let width = item.width * reducePercentage;
            let height = item.height * reducePercentage;

            let spaceAvb = document.querySelector(".main-photo-container").getBoundingClientRect().width;
            spaceAvb -= (photosOnRow.length - 1)*5;
            spaceAvb -= photosOnRow.reduce((total, current) => total + current.width, 0);

            let shouldWriteToRow = true;

            if (spaceAvb < 110) {
               
                photosOnRow.forEach(x => {
                   
                    let newWidth = x.width + (spaceAvb / photosOnRow.length);

                   

                    document.getElementById(x.obj).style.width = `${newWidth}px`;
                    document.getElementById(x.obj).querySelector("img").style.width = `${newWidth}px`;
                })

               
                photosOnRow = [];
            }
            else if (spaceAvb < width) {


                shouldWriteToRow = false;

                var spaceToReduce = ((photosOnRow.length) * 5) + photosOnRow.reduce((total, current) => total + current.width, 0) + width - document.querySelector(".main-photo-container").getBoundingClientRect().width;

                width -= (spaceToReduce / (photosOnRow.length + 1));

                

                photosOnRow.forEach(x => {

                    let newWidth = x.width - (spaceToReduce / (photosOnRow.length+1));

                    document.getElementById(x.obj).style.width = `${newWidth}px`;
                })

                
                photosOnRow = [];
            }


            btn.style.width = `${width}px`;
            btn.style.height = `${height}px`;
            btn.appendChild(currentImg);
            document.querySelector(".main-photo-container").appendChild(btn);

            if (shouldWriteToRow) {
                photosOnRow.push({ obj: btn.id, width: width, height: height });
            }
           
        }
    });
}

function DownloadImg(path) {
        return fetch(`/NAS/GetImage?path=${path}`);
    
}