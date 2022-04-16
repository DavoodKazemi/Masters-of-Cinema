
//start hide the suggestions list when user click outside the input
$(document).mouseup(function (e) {
    var container = $("#live-search-input-wrapper");

    // if the target of the click isn't the the input, hide the suggestions
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        $("#clist-live-search-suggest").hide();
    }
});
//End hide the suggestions list when user click outside the input


//Start
//When user press a key in the live search input, do a search for movies that contain the input search term
$(document).ready(function () {
    $("#search-input").keyup(function () {
        $('#live-search-input-wrapper').addClass('loading');

        var data = "searchTerm=" + $("#search-input").val();
        console.log(data);

        $.ajax({
            type: 'GET',
            url: '/Account/Search',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
            data: data,
            success: function (data, result) {
                //display the live search results
                $("#clist-live-search-suggest").show();
                if (data != '') {
                    $("#clist-live-search-suggest > .suggest-body").empty().append(data);
                }

                //hide the loading animation (with delay)
                setTimeout(function () {
                    $('#live-search-input-wrapper').removeClass('loading');
                }, 500);

                //console.log(result);
            },
            error: function () {
                console.log('Failed ');
            }
        })
    });
});
    //END

//Strat Remove added movies from the new list.
$(document).on("click", "#clist-delete-added-movie", function (e) {
    e.preventdefault;

    //If this is not the last movie
    if ($('[id=clist-delete-added-movie]').length > 1) {
        //console.log($('[id=clist-delete-added-movie]').length.toString() + " movies remained.");
        $(this).parent().fadeOut('slow', function () { $(this).remove(); });

        //If this is the last movie (remove it without animation)
    } else if ($('[id=clist-delete-added-movie]').length == 1) {
        //console.log("only 1 movie remained. it was the last!");
        $(this).parent().remove();
    }

    //if the removed movie was the last one, display the placeholder.
    if ($("#clist-delete-added-movie").length == 0) {
        //console.log("No movies! Only placeholder!");
        $("#empty-list-placeholder").slideDown(320);
    }
});
//End Remove added movies from the new list.


//Focus on the live search input, when user clicks on the button
function addMovie() {
    document.getElementById("search-input").focus();
}
    //End



//Start
//Clicks the movies to add it to this list
function AddMovie(id) {
    //If the movie is not added already
    if ($("." + id).length == 0) {
        var data = "movieIdToAdd=" + id;
        console.log(data);

        $.ajax({
            type: 'GET',
            url: '/Account/AddMovieToCList',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
            data: data,
            success: function (data, result) {
                //Display the movie user clicked on
                if (data != '') {
                    var new_div = $(data).hide();
                    $(".to-add-movies-container > .movies-to-add-bodey").append(new_div);
                    new_div.fadeIn('slow');
                }
                //Hide the placeholder, if it's not hidden already
                $("#empty-list-placeholder").slideUp(320);

                console.log(result);
            },
            error: function () {
                console.log('Failed ');
            }
        })

    }
    //if the movie is already added
    else {

        console.log("you already added this movie!");

        $("#jnotify-message").empty().append("<div><strong>" + $("." + id + " .movie-to-add-title").text() + "</strong> is already in this list</div>"
            + `<span class="notify-message-close-wrapper" id="notify-message-close-wrapper">
                <i class="notify-message-close-icon" id=""></i></span>`);
        $("#clist-add-notify").slideDown(320);
        $('#clist-add-notify').delay(2000).slideUp(320);
    }
}
//End


//Close button for the notifying messages when adding/editing a review
$(document).on("click", "#notify-message-close-wrapper", function (e) {
    $(this).closest("#clist-add-notify").hide();
});
//End Close button for the notifying messages