var connection = new signalR.HubConnectionBuilder().withUrl("/usersActiviryHub").build();

connection.on("PushNewLogEntry", function (model) {
    insertNewLogEntry(model);

    assignUsernameColors();
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
    usernameParagraph.className = `username ${model.userName}`;
    usernameParagraph.textContent = `@${model.userName}`;
    authorDiv.appendChild(usernameParagraph);

    const accordionCollapse = document.createElement('div');
    accordionCollapse.id = `a${randomId}`;
    accordionCollapse.className = 'accordion-collapse collapse';
    accordionItem.appendChild(accordionCollapse);

    const accordionBody = document.createElement('div');
    accordionBody.className = 'accordion-body';
    accordionCollapse.appendChild(accordionBody);

    if (model.queryString) {
        const bodyJsonHeader1 = document.createElement('h3');
        bodyJsonHeader1.textContent = 'URL Query';
        accordionBody.appendChild(bodyJsonHeader1);

        const bodyJsonHeader2 = document.createElement('p');
        bodyJsonHeader2.innerHTML = "<strong>" + model.queryString + "</strong>";
        accordionBody.appendChild(bodyJsonHeader2);
    }

    const bodyJsonHeader = document.createElement('h3');
    bodyJsonHeader.textContent = 'Body Json';
    accordionBody.appendChild(bodyJsonHeader);

    const preElement = document.createElement('pre');
    preElement.className = 'code-block';
    preElement.textContent = formattedJsonString;
    accordionBody.appendChild(preElement);

    const interactionsContainer = document.querySelector('.interactions-container');
    if (interactionsContainer) {
        if (window.location.href.includes('?page=1') || !window.location.href.includes('page')) {
            interactionsContainer.insertBefore(overallLogContainer, interactionsContainer.firstChild);
        }

    } else {
        console.error('Interactions container not found');
    }
}

function getRandomColor() {
    const letters = '012345678AE';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 11)];
    }
    return color;
}
const userIdColors = {};

function assignUsernameColors() {
    const pElements = document.querySelectorAll('p.username');

    pElements.forEach(p => {
        const classes = Array.from(p.classList);
        classes.forEach(className => {
            if (!userIdColors[className]) {
                userIdColors[className] = getRandomColor();
            }
            p.style.color = userIdColors[className];
        });
    });

}

assignUsernameColors();
