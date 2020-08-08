const hubConnection = new signalR.HubConnectionBuilder()
	.withUrl(HubUrl)
	.configureLogging(signalR.LogLevel.Information)
	.build();

var UpdatingMessagesNow = false;

$(document).ready(function () {
	//SignalR
	//Отобразить сообщение
	hubConnection.on('Send', function (message, name, createdDate, dialogId, avatar) {
		if (DialogID === dialogId) {
			$("#dialog").append(`<div class="mt-2">
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
			$('#dialog').scrollTop($('#dialog')[0].scrollHeight);
			$("#inputForm input").focus();
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

	$("#inputForm input").focus(); //фокус на форму при загрузки страницы

	$("#inputForm input").keyup(function (e) { //отправка по нажатию на enter
		var code = e.key;
		if (code === "Enter") {
			e.preventDefault();
			$("#sendBtn").click();
		}
	});

	$("#dialog").scroll(function () {
		if ($("#dialog").scrollTop() <= 500 && UpdatingMessagesNow === false && TotalCount > Offset * 50) {
			//console.log($("#dialog").scrollTop()); //for debug
			UpdatingMessagesNow = true;
			axios.get('/Dialog/GetMessagesJSON',
				{ params: { dialogId: DialogID, offset: Offset * 50 } })
				.then(response => {
					response.data.messageList.forEach(element => {
						$("#dialog").prepend(
							`
                            <div class="mt-3">
			            	    <div class="d-flex">
					                <b class="border-bottom w-100">${element.fullName}</b>
				                </div>
				                <div class="d-flex justify-content-between">
					                <span>
					        	       ${element.message}
					                </span>
					                <span id="date">${element.date}</span>
				                </div>
			                </div>
                        `
						);
						//console.log(element.date + " " + element.message); //for debug
					});
					Offset++;
					UpdatingMessagesNow = false;
				});
		}
	});
});
