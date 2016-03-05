<script type="text/javascript">
$(function(){
    var OnClick = function() {
        showContent($($(this).attr("href").replace(".", "\\.")));
    };
    
    var showContent = function(element){
        $("#CONTENT").html(element.html());
        $("#CONTENT > a").click(OnClick);
        $("#CONTENT").show();
    };
    

    $("body > div:not(#TOC):not(#HEADER):not(#FOOTER):not(.vecto2):not(.vecto3)").hide();
    $("body > div:not(#TOC):not(#HEADER):not(#FOOTER) > div:not(.vecto2):not(.vecto3)").hide();

    window.onhashchange=function(){showContent($(window.location.hash.replace(".", "\\.")));};
    if (window.location.hash) {
        showContent($(window.location.hash.replace(".", "\\.")));
    } else {
        showContent($("#user-manual"));
    }
    
    $("#TOC").resizable({
        handles: "e",
        resize: function(event, ui) {
            $("body > div:not(#TOC):not(#HEADER):not(#FOOTER)").css("padding-left", ui.size.width);
        }
    });
    $("#TOC").scroll(function() {
        $(".ui-resizable-handle").css('top', $("#TOC").scrollTop());
    });


});
</script>