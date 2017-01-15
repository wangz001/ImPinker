//在首页index.js 中，填充数据处绑定。。。。文章收藏
function bindCollect(obj, articleId) {
    if (!LoginState) {
        alert("请先登录~");
        return;
    }
    var isCollect = $(obj).attr("class");
    
    alert(isCollect);
    var url = "";
    if (isCollect == "click-collect") {
        // 添加收藏
        url = "/ArticleCollection/AddCollect";
    } else {
        // 取消收藏
        url = "/ArticleCollection/RemoveCollect";
    }
    $.ajax({
        url: url,
        type: "post",
        data: { articleId: articleId },
        async: true,
        success: function (data) {
            if (data.IsSuccess == "1") {
                $(obj).find("em").html(count + 1);
                VoteAnimate(obj);
                showTips("ok，收藏成功~", 1500, 1);
                $(obj).attr("class", "click-collect-ok")
            } else {
                showTips("您已经评价过该评论~", 1500, 2);
            }
        },
        error: function (data) {
            showTips("Sorry，出错咯~", 1500, 0);
        }
    });
}