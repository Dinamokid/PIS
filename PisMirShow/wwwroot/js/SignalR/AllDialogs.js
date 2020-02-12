const hubConnection = new signalR.HubConnectionBuilder()
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
    });

    hubConnection.start();
});