//在首页index.js 中，填充数据处绑定。。。。文章收藏
function bindCollect(obj, articleId) {
    if (!LoginState) {
        alert("请先登录~");
        return;
    }
    var isCollect = $(obj).attr("class");
    var url = "";
    if (isCollect == "click-collect") {
        // 添加收藏
        url = "/ArticleCollection/AddCollect";
    } else {
        // 取消收藏
        url = "/ArticleCollection/RemoveCollect";
        showTips("您已收藏过该文章~", 1500, 2);
        $(obj).attr("class", "click-collect-ok")
        return;
    }
    $.ajax({
        url: url,
        type: "post",
        data: { articleId: articleId },
        async: true,
        success: function (data) {
            if (data.IsSuccess == "1") {
                showTips("ok，收藏成功~", 1500, 1);
                $(obj).attr("class", "click-collect-ok")
            } else {
                showTips("您已收藏过该文章~", 1500, 2);
                $(obj).attr("class", "click-collect-ok")
            }
        },
        error: function (data) {
            showTips("Sorry，出错咯~", 1500, 0);
        }
    });
}
//取消收藏
function unCollect(obj,articleId) {
    var url = "/ArticleCollection/RemoveCollect";
    $.ajax({
        url: url,
        type: "post",
        data: { articleId: articleId },
        async: true,
        success: function (data) {
            if (data.IsSuccess == "1") {
                $(obj).closest("li").remove();
                showTips("取消收藏成功~", 1500, 1);
            } else {
                
            }
        },
        error: function (data) {
            showTips("Sorry，出错咯~", 1500, 0);
        }
    });
}