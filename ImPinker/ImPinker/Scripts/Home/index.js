$(document).ready(function () {
   
    //加载下一页数据
    $("#load-more-topic").bind('click', function () {
        $("#f_loadimg").show();
        pageIndex = pageIndex + 1;
        getNextPage(pageCount, pageIndex);
    });
    function getNextPage(pageCount, pageIndex) {
        $.ajax({
            url: "/Home/GetNextPage",
            type: "get",
            data: { pageCount: pageCount, pageNum: pageIndex },
            success:function(data) {
                if (data==null||data==="") {
                    $("#loadmore").parent().hide();
                } else {
                    $('#articleConten').append(data);
                }
                $("#f_loadimg").hide();
            },
            error: function (data) {
                $("#f_loadimg").hide();
                alert(data);
            }
    });
    }
});