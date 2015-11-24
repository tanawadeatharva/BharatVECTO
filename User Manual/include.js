<script type="text/javascript">$(function () {
    var hash = window.location.hash;
    if (hash) {
        $(hash.replace(".", "\\.")).show();
        $(hash.replace(".", "\\.")).parents().show();
        $(window).scrollTop(0);
    } else $("#user-manual").show();

    
    $('a').click(function() {
        $("body > div:not(#TOC):not(#HEADER):not(#FOOTER)").hide();
        $($(this).attr("href").replace(".", "\\.")).show();
        $($(this).attr("href").replace(".", "\\.")).parents().show();
        window.scrollTo(0, 0);
    });

    $("#TOC").resizable({
        handles: "e",
        resize: function(event, ui) {
            $("body > div:not(#TOC):not(#HEADER):not(#FOOTER)").css("padding-left", ui.size.width);
        }
    });
    $("#TOC").scroll(function() {
        $(".ui-resizable-handle").css('top', $("#TOC").scrollTop());
    });
});</script>