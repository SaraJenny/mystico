$(document).ready(function () {
	/*
	MENY
	*/
	/* Togglar menyn */
	$('#toggle-nav').click(function (e) {
		e.preventDefault();
		$(this).toggleClass('active');
		$('#mainmenu ul').toggleClass('active');
	});
	/*
	LÄGG TILL VÄNNER
	*/
	var string = "";
	var idString = "";
	var eventIdString = "";

	/*
	SPLIT/EVENT
	*/
	$('#friendsTextBox').keypress(function (e) {
		// töm vänboxen
		$('.friend-box').html("");
		string += e.originalEvent.key;
		$.ajax({
			url: "/Json/GetAllUsersExceptMe",
			type: "GET",
			data: {
				id: string
			},
			success: function (result) {
				for (var i = 0; i < result.length; i++) {
					$('.friend-box').append('<p class="friend" id="' + result[i].id + '">' + result[i].firstName + ' ' + result[i].lastName + '</p>');
				}
			}
		});
	});

	$('body').on('click', '.friend', function (e) {
		var id = e.target.id;
		if (idString === "") {
			idString = id;
		}
		else {
			idString += ',' + id;
		}
		$('#FriendIds').val(idString);
		console.log(idString)
	});

	/*
	SPLIT/EXPENSE
	*/
	if ($('#splitExpense').length > 0) {
		var eventId = $('#SelectedEvent').val();

		$.ajax({
			url: "/Json/GetUsersByEventId",
			type: "GET",
			data: {
				id: eventId
			},
			success: function (result) {
				for (var i = 0; i < result.length; i++) {
					$('#friendListBox').append('<input type="checkbox" name="payerList" value="' + result[i].id + '" checked />' + result[i].firstName + ' ' + result[i].lastName);
					if (eventIdString === "") {
						eventIdString = result[i].id;
					}
					else {
						eventIdString += ',' + result[i].id;
					}
				}
				$('#FriendIds').val(eventIdString);
			}
		});

	}
	$('#SelectedEvent').change(function () {
		$('#friendListBox').html("");
		var eventId = $('#SelectedEvent').val();
		eventIdString = "";

		$.ajax({
			url: "/Json/GetUsersByEventId",
			type: "GET",
			data: {
				id: eventId
			},
			success: function (result) {
				for (var i = 0; i < result.length; i++) {
					$('#friendListBox').append('<input type="checkbox" name="payerList" value="' + result[i].id + '" checked />' + result[i].firstName + ' ' + result[i].lastName);
					if (eventIdString === "") {
						eventIdString = result[i].id;
					}
					else {
						eventIdString += ',' + result[i].id;
					}
				}
				$('#FriendIds').val(eventIdString);
			}
		});
	});
});