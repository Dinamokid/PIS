//Показать ошибку
hubConnection.on('Error', function (message) {
	toastr.error(message);
});
