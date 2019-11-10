// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// When the user changes the timezone option...
$("#TZSelector").change(function() {
    var offset = 0;
    //...to 'local'
    if ($("#TZSelector option:selected").val() === "local") {
        //get the user's utc offset using the moment.js library
        offset = moment().utcOffset();
    }

    //and iterate over all time-class
    $(".time").each(function() {
        //get the original time from the aria-valuetext attribute
        ct = moment($(this).attr('aria-valuetext'), "D/M/YYYY h:mm:ss A");
        //add the user's utc offset
        $(this).html(moment(ct).add(offset, 'm').format("D/M/YYYY h:mm:ss A"));
    });
});