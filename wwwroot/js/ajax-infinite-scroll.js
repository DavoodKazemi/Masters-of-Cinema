//2 variables needs to be defined in the view:
//pageCount and controllerActionUrl

//when page get loaded
var page = 0,
    inCallback = false,
    hasReachedEndOfInfiniteScroll = false;

document.addEventListener("DOMContentLoaded", function () {

        $("div#loading-animation").hide();
        $("div#reached-end").hide();
        $(window).scroll(scrollHandler);

    //load more button
    if (hasReachedEndOfInfiniteScroll == false) {
        $(document).on('click', '#load-more-movies-button:not(.button-loading)', function () {
            loadMoreAjax(controllerActionUrl);
            $(this).addClass('button-loading');
            hasReachedEndOfInfiniteScroll = (page + 1) >= pageCount;
        });
    }    
});

//load with scrolling
var scrollHandler = function () {
    //if not reached end
    if (!hasReachedEndOfInfiniteScroll &&
        ($(window).scrollTop() == $(document).height() - $(window).height())) {
        loadMoreAjax(controllerActionUrl);
        $("div#loading-animation").show();
        $("#load-more-movies-button").hide();
    }
    hasReachedEndOfInfiniteScroll = (page + 1) >= pageCount;
}


function revealPosts() {
    var posts = $('.ajax-individual-item-wrapper:not(.reveal)');
    var i = 0;
    setInterval(function () {
        if (i >= posts.length) return false;

        var el = posts[i];
        $(el).addClass('reveal');

        i++;
    }, 120);
}


//start ajax function for infinite scroll

function loadMoreAjax(loadMoreRowsUrl) {
    if (!inCallback) {
        inCallback = true;

        //increase page counter
        page++;

        $.ajax({
            type: 'GET',
            //Controller function url
            url: loadMoreRowsUrl,

            data: "pageNum=" + page,
            success: function (data, textstatus) {
                setTimeout(function () {
                    console.log(hasReachedEndOfInfiniteScroll);
                    console.log(page);
                    if (data != '') {
                        $(".list-ajax-container > .tbody").append(data);
                    }
                    inCallback = false;
                    $("div#loading-animation").hide();
                    $('#load-more-movies-button').show();
                    $('#load-more-movies-button').removeClass('button-loading');
                    revealPosts();


                    //if reached end, hide button and show the message
                    if (hasReachedEndOfInfiniteScroll) {
                        $("div#reached-end").slideDown(320);
                        $("#load-more-movies-button").slideUp(320);
                    }

                }, 800);

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log('Failed ');
            }
        });
    }
}


//end ajax for infinite scroll