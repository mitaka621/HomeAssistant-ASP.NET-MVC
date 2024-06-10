"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.on("PushNotfication", function (notification) {
    let newNotification = document.createElement("div");
    newNotification.classList.add("accordion-item");
    newNotification.style.maxWidth = "0";
    newNotification.style.maxHeight = "0";

	if (/\S/.test(notification.invoker.photo)) {
        newNotification.innerHTML = `<h2 class="accordion-header" id="headingTwo">
					<button class="accordion-button collapsed alert alert-danger" type="button" data-bs-toggle="collapse" data-bs-target="#n${notification.id}" aria-expanded="false" aria-controls="${notification.id}">
						<div class="notf-title">
							<p>${notification.title}</p>
							<p class="text-muted">Less than a minute ago</p>							
							<figcaption figcaption class="blockquote-footer ${notification.source}">
								${notification.source}
							</figcaption>														
						</div>						
						<div class="notification-img-container">								
							<div class="smallpfp">
								<img src="data:image/jpeg;base64,${notification.invoker.photo}" alt="Profile Picture" />
							</div>
							${notification.invoker.firstName}
						</div>
					</button>
				</h2>
				<div id="n${notification.id}" class="accordion-collapse collapse" aria-labelledby="headingTwo" data-bs-parent="#accordionExample">
					<div class="accordion-body">
						${notification.description.replaceAll("\r\n", "<br />")}						
					</div>
					<a id="b${notification.id}" class="btn btn-danger dismiss" onclick="Dismiss(this)">Dismiss</a>
				</div>`
    }
    else {

        newNotification.innerHTML = `<h2 class="accordion-header" id="headingTwo">
					<button class="accordion-button collapsed alert alert-danger" type="button" data-bs-toggle="collapse" data-bs-target="#n${notification.id}" aria-expanded="false" aria-controls="${notification.id}">
						<div class="notf-title">
							<p>${notification.title}</p>
							<p class="text-muted">Less than a minute ago</p>							
							<figcaption figcaption class="blockquote-footer ${notification.source}">
								${notification.source}
							</figcaption>														
						</div>												
					</button>
				</h2>
				<div id="n${notification.id}" class="accordion-collapse collapse" aria-labelledby="headingTwo" data-bs-parent="#accordionExample">
					<div class="accordion-body">
						${notification.description}						
					</div>
					<a id="b${notification.id}" class="btn btn-danger dismiss" onclick="Dismiss(this)">Dismiss</a>
				</div>`

    }




    let div = document.querySelector("div.notifications");
    div.insertBefore(newNotification, div.firstChild);

    setTimeout(function () {
        newNotification.style.maxWidth = "999px";
        newNotification.style.maxHeight = "999px";
    }, 1);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

async function Dismiss(e) {
    let notificationId = e.id.split("b")[1];
    await connection.invoke("MarkAsDismissed", parseInt(notificationId)).then(function () {
        e.parentElement.parentElement.style.maxWidth = "0";
        e.parentElement.parentElement.style.maxHeight = "0";
        setTimeout(function () {
            e.parentElement.parentElement.remove();
        }, 500);

    }).catch(function (error) {
        console.error("Error dismissing notification:");
    });
}

function DismissAll() {
    fetch(`api/Notification/DismissAllNotificationsForUser`);

    document.querySelector(".spinner-container").remove();
    document.querySelectorAll("a.dismiss").forEach(x => {
        x.parentElement.parentElement.style.maxWidth = "0";
        x.parentElement.parentElement.style.maxHeight = "0";
        setTimeout(function () {
            x.parentElement.parentElement.remove();
        }, 500);
    });
    

}


let skip = 10;
let take = 10;

let notificationsContainer = document.querySelector(".notifications");
function LoadMoreMessages() {
    fetch(`api/Notification/GetNotificationsForUserJson?skip=${skip}&take=${take}`)
        .then(r => r.json())
        .then(data => {
            document.querySelector(".spinner-container").remove();

            if (data.notificationsContent.length === 0) {
                return;
            }

            skip += take;

            data.notificationsContent.forEach(notification => {
                const notificationElement = createNotificationElement(notification, data.profilePictures[notification.invoker.id]);
                notificationsContainer.appendChild(notificationElement);				
            });
            document.querySelector(".notifications").innerHTML += `<div class="spinner-container">
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
    rootMargin: '0px',
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

function formatDuration(createdOn) {
	const notificationTime = new Date(createdOn);
	const currentTime = new Date();

	const duration = (currentTime - notificationTime) / 1000; // Convert to seconds

	let convertedDuration = "";

	if (duration >= 86400) {
		convertedDuration += Math.round(duration / 86400) + (Math.round(duration / 86400) === 1 ? " day" : " days") + " ago";
	} else if (duration >= 3600) {
		convertedDuration += Math.round(duration / 3600) + (Math.round(duration / 3600) === 1 ? " hour" : " hours") + " ago";
	} else if (duration >= 60) {
		convertedDuration += Math.round(duration / 60) + (Math.round(duration / 60) === 1 ? " minute" : " minutes") + " ago";
	} else {
		convertedDuration += "Less than a minute ago";
	}

	return convertedDuration;
}

function createNotificationElement(notification, imgData) {
   
    const accordionItem = document.createElement('div');
    accordionItem.classList.add('accordion-item');

    const accordionHeader = document.createElement('h2');
    accordionHeader.classList.add('accordion-header');
    accordionHeader.setAttribute('id', `heading${notification.id}`);

    const button = document.createElement('button');
    button.classList.add('accordion-button', 'collapsed');
    button.setAttribute('type', 'button');
    button.setAttribute('data-bs-toggle', 'collapse');
    button.setAttribute('data-bs-target', `#n${notification.id}`);
    button.setAttribute('aria-expanded', 'false');
    button.setAttribute('aria-controls', `n${notification.id}`);

    const notfTitleContainer = document.createElement('div');
    notfTitleContainer.classList.add('notf-title');

    const titleParagraph = document.createElement('p');
    titleParagraph.textContent = notification.title;

    const timeParagraph = document.createElement('p');
    timeParagraph.classList.add('text-muted');
    timeParagraph.textContent = formatDuration(notification.createdOn);

    const figcaption = document.createElement('figcaption');
    figcaption.classList.add('blockquote-footer', notification.source);
    figcaption.textContent = notification.source;

    const invokerContainer = document.createElement('div');
    invokerContainer.classList.add('notification-img-container');

    const smallpfpContainer = document.createElement('div');
    smallpfpContainer.classList.add('smallpfp');

    if (imgData) {
        const img = document.createElement('img');
        img.setAttribute('src', `data:image/jpeg;base64,${imgData}`);
        img.setAttribute('alt', 'Profile Picture');

        smallpfpContainer.appendChild(img);
        invokerContainer.appendChild(smallpfpContainer);
        invokerContainer.innerHTML += notification.invoker.firstName;
    }

    const accordionCollapse = document.createElement('div');
    accordionCollapse.setAttribute('id', `n${notification.id}`);
    accordionCollapse.classList.add('accordion-collapse', 'collapse');
    accordionCollapse.setAttribute('aria-labelledby', `heading${notification.id}`);
    accordionCollapse.setAttribute('data-bs-parent', '#accordionExample');

    const accordionBody = document.createElement('div');
    accordionBody.classList.add('accordion-body');
    accordionBody.innerHTML = notification.description.replace(/\r\n/g, "<br />");

    const dismissButton = document.createElement('a');
    dismissButton.setAttribute('id', `b${notification.id}`);
    dismissButton.classList.add('btn', 'btn-danger', 'dismiss');
    dismissButton.textContent = 'Dismiss';
    dismissButton.setAttribute("onclick", "Dismiss(this)");


    // Construct hierarchy
    notfTitleContainer.appendChild(titleParagraph);
    notfTitleContainer.appendChild(timeParagraph);
    notfTitleContainer.appendChild(figcaption);

    button.appendChild(notfTitleContainer);
    button.appendChild(invokerContainer);

    accordionHeader.appendChild(button);

    accordionCollapse.appendChild(accordionBody);
    accordionCollapse.appendChild(dismissButton);

    accordionItem.appendChild(accordionHeader);
    accordionItem.appendChild(accordionCollapse);

    return accordionItem;
}
