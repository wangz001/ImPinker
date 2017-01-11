//在首页index.js 中，填充数据处绑定
function bindVote() {
    $(".praise-count").unbind("click");//避免重复绑定
    $(".praise-count").bind("click", function () {
        if (!LoginState) {
            alert("请先登录~");
            return;
        }
        
        var articleId = $(this).attr("articleid");
        $.ajax({
            url: "/ArticleVote/UserVote",
            type: "post",
            data: { articleId: articleId, vote: 1 },
            success: function (data) {
                if (data.IsSuccess == 1) {
                    showTips("谢谢点赞~~~~~", 1500, 1);
                } else {
                    showTips("您已赞过该篇文章~~", 1500, 2);
                }
                
            },
            error: function (data) {
                showTips("Sorry，系统出错了~", 1500, 0);
            }
        });
    });
}