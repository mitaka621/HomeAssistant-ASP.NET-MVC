let skip = 100;
let take = 100;


let notificationsContainer = document.querySelector(".notifications");
function LoadMoreMessages() {
    fetch(`NAS/GetFilesJson?skip=${skip}&take=${take}&path=${document.getElementById("path").textContent}`)
        .then(r => r.json())
        .then(data => {
            document.querySelector(".spinner-container").remove();

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

                var tbody=document.querySelector("tbody");

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