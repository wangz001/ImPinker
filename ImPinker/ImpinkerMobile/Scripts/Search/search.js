$(document).ready(function () {
    //点击搜索
    $("#btnsearch").bind("click", function () {
        var key = $("#txtsearch").val();
        window.location = ("/Search/Index?key="+key);
    });
});