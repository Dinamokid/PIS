﻿const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(HubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();

$(document).ready(function () {

    //Отобразить сообщение
    hubConnection.on('Send', function (message, name, createdDate, dialogId, avatar) {
        toastr.info(`<div class="cursor-pointer" onclick="location.href = '/Dialog/WithId/${dialogId}'">
	                        <b>Новое сообщение!</p>
	                        <div class="d-flex">
		                        <div style="background-image: url('${avatar}'); background-size:cover; background-position:center; width: 40px; height: 40px; border-radius: 100%;"></div>
		                        <div class="ml-2">
			                        <b>${name}</b>
			                        <p class="mb-0 pb-0">${message}</p>
		                        </div>
	                        </div>
                        </div>`);
        UpdateDialog(dialogId, message, avatar, createdDate);
    });

    function UpdateDialog(dialogId, message, avatar, createdDate) {
        $(`#dialog_${dialogId} #message-dialog_${dialogId}`).text(message);
        $(`#dialog_${dialogId} #lastAutor-dialog_${dialogId}`).css("background-image", `url('${avatar}')`);
        $(`#dialog_${dialogId} #lastMessageDate-dialog_${dialogId}`).text(createdDate);
    }

    hubConnection.start();
});