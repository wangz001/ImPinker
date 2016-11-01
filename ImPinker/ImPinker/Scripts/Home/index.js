$(document).ready(function () {
    //设置第一页数据
    if (jsonData != "") {
        var data = jsonData;
        setCard(data);
    }

    //加载下一页数据
    $("#loadmore").bind('click', function () {
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
                    var list = JSON.parse(data);
                    setCard(list);
                }
                $("#f_loadimg").hide();
            },
            error: function (data) {
                $("#f_loadimg").hide();
                alert(data);
            }
    });
    }
    //通过模板填充数据
    function setCard (dataJson) {
        if (dataJson != null && dataJson.length > 0) {
            //获取模板上的HTML
            var html = $('script[type="text/template1"]').html();
            //定义一个数组，用来接收格式化合的数据
            var arr = [];
            //对数据进行遍历
            $.each(dataJson, function (i, o) {
                //这里取到o就是上面rows数组中的值, formatTemplate是最开始定义的方法.
             arr.push(html.temp(o));
            });
            $('#articleConten').append(arr.join(''));
            //绑定事件
            bindVote();
        }
    };
});