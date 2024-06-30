"use strict";

let options = {
    root: null,
    rootMargin: '0px',
    threshold: 0.1
};

let observer = new IntersectionObserver(handleIntersection, options);

document.addEventListener("DOMContentLoaded", () => {
    const spinnerContainer = document.createElement('div');
    spinnerContainer.className = 'spinner-container';

    const spinnerBorder = document.createElement('div');
    spinnerBorder.className = 'spinner-border';
    spinnerBorder.setAttribute('role', 'status');

    const visuallyHiddenSpan = document.createElement('span');
    visuallyHiddenSpan.className = 'visually-hidden';
    visuallyHiddenSpan.innerText = 'Loading...';

    spinnerBorder.appendChild(visuallyHiddenSpan);

    spinnerContainer.appendChild(spinnerBorder);

    document.querySelector(".messages-container").insertBefore(spinnerContainer, document.querySelector(".messages-container").firstChild);

    observer.observe(document.querySelector(".spinner-container"));

    document.getElementById("messageToSend").addEventListener("input", (e) => {
        if (!e.target.value) {
            document.querySelector(".sendBtn").disabled = true;
        } else {
            document.querySelector(".sendBtn").disabled = false;
        }     
    })
});

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

connection.start().then(() => document.querySelector(".sendBtn").disabled ? false:true).catch(function (err) {
    return console.error(err.toString());
});

let chatRoomId = parseInt(document.getElementById("chatRoomId").value);
let recipientId = document.getElementById("recipientId").value;

const delay = ms => new Promise(res => setTimeout(res, ms));
async function Send() {
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

    document.querySelector(".sendBtn").disabled = true;
    document.getElementById("send-container-form").setAttribute("onsubmit", "return false;");
    document.querySelector(".sendBtn>svg").style.fill = "red";

    await delay(1800);
    document.getElementById("send-container-form").setAttribute("onsubmit", "Send(); return false;");
    document.querySelector(".sendBtn>svg").style.fill = "white";
}

let skip = 20;
let take = 20;
function LoadMoreMessages() {
    fetch(`/Message/LoadMessageRangeJson?chatroomId=${chatRoomId}&skip=${skip}&take=${take}`)
        .then(r => r.json())
        .then(data => {
            document.querySelector(".spinner-container").remove();

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
				            <p class="message-date">${formatDateTime(message.createdOn)}</p>
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
			            <p class="message-date">${formatDateTime(message.createdOn)}</p>
		            </div>`;
                    document.querySelector(".messages-container").insertAdjacentElement("afterbegin", div);
                }
            });
            document.querySelector(".messages-container").insertAdjacentHTML("afterbegin", `<div class="spinner-container">
				<div class="spinner-border" role="status">
					<span class="visually-hidden">Loading...</span>
				</div>
			</div>`)

            let observer = new IntersectionObserver(handleIntersection, options);
            observer.observe(document.querySelector(".spinner-container"));

            var newChatBoxHeight = chatBox.scrollHeight;
            var heightDifference = newChatBoxHeight - chatBoxHeight;

            chatBox.scrollTop = currentScrollPos + heightDifference;
        });
}

function handleIntersection(entries, observer) {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            LoadMoreMessages();
        }
    });
}

function formatDateTime(originalDateTime) {
    const date = new Date(originalDateTime);

    const month = date.getMonth() + 1;
    const day = date.getDate();
    const year = date.getFullYear();
    let hours = date.getHours();
    const minutes = date.getMinutes();
    const seconds = date.getSeconds();

    const ampm = hours >= 12 ? 'PM' : 'AM';

    hours = hours % 12;
    hours = hours ? hours : 12; 

    return `${month}/${day}/${year} ${hours}:${minutes}:${seconds} ${ampm}`;
}

