
//Post review - Only available when user is logged in
$(document).on("click", "#submit-review", function (e) {


    var data = $("#submitForm").serialize();
    console.log(data);

    //alert(data);
    $.ajax({

        type: 'POST',
        url: '/Movie/Review',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
        data: data,
        success: function (data, result) {
            //alert('Successfully received Data ');
            console.log(result);
            if (data != '') {
                $("#submitForm").hide();
                $(".review-ajax-container > #post-review").append(data);

                //scroll to the review
                $("html").animate(
                    {
                        scrollTop: $("#user-review-section").offset().top
                    },
                    800 //speed
                );
                //notify user
                console.log("Your review saved!");

                $("#jnotify-message").empty().append("<div>You review of <strong>" + $(".movie-header-title").text() + "</strong> is saved!</div>");
                $("#clist-add-notify").delay(1400).slideDown(320);
                $('#clist-add-notify').delay(5000).slideUp(320);
                //end notify user

                $("#post-review").show('slow');
                $("#no-review-yet").hide("slow");

            }
        },
        error: function () {
            //alert('Failed to receive the Data');
            console.log('Failed ');
        }
    })
});