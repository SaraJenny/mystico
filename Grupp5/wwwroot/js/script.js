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

	$('#Friends').keypress(function (e) {
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
					$('.friend-box').append('<p>Id: ' + result[i].id + '</p><p>Namn: ' + result[i].firstName + ' ' + result[i].lastName + '</p><p>E-post: ' + result[i].email + '</p>');
				}
			}
		});
	});
});