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

function Dismiss(e) {
    let notificationId = e.id.split("b")[1];
    connection.invoke("MarkAsDismissed", parseInt(notificationId)).then(function () {
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
    document.querySelectorAll("a.dismiss").forEach(x => x.click());
}
