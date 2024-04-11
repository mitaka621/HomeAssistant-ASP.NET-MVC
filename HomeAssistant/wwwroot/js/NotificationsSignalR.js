"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.on("PushNotfication", function (notification) {
	console.log(notification);

    var div = document.querySelector("div.notifications");

	console.log(div);

    div.innerHTML = `<div class="accordion-item">
				<h2 class="accordion-header" id="headingTwo">
					<button class="accordion-button collapsed alert alert-danger" type="button" data-bs-toggle="collapse" data-bs-target="#n${notification.id}" aria-expanded="false" aria-controls="${notification.id}">
						<div class="notf-title">
							<p>${notification.title}</p>
							<p class="text-muted">Less than a minute ago</p>							
							<figcaption figcaption class="blockquote-footer">
								${notification.source}
							</figcaption>														
						</div>						
						<div>								
							<div class="smallpfp">
								<img src="data:image/jpeg;base64,${notification.invoker.photo}" alt="Profile Picture" />
							</div>
							${notification.invoker.firstName}
						</div>
					</button>
				</h2>
				<div id="n${notification.id}" class="accordion-collapse collapse" aria-labelledby="headingTwo" data-bs-parent="#accordionExample">
					<div class="accordion-body">
						${notification.description}
					</div>
				</div>
</div>`+ div.innerHTML;
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
