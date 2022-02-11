//2 variables needs to be defined in the view:
//pageCount and controllerActionUrl

//when page get loaded
document.addEventListener("DOMContentLoaded", function () {

        $("div#loading").hide();
        $("div#reached-end").hide();
        $(window).scroll(scrollHandler);
});

//start ajax for infinite scroll


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




var page = 0,
    inCallback = false,
    hasReachedEndOfInfiniteScroll = false;

var scrollHandler = function () {
    //if not reached end
    if (!hasReachedEndOfInfiniteScroll &&
        ($(window).scrollTop() == $(document).height() - $(window).height())) {
        loadMoreToInfiniteScrollTable(controllerActionUrl);
    }
    //if reached end
    if (hasReachedEndOfInfiniteScroll && !inCallback &&
        ($(window).scrollTop() == $(document).height() - $(window).height())) {
        $("div#reached-end").show();
        //console.log(hasReachedEndOfInfiniteScroll);
    }

    //reached end?
    hasReachedEndOfInfiniteScroll = (page + 1) >= pageCount;
}

function loadMoreToInfiniteScrollTable(loadMoreRowsUrl) {
    if (!inCallback) {
        inCallback = true;

        //increase page counter
        page++;

        //display loading message
        $("div#loading").show();

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
                    $("div#loading").hide();

                    revealPosts();

                }, 800);

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log('Failed ');

            }
        });
    }
}


//end ajax for infinite scroll