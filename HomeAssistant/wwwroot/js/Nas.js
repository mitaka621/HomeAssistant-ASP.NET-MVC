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

                var startDate = new Date(item.dateModified);
                var formattedDateTime = formatDate(startDate);

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
    window.removeEventListener('scroll', checkIfAtBottom);

    if (document.querySelector("table")) {
        document.querySelector("table").remove();
        const mainPhotosContainer = document.createElement("div");
        mainPhotosContainer.className += "main-photo-container";
        document.querySelector("main").appendChild(mainPhotosContainer);
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


            document.querySelectorAll("img.photo").forEach(x => {
                x.src = "/nas/getimage?path=" + x.id;
                x.onload = () => {
                    x.classList.remove("loading");
                }
            });

            window.addEventListener('scroll', checkIfAtBottom);
        });
}


function checkIfAtBottom() {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 2000) {
        var photosToSkip = skip + take;
        GetPhotos(photosToSkip);
    }
}

function myFunction() {
    console.log('You have scrolled to the bottom!');
    window.removeEventListener('scroll', checkIfAtBottom);
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
                currentImg.src = "/svg/loading.gif";
                resolve();
            };

            currentImg.src = "/svg/loading.gif";

            const btn = document.createElement("button");


            btn.id = `b${counter++}`;

            btn.setAttribute("onclick", "DisplayImageFull(this)");

            btn.classList = "btn btn-add";
            let reducePercentage = (20000.0 / item.height) / 100;

            let width = item.width * reducePercentage;
            let height = item.height * reducePercentage;

            let spaceAvb = document.querySelector(".main-photo-container").getBoundingClientRect().width;
            spaceAvb -= (photosOnRow.length - 1) * 5;
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

                    let newWidth = x.width - (spaceToReduce / (photosOnRow.length + 1));

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

async function DisplayImageFull(e) {

    disableScroll();

    let closeBtn = document.createElement("a");

    closeBtn.classList = "btn close-btn";
    closeBtn.setAttribute("onclick", "closePopUp(this)");

    closeBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M342.6 150.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L192 210.7 86.6 105.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L146.7 256 41.4 361.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0L192 301.3 297.4 406.6c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L237.3 256 342.6 150.6z"/></svg>`;

    let div = document.createElement("div");
    
    div.classList = "popup-img";

   

    let img = document.createElement("img")

    let actualimg = e.querySelector("img");

    let nextBtn, prevBtn;

    await fetch("/Nas/GetPrevAndNextPathsForPhoto?path=" + actualimg.id)
        .then(x => x.json())
        .then(x => {

            if (x.nextImg) {
                nextBtn = document.createElement("a");

                nextBtn.classList = "btn next-btn";
                nextBtn.setAttribute("onclick", `OpenNextPhotoFull(this)`);
                nextBtn.id = x.nextImg;

                nextBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M464 256A208 208 0 1 1 48 256a208 208 0 1 1 416 0zM0 256a256 256 0 1 0 512 0A256 256 0 1 0 0 256zM294.6 135.1c-4.2-4.5-10.1-7.1-16.3-7.1C266 128 256 138 256 150.3V208H160c-17.7 0-32 14.3-32 32v32c0 17.7 14.3 32 32 32h96v57.7c0 12.3 10 22.3 22.3 22.3c6.2 0 12.1-2.6 16.3-7.1l99.9-107.1c3.5-3.8 5.5-8.7 5.5-13.8s-2-10.1-5.5-13.8L294.6 135.1z"/></svg>`;
            }
            else {
                nextBtn = document.createElement("a");

                nextBtn.classList = "btn next-btn";
            }

            if (x.prevImg) {
                prevBtn = document.createElement("a");

                prevBtn.classList = "btn prev-btn";
                prevBtn.setAttribute("onclick", `OpenNextPhotoFull(this)`);

                prevBtn.id = x.prevImg;

                prevBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M48 256a208 208 0 1 1 416 0A208 208 0 1 1 48 256zm464 0A256 256 0 1 0 0 256a256 256 0 1 0 512 0zM217.4 376.9c4.2 4.5 10.1 7.1 16.3 7.1c12.3 0 22.3-10 22.3-22.3V304h96c17.7 0 32-14.3 32-32V240c0-17.7-14.3-32-32-32H256V150.3c0-12.3-10-22.3-22.3-22.3c-6.2 0-12.1 2.6-16.3 7.1L117.5 242.2c-3.5 3.8-5.5 8.7-5.5 13.8s2 10.1 5.5 13.8l99.9 107.1z"/></svg>`;

            }
            else {
                prevBtn = document.createElement("a");

                prevBtn.classList = "btn prev-btn";
            }
        })

    img.src = `/nas/getimage?path=${actualimg.id}&isFull=true`;

    
    div.appendChild(closeBtn);

    div.innerHTML += `<button class="btn btn-primary download-btn" onclick="OpenModal()">
	                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M169.4 470.6c12.5 12.5 32.8 12.5 45.3 0l160-160c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L224 370.8 224 64c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 306.7L54.6 265.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l160 160z"/></svg>
                                 </button>`;

    if (prevBtn) {
        div.appendChild(prevBtn);
    }
   
    div.appendChild(img);

    if (nextBtn) {
        div.appendChild(nextBtn);
    }

    let main = document.querySelector("main");

    main.insertBefore(div, main.firstChild);
}

function closePopUp(e) {
    enableScroll();
    e.parentElement.remove();
}

async function OpenNextPhotoFull(e) {

    if (document.querySelector("div.popup-img")) {

        document.querySelector("div.popup-img img").src = `/nas/getimage?path=${e.id}&isFull=true`;

        await fetch("/Nas/GetPrevAndNextPathsForPhoto?path=" + e.id)
            .then(x => x.json())
            .then(x => {
                if (x.nextImg) {

                    document.querySelector(".next-btn").setAttribute("onclick", `OpenNextPhotoFull(this)`);
                    document.querySelector(".next-btn").id = x.nextImg;

                    document.querySelector(".next-btn").innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M464 256A208 208 0 1 1 48 256a208 208 0 1 1 416 0zM0 256a256 256 0 1 0 512 0A256 256 0 1 0 0 256zM294.6 135.1c-4.2-4.5-10.1-7.1-16.3-7.1C266 128 256 138 256 150.3V208H160c-17.7 0-32 14.3-32 32v32c0 17.7 14.3 32 32 32h96v57.7c0 12.3 10 22.3 22.3 22.3c6.2 0 12.1-2.6 16.3-7.1l99.9-107.1c3.5-3.8 5.5-8.7 5.5-13.8s-2-10.1-5.5-13.8L294.6 135.1z"/></svg>`;


                } else {
                    document.querySelector(".next-btn").setAttribute("onclick", ``);
                    document.querySelector(".next-btn").id = "";

                    document.querySelector(".next-btn").innerHTML = ``;
                }

                if (x.prevImg) {

                    document.querySelector(".prev-btn").id = x.prevImg;
                    document.querySelector(".prev-btn").setAttribute("onclick", `OpenNextPhotoFull(this)`);

                    document.querySelector(".prev-btn").innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--!Font Awesome Free 6.5.2 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path d="M48 256a208 208 0 1 1 416 0A208 208 0 1 1 48 256zm464 0A256 256 0 1 0 0 256a256 256 0 1 0 512 0zM217.4 376.9c4.2 4.5 10.1 7.1 16.3 7.1c12.3 0 22.3-10 22.3-22.3V304h96c17.7 0 32-14.3 32-32V240c0-17.7-14.3-32-32-32H256V150.3c0-12.3-10-22.3-22.3-22.3c-6.2 0-12.1 2.6-16.3 7.1L117.5 242.2c-3.5 3.8-5.5 8.7-5.5 13.8s2 10.1 5.5 13.8l99.9 107.1z"/></svg>`;


                }
                else {

                    document.querySelector(".prev-btn").setAttribute("onclick", ``);


                    document.querySelector(".prev-btn").innerHTML = ``;


                }
            })

    }
}

var keys = { 37: 1, 38: 1, 39: 1, 40: 1 };

function preventDefault(e) {
    e.preventDefault();
}

function preventDefaultForScrollKeys(e) {
    if (keys[e.keyCode]) {
        preventDefault(e);
        return false;
    }
}

var supportsPassive = false;
try {
    window.addEventListener("test", null, Object.defineProperty({}, 'passive', {
        get: function () { supportsPassive = true; }
    }));
} catch (e) { }

var wheelOpt = supportsPassive ? { passive: false } : false;
var wheelEvent = 'onwheel' in document.createElement('div') ? 'wheel' : 'mousewheel';


function disableScroll() {
    window.addEventListener('DOMMouseScroll', preventDefault, false); // older FF
    window.addEventListener(wheelEvent, preventDefault, wheelOpt); // modern desktop
    window.addEventListener('touchmove', preventDefault, wheelOpt); // mobile
    window.addEventListener('keydown', preventDefaultForScrollKeys, false);
}

function enableScroll() {
    window.removeEventListener('DOMMouseScroll', preventDefault, false);
    window.removeEventListener(wheelEvent, preventDefault, wheelOpt);
    window.removeEventListener('touchmove', preventDefault, wheelOpt);
    window.removeEventListener('keydown', preventDefaultForScrollKeys, false);
}


function OpenModal() {

    const url = document.querySelector("div.popup-img img").src.split("&")[0];

    let currentpath = url.split("=")[1];



    fetch("/nas/GetPhotoInfo?path=" + currentpath)
        .then(r => r.json())
        .then(x => {
            currentpath = currentpath.split("\\");
            currentpath.splice(-1);

            document.getElementById("file-path").textContent = currentpath.join("\\");
            document.getElementById("file-size").textContent = x.size.toFixed(2) + " MB";
            document.getElementById("file-width").textContent = x.width;
            document.getElementById("file-height").textContent = x.height;
            document.getElementById("file-name").textContent = x.path.split("\\").splice(-1);
            document.querySelector(".submit-btn").href ="/nas/DownloadFile?path="+ x.path;

            document.querySelector(".submit-btn").setAttribute("onmousedown", "CloseModal()");

            var startDate = new Date(x.dateModified);
            var formattedDateTime = formatDate(startDate);

            document.getElementById("file-date").textContent = formattedDateTime;
        })


    document.getElementById("open-modal").click();
}


function formatDate(date) {
    var day = date.getDate().toString().padStart(2, '0');
    var month = (date.getMonth() + 1).toString().padStart(2, '0');
    var year = date.getFullYear();
    var hours = date.getHours().toString().padStart(2, '0');
    var minutes = date.getMinutes().toString().padStart(2, '0');
    var seconds = date.getSeconds().toString().padStart(2, '0');
    return day + '-' + month + '-' + year + ' ' + hours + ':' + minutes + ':' + seconds;
}

function CloseModal() {
    document.querySelector(".btn-close").click();
}

let clicked = false;

function togle(e) {

    if (!clicked) {
        clicked = true;
        GetPhotos();
    }
    else {
        location.reload();
    }
    
}