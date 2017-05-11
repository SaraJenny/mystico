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

	$('#friendsTextBox').keypress(function (e) {
		// töm vänboxen
		$('.friend-box').html("");
		string += e.originalEvent.key;
		$.ajax({
			url: "/Home/Temp",
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
});