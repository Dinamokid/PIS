const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(HubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();

$(document).ready(function () {

    //Отобразить сообщение
    hubConnection.on('Send', function (message, name, createdDate, dialogId, avatar) {
        if (DialogID === dialogId) {
            $("#chatroom").append(`<div class="mt-2">
                    <div class="d-flex">
                        <b class="border-bottom w-100">`+ name + `</b>
                    </div>
                    <div class="d-flex justify-content-between">
                        <span>
                           `+ message + `
                        </span>
                        <span id="date">`+ createdDate + `</span>
                    </div>
                </div>`);
            $("#message").val("");
        }
        else {
            toastr.info(`Вам пришло новое сообщение от ${name}`);
            toastr.info(`<a href="~/Dialog/WithId/${dialogId}">
	                        <p>Новое сообщение!</p>
	                        <div class="d-flex">
		                        <div style="background-image: url('${avatar}'); background-size:cover; background-position:center; width: 40px; height: 40px; border-radius: 100%;"></div>
		                        <div>
			                        <b>${name}</b>
			                        <p>${message}</p>
		                        </div>
	                        </div>
                        </a>`);
        }
    });

    //Отправить сообщение
    document.getElementById("sendBtn").addEventListener("click", function (e) {
        let message = document.getElementById("message").value;
        hubConnection.invoke('Send', message, DialogID);
    });

    hubConnection.start();
});
