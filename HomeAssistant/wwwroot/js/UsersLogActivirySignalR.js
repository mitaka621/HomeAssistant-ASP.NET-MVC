"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/usersActiviryHub").build();

connection.on("PushNewLogEntry", function (model) {
    insertNewLogEntry(model);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function formatDateTime(dateTime) {
    const date = new Date(dateTime);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
}


function insertNewLogEntry(model) {

    let randomId = String(Math.random()).split(".")[1];

    const formattedDateTime = formatDateTime(model.dateTime);

    const formattedJsonString = JSON.stringify(JSON.parse(model.actionArgumentsJson), null, 2);

    const overallLogContainer = document.createElement('div');
    overallLogContainer.className = 'overall-log-container zoom-in';

    const timeParagraph = document.createElement('p');
    timeParagraph.classList = "time new";
    timeParagraph.textContent = `${formattedDateTime}:`;
    overallLogContainer.appendChild(timeParagraph);

    const accordionItem = document.createElement('div');
    accordionItem.className = 'accordion-item';
    overallLogContainer.appendChild(accordionItem);

    const accordionHeader = document.createElement('h2');
    accordionHeader.className = 'accordion-header';
    accordionHeader.id = 'headingTwo';
    accordionItem.appendChild(accordionHeader);

    const accordionButton = document.createElement('button');
    accordionButton.className = 'accordion-button collapsed';
    accordionButton.type = 'button';
    accordionButton.setAttribute('data-bs-toggle', 'collapse');
    accordionButton.setAttribute('data-bs-target', `#a${randomId}`);
    accordionButton.setAttribute('aria-expanded', 'false');
    accordionButton.setAttribute('aria-controls', 'collapseTwo');
    accordionHeader.appendChild(accordionButton);

    const interactionDiv = document.createElement('div');
    interactionDiv.className = 'interaction';
    accordionButton.appendChild(interactionDiv);

    const urlDiv = document.createElement('div');
    urlDiv.className = 'url';
    interactionDiv.appendChild(urlDiv);

    const requestTypeParagraph = document.createElement('p');
    requestTypeParagraph.className = 'request-type';
    requestTypeParagraph.textContent = model.requestType;
    urlDiv.appendChild(requestTypeParagraph);

    const requestUrlParagraph = document.createElement('p');
    requestUrlParagraph.className = 'request-url';
    requestUrlParagraph.title = model.queryString;
    requestUrlParagraph.innerHTML = `${model.requestUrl}<strong>${model.queryString}</strong>`;
    urlDiv.appendChild(requestUrlParagraph);

    const authorDiv = document.createElement('div');
    authorDiv.className = 'author';
    interactionDiv.appendChild(authorDiv);

    const usernameParagraph = document.createElement('p');
    usernameParagraph.className = 'username';
    usernameParagraph.textContent = `@${model.userName}`;
    authorDiv.appendChild(usernameParagraph);

    const accordionCollapse = document.createElement('div');
    accordionCollapse.id = `a${randomId}`;
    accordionCollapse.className = 'accordion-collapse collapse';
    accordionItem.appendChild(accordionCollapse);

    const accordionBody = document.createElement('div');
    accordionBody.className = 'accordion-body';
    accordionCollapse.appendChild(accordionBody);

    const bodyJsonHeader = document.createElement('h3');
    bodyJsonHeader.textContent = 'Body Json';
    accordionBody.appendChild(bodyJsonHeader);

    const preElement = document.createElement('pre');
    preElement.className = 'code-block';
    preElement.textContent = formattedJsonString;
    accordionBody.appendChild(preElement);

    const interactionsContainer = document.querySelector('.interactions-container');
    if (interactionsContainer) {
        console.log(window.location.href);
        if (window.location.href.includes('?page=1') || !window.location.href.includes('page')) {
            interactionsContainer.insertBefore(overallLogContainer, interactionsContainer.firstChild);
        }      

    } else {
        console.error('Interactions container not found');
    }
}
