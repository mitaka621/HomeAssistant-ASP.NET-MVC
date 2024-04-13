"use strict";

var chatBox = document.querySelector(".card-body");
chatBox.scrollTop = chatBox.scrollHeight;

var connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").build();

connection.on("LoadMessage", function (message) {
    var div = document.createElement("div");
    div.classList.add("other-user-message");
    div.innerHTML =
        `
		<div class="smallpfp">
			<img src="${document.getElementById("otherUserPhoto").value}" alt="Profile Picture" />
		</div>
		<div class="message-and-time2">
			<p class="message-text">${message}</p>
			<p class="message-date">Less than a minute ago</p>
		</div>`;
    document.querySelector(".messages-container").append(div);

    var chatBox = document.querySelector(".card-body");
    chatBox.scrollTop = chatBox.scrollHeight;
});

connection.start().then(() => document.querySelector(".sendBtn").disabled = false).catch(function (err) {
    return console.error(err.toString());
});

let chatRoomId = parseInt(document.getElementById("chatRoomId").value);
let recipientId = document.getElementById("recipientId").value;

function Send() {
    let message = document.getElementById("messageToSend").value;

    var div = document.createElement("div");
    div.classList.add("current-user-message");
    div.innerHTML =
        `<div class="message-and-time">
				<p class="message-text">${message}</p>
				<p class="message-date">Less than a minute ago</p>
			</div>
			<div class="smallpfp">
				<img src="${document.getElementById("currentUserPhoto").value}" alt="Profile Picture" />
			</div>`

    document.querySelector(".messages-container").append(div);

    document.getElementById("messageToSend").value = "";

    var chatBox = document.querySelector(".card-body");
    chatBox.scrollTop = chatBox.scrollHeight;

    connection.invoke("SendMessage", chatRoomId, recipientId, message).catch(function (error) {
        div.classList += (" alert alert-danger");
        div.querySelector(".message-date").textContent = "Couldn't send message!";

    });
}

let skip = 50;
let take = 50;
function LoadMoreMessages() {
    fetch(`/Message/LoadMessageRangeJson?chatroomId=${chatRoomId}&skip=${skip}&take=${take}`)
        .then(r => r.json())
        .then(data => {
            document.querySelector(".spinner-border").remove();

            if (data.length===0) {
                return;
            }

            skip += take;

            var currentScrollPos = chatBox.scrollTop;
            var chatBoxHeight = chatBox.scrollHeight;

            data.forEach(message => {
                if (message.userId != recipientId) {
                    var div = document.createElement("div");
                    div.classList.add("current-user-message");
                    div.innerHTML =
                        `<div class="message-and-time">
				            <p class="message-text">${message.messageContent}</p>
				            <p class="message-date">${message.createdOn}</p>
			            </div>
			            <div class="smallpfp">
				            <img src="${document.getElementById("currentUserPhoto").value}" alt="Profile Picture" />
			            </div>`

                    document.querySelector(".messages-container").insertAdjacentElement("afterbegin", div);
                }
                else {
                    var div = document.createElement("div");
                    div.classList.add("other-user-message");
                    div.innerHTML =
                        `
		            <div class="smallpfp">
			            <img src="${document.getElementById("otherUserPhoto").value}" alt="Profile Picture" />
		            </div>
		            <div class="message-and-time2">
			            <p class="message-text">${message.messageContent}</p>
			            <p class="message-date">${message.createdOn}</p>
		            </div>`;
                    document.querySelector(".messages-container").insertAdjacentElement("afterbegin", div);
                }
            });
            document.querySelector(".messages-container").insertAdjacentHTML("afterbegin", `<div class="spinner-border" role="status">
				<span class="visually-hidden">Loading...</span>
			</div>`)

            let observer = new IntersectionObserver(handleIntersection, options);
            observer.observe(document.querySelector(".spinner-border"));

            var newChatBoxHeight = chatBox.scrollHeight;
            var heightDifference = newChatBoxHeight - chatBoxHeight;

            chatBox.scrollTop = currentScrollPos + heightDifference;
        });
}

let options = {
    root: null,
    rootMargin: '0px',
    threshold: 0.1
};

let observer = new IntersectionObserver(handleIntersection, options);
observer.observe(document.querySelector(".spinner-border"));

function handleIntersection(entries, observer) {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            LoadMoreMessages();
        }
    });
}
