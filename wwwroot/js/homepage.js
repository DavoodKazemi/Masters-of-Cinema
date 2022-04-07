/*Start Popular reviews section*/

//Read more / read less
$(document).ready(function () {
    var showChar = 700;
    var ellipsestext = "...";
    var moretext = "more";
    var lesstext = "less";
    $('.read-more-object-text').each(function () {
        var content = $(this).html();

        if (content.length > showChar) {

            var c = content.substr(0, showChar);
            var h = content.substr(showChar - 1, content.length - showChar);

            var html = c + '<span class="moreellipses">' + ellipsestext + '&nbsp;</span><span class="morecontent read-more-review"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink read-more-link">' + moretext + '</a></span>';

            $(this).html(html);
        }

    });

    var $container = $('#container-masonary');

    /*Activate masonary plugin for a grid with different heights*/
    var reMasonry = function () {
    $container.masonry();
    };

    /*When user clicks on more/less */
    $('.morelink').click(function () {

    //Change the height of the grid dynamically
    var $this = $(this).closest(".box-masonary"),
        size = $this.hasClass('large') ?
            { height: "fit-content" } :
            { height: "fit-content" };
    $this.toggleClass('large').animate(size, 12, reMasonry);

    /*Toggle the button's htm to less / more*/
    if ($(this).hasClass("less")) {
        $(this).removeClass("less");
        $(this).html(moretext);
    } else {
        $(this).addClass("less");
        $(this).html(lesstext);
    }
    $(this).parent().prev().toggle();
    $(this).prev().toggle();
    return false;
    });

});

$(function () {

    var $container = $('#container-masonary');

    // trigger masonry
    $container.masonry({
    animate: true
    });
});

//Start Like/Unlike a review

/*<#review-like-button> the like button
    *  <#like-icon> the heart icon
    *  <#like-caption> liked/like text
    </> End the like button
*/
/*<#likers-head-wrapper> the likers section
    *  <#like-count> the count
    *  <#review-stats> x people .. text
    </> End the likers section
*/
$(document).on("click", "#review-like-button", function (e) {
    //Start changing UI
    //*****toggle between two class for changing the icon color
    $(this).children("#like-icon").toggleClass('review-icon-like review-icon-liked');

    //****change the class of caption
    $(this).children("#like-caption").toggleClass('review-like-text review-liked-text');

    //*****if review is liked (or unliked), change the text
    if ($(this).children('#like-caption').hasClass('review-liked-text')) {
    //if (...) ==> User liked review
    //******change the caption to "Liked"
    $(this).children("#like-caption").html("Liked");

    //******Increase the count by 1
    $(this).siblings().children("#like-count").html(parseInt($(this).siblings().children("#like-count").html()) + 1);
    //******display the stats anyway, because if you pressed like, the like count is certainly more than zero
    //$(this).siblings("#likers-head-wrapper").fadeIn(400);

    } else {
    //else ==> User unliked review
    //******change the caption to "Like Review"
    $(this).children("#like-caption").html("Like Review");

    //hide the stats if the like count is gonna be zero (after dcreasing)
    if (parseInt($(this).siblings().children("#like-count").html()) < 2) {
                $(this).siblings("#likers-head-wrapper").fadeOut(400);
    }

    //******Decrease the count by 1
    $(this).siblings().children("#like-count").html(parseInt($(this).siblings().children("#like-count").html()) - 1);


    }
    //END Start changing UI

    //Start Ajax function for saving like or removing like from database
    var id = $(this).children("#review-id").val();
    var data = "ReviewId=" + id;

    //alert(data);
    $.ajax({

        type: 'POST',
        url: '/Review/LikeReview',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
        data: data,
        success: function (result) {
            //alert('Successfully received Data ');
            console.log(result);
        },
        error: function () {
            //alert('Failed to receive the Data');
            console.log('Failed ');
        }
    })
    //End Ajax function

});
//End Like/Unlike a review
/*End Popular reviews section*/

