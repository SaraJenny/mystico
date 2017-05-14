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
	/* Döljer menyn vid klick på huvudsidan */
	$('main').click(function () {
		$('#toggle-nav').addClass('active');
		$('#mainmenu ul').addClass('active');
	});
	/*
	LÄGG TILL VÄNNER
	*/
	var string = "";
	var idString = ""; // SPLIT/EVENT
	var userIdString = ""; // SPLIT/EXPENSE

	/*
	SPLIT/EVENT
	*/
	$('#friendsTextBox').on('input', function (e) {
		// töm vänboxen
		$('#friend-box').html("");
		string = $('#friendsTextBox').val();
		var chosenFriendIds = $('#FriendIds').val();

		$.ajax({
			url: "/Json/SearchUserExceptMe",
			type: "GET",
			data: {
				search: string,
				chosen: chosenFriendIds
			},
			success: function (result) {
				for (var i = 0; i < result.length; i++) {
					$('#friend-box').append('<a href="#" class="friend not-choosen" id="' + result[i].id + '">' + result[i].firstName + ' ' + result[i].lastName + '</a>');
				}
			}
		});
	});
	/* Klick på en väns namn bland de sökta */
	$('body').on('click', '.not-choosen', function (e) {
		var id = e.target.id;
		// Lägg till userId i hiddenfältet
		if (idString === "") {
			idString = id;
		}
		else {
			idString += ',' + id;
		}
		$('#FriendIds').val(idString);
		//Flytta vännen från sökrutan till valda vänner + lägg till kryss till namnet
		$('#' + id).detach().appendTo('#choosenFriends').append('<span class="deleteX">x</span>');
		// ta bort klass och lägg till en annan
		$('#' + id).removeClass('not-choosen').addClass('choosen');
		// töm textboxen
		$('#friendsTextBox').val('');
		// töm vänboxen
		$('#friend-box').html("");
	});

	/* Ta bort en väns namn bland de utvalda */
	$('body').on('click', '.choosen', function (e) {
		var id = e.target.id;
		idString = $('#FriendIds').val();
		// Ta bort id i hiddenfältet
		if (idString.includes(',' + id)) {
			idString = idString.replace(',' + id, '');
		}
		else if (idString.includes(id + ',')) {
			idString = idString.replace(id + ',', '');
		}
		else {
			idString = idString.replace(id, '');
		}
		$('#FriendIds').val(idString);
		//Ta bort vännen från valda vänner till sökrutan
		$('#' + id).remove();
		// ta bort klass och lägg till en annan
		$('#' + id).removeClass('choosen').addClass('not-choosen');
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
					$('#friendListBox').append('<input class="friendCheckbox" type="checkbox" name="payerList" value="' + result[i].id + '" checked />' + result[i].firstName + ' ' + result[i].lastName);
					if (userIdString === "") {
						userIdString = result[i].id;
					}
					else {
						userIdString += ',' + result[i].id;
					}
				}
				$('#FriendIds').val(userIdString);
			}
		});

	}
	/* Sker då användaren ändrar valt event */
	$('#SelectedEvent').change(function () {
		$('#friendListBox').html("");
		var eventId = $('#SelectedEvent').val();
		userIdString = "";

		$.ajax({
			url: "/Json/GetUsersByEventId",
			type: "GET",
			data: {
				id: eventId
			},
			success: function (result) {
				for (var i = 0; i < result.length; i++) {
					$('#friendListBox').append('<input class="friendCheckbox" type="checkbox" name="payerList" value="' + result[i].id + '" checked />' + result[i].firstName + ' ' + result[i].lastName);
					if (userIdString === "") {
						userIdString = result[i].id;
					}
					else {
						userIdString += ',' + result[i].id;
					}
				}
				$('#FriendIds').val(userIdString);
			}
		});
	});
	/* Sker då användaren bockar av/i vänner */
	$('body').on('change', '.friendCheckbox', function () {
		var userId = $(this).val();
		var isChecked = $(this).prop('checked');
		userIdString = $('#FriendIds').val();

		if (isChecked) {
			if (userIdString === '') {
				userIdString = userId;
			}
			else {
				userIdString += ',' + userId;
			}
		}
		else {
			if (userIdString.includes(',' + userId)) {
				userIdString = userIdString.replace(',' + userId, '');
			}
			else if (userIdString.includes(userId + ',')) {
				userIdString = userIdString.replace(userId + ',', '');
			}
			else {
				userIdString = userIdString.replace(userId, '');
			}
		}
		$('#FriendIds').val(userIdString);
	});
	/*
	PROFILE
	*/
	// Visa lösenordsformuläret
	$('#updatePassword').click(function () {
		$('#passwordForm').toggle();
		if ($('#updatePassword').text() === 'Uppdatera lösenord') {
			$('#updatePassword').text('Dölj lösenordsformulär');
		}
		else {
			$('#updatePassword').text('Uppdatera lösenord');
		}
	});
});