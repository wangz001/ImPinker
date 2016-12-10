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
            type: "get",
            data: { articleId: articleId, vote: 1 },
            success: function (data) {
                $(".dialog-layer").show();
                $(".confirm").bind('click', function () {
                    $(".dialog-layer").hide();
                });
            },
            error: function (data) {
                alert(data);
            }
        });
    });
}