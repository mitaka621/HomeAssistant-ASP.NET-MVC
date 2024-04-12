"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").build();

connection.on("LoadMessage", function (message) {
	document.querySelector(".messages-container").innerHTML +=
	`<div class="other-user-message">
	<div class="smallpfp" asp-area="Identity" asp-page="/Account/Manage/Index">
		<img src="${document.getElementById("otherUserPhoto").value}" alt="Profile Picture" />
	</div>
	<div>
		<p>${message}</p>
	</div>
</div>`
});

connection.start().then(() => document.querySelector(".sendBtn").disabled=false).catch(function (err) {
    return console.error(err.toString());
});

function Send() {
	let chatRoomId = parseInt(document.getElementById("chatRoomId").value);
	let message = document.getElementById("messageToSend").value;
	let recipientId = document.getElementById("recipientId").value;

	connection.invoke("SendMessage", chatRoomId, recipientId, message ).then(function () {
		document.querySelector(".messages-container").innerHTML +=
		`<div class="current-user-message">
				<div>
					<p>${message}</p>
				</div>
				<div class="smallpfp" asp-area="Identity" asp-page="/Account/Manage/Index">
					<img src="${document.getElementById("currentUserPhoto").value}" alt="Profile Picture" />
				</div>
			</div>`

		document.getElementById("messageToSend").value = "";
	
	}).catch(function (error) {
		console.error(error);
	});
}
