﻿$(document).ready(function () {
    //设置第一页数据
    if (jsonData != ""&&jsonData.length>0) {
        var data = jsonData;
        setCard(data);
    }

    //加载下一页数据
    $("#a_getmorenews").bind('click', function () {
        pageNum = pageNum + 1;
        getNextPage(pageNum, pageCount);
    });
    function getNextPage(pageNum, pageCount) {
        var key = $.getUrlParam("key");
        $.ajax({
            url: "/Search/GetNextPage",
            type: "get",
            data: { key: key, pageNum: pageNum, pageCount: pageCount },
            success: function (data) {
                if (data == null || data == "") {
                    $("#loadmore").parent().hide();
                } else {
                    var list = JSON.parse(data);
                    setCard(list);
                }
            },
            error: function (data) {
                alert(data);
            }
        });
    }
    //通过模板填充数据
    function setCard(dataJson) {
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
        }
    };

    //点击搜索
    $("#btn_search").bind("click", function() {
        var key = $("#search_txt").val();
        window.location = ("/Search/Index?key="+key);
    });
});