"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/cartHub").build();

connection.on("GetShoppingCartUpdate", function (userId, progress) {
    var div = document.getElementById(userId);
    div.style.width = `${progress}%`;
    div.textContent = `${progress}%`;
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
