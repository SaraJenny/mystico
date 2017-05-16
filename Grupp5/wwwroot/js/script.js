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

		// Om sökningen görs på redan inlagt event (split/overview eller split/updateEvent)
		if ($('.addFriendsBox').length > 0) {
			var eventId = $('.addFriendsBox').attr('eventid');
			$.ajax({
				url: "/Json/SearchAllUsersExceptAlreadyParticipantsAndChosen",
				type: "GET",
				data: {
					search: string,
					chosen: chosenFriendIds,
					eventid: eventId
				},
				success: function (result) {
					for (var i = 0; i < result.length; i++) {
						$('#friend-box').append('<a href="#" class="friend not-choosen" id="' + result[i].id + '">' + result[i].firstName + ' ' + result[i].lastName + '</a>');
					}
				}
			});
		}
		// Om sökningen görs på nytt event (split/event)
		else {
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
		}

	});
	/* Klick på en väns namn bland de sökta */
	$('body').on('click', '.not-choosen', function (e) {
		var id = e.target.id; // TODO byt till userId istället för id i html
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

	/*
	Ta bort en väns namn bland de utvalda
	*/
	$('body').on('click', '.choosen', function (e) {
		var id = e.target.id; // TODO byt till userId istället för id i html
		removeChoosenFriend(id);
	});
	$('body').on('click', '.deleteX', function (e) {
		var id = $(this).parent().attr('id');
		removeChoosenFriend(id);
	});
	function removeChoosenFriend(id) {
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
	}

	// Hämta standardcurrency och sätt till selected
	function setStandardCurrency(eventId) {
		$.ajax({
			url: "/Json/GetStandardCurrencyByEvent",
			type: "GET",
			data: {
				id: eventId
			},
			success: function (result) {
				$('#SelectedCurrency').val(result);
			}
		});
	}


	if ($('#updateEventForm').length > 0) {
		var eventId = $('#updateEventForm').attr('eventid');
		setStandardCurrency(eventId);
	}
	/*
	SPLIT/EXPENSE & SPLIT/UPDATEEXPENSE
	*/
	if ($('#splitExpense').length > 0 || $('#updateExpenseForm').length > 0) {
		eventId = $('#SelectedEvent').val();
		setStandardCurrency(eventId);

		if ($('#splitExpense').length > 0) {
			// Hämta vänner
			$.ajax({
				url: "/Json/GetUsersByEventId",
				type: "GET",
				data: {
					id: eventId
				},
				success: function (result) {
					for (var i = 0; i < result.length; i++) {

						$('#friendListBox').append('<div><input class="friendCheckbox" type="checkbox" name="payerList" value="' + result[i].id + '" checked />' + result[i].firstName + ' ' + result[i].lastName + '</div>');
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
		else {

			var expenseId = $('#updateExpenseForm').attr('expenseid');
			var output = '';
			// Hämta vänner
			$.ajax({
				url: "/Json/GetUsersByExpense",
				type: "GET",
				data: {
					id: expenseId
				},
				success: function (result) {
					for (var i = 0; i < result.length; i++) {
						output += '<div><input class="friendCheckbox" type="checkbox" name="payerList" value="' + result[i].id + '"';
						// Om personen är satt till payer: bocka för checkboxen + lägg till i FriendIds
						if (result[i].isPayer) {
							if (userIdString === "") {
								userIdString = result[i].id;
							}
							else {
								userIdString += ',' + result[i].id;
							}
							output += ' checked';
						}
						output += '/>' + result[i].firstName + ' ' + result[i].lastName + '</div > ';
					}
					$('#friendListBox').append(output);
					$('#FriendIds').val(userIdString);
				}
			});
		}

	}
	/* Sker då användaren ändrar valt event */
	$('#SelectedEvent').change(function () {
		var eventId = $('#SelectedEvent').val();
		// sätt valuta till standardvaluta för eventet
		setStandardCurrency(eventId);

		$('#friendListBox').html("");
		userIdString = "";

		$.ajax({
			url: "/Json/GetUsersByEventId",
			type: "GET",
			data: {
				id: eventId
			},
			success: function (result) {
				for (var i = 0; i < result.length; i++) {
					$('#friendListBox').append('<div><input class="friendCheckbox" type="checkbox" name="payerList" value="' + result[i].id + '" checked />' + result[i].firstName + ' ' + result[i].lastName + '</div>');
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
	SPLIT/UPDATEEXPENSE
	*/
	//if ($('#updateExpenseForm').length > 0) {
	//	userIdString = '';
	//	$('input:checked').each(function () {
	//		var userId = $(this).val();
	//		if (userIdString === '') {
	//			userIdString = userId;
	//		}
	//		else {
	//			userIdString += ',' + userId;
	//		}
	//	});
	//	$('#FriendIds').val(userIdString);
	//}
	/*
	SPLIT/OVERVIEW
	*/
	// Visa formulär för att lägga till vänner till eventet
	$('#addFriendsButton').click(function (e) {
		e.preventDefault();
		$('#addFriendsBox').show();
		$(this).hide();
	});
	// Dölj formulär för att lägga till vänner
	$('#closeFriends').click(function (e) {
		e.preventDefault();
		$('#addFriendsButton').show();
		$('#addFriendsBox').hide();
	});
	// Ändra cirkelns storlek dynamiskt
	if ($('#circleSection').length > 0) {
		var circleLength = $('#circle').text().length;
		if (circleLength > 6) {
			var circleSize = circleLength * 25;
			$('.circle').css('width', circleSize);
			$('.circle').css('height', circleSize);
			$('.circle .focus').css('line-height', circleSize + 'px');
		}
	}
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
	/*******************************
	SPLIT/EVENT
	*******************************/
	/*
	Stäng event
	*/
	$('#deleteEventButton').click(function (e) {
		e.preventDefault();
		eventId = $('#addFriendsBox').attr('eventid');
		if (confirm('Vill du stänga eventet?')) {
			window.location.replace('/Split/DeleteEvent/' + eventId);
		}
		else {
			return false;
		}
	});
	/*******************************
	SPLIT/DETAILS
	*******************************/
	/*
	Radera expense
	*/
	$('.deleteExpenseButton').click(function (e) {
		e.preventDefault();
		var expenseId = $(this).attr('expenseid');
		if (confirm('Är du säker på att du vill radera utlägget?')) {
			window.location.replace('/split/deleteexpense/' + expenseId);
		}
		else {
			return false;
		}
	});
	/*******************************
	SPLIT/OVERVIEW
	*******************************/
	// Se/dölj alla transaktioner
	$('#allTransactionsButton').click(function (e) {
		e.preventDefault();
		$('#transactionsWithoutMe').toggle();
		if ($('#allTransactionsText').text() === 'Se alla överföringar') {
			$('#allTransactionsText').text('Dölj överföringar');
		}
		else {
			$('#allTransactionsText').text('Se alla överföringar');
		}
	});

	// Se mer info om person
	$('.person').click(function () {
		var userId = $(this).attr('userId');
		var firstname = $(this).text();
		var lastname = $(this).attr('lastname');
		var email = $(this).attr('email');
		console.log(firstname + ' ' + lastname + ' ' + email);
		// TODO visa denna info
	});
});