$(document).ready(function () {
    $('#shortenBtn').click(function () {
        var longUrl = $('#longUrl').val();
        $.ajax({
            type: 'POST',
            url: '/UrlPair/Create',
            contentType: 'application/json',
            data: JSON.stringify({ longUrl: longUrl }),
            success: function (response) {
                $('#shortenedUrl').attr('href', 'https://top-url-shortener.azurewebsites.net/' + response.shortUrl);
                $('#shortenedUrl').text('https://top-url-shortener.azurewebsites.net/' + response.shortUrl);
                $('#shortenedUrlContainer').show();
            },
            error: function (xhr, status, error) {
                if (xhr.status === 400) {
                    alert(xhr.responseText); // Display the error message sent from the server
                } else {
                    alert('An error occurred while shortening URL.'); // Default error message
                }
            }
        });
    });
});
