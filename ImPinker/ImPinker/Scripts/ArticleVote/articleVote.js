$(document).ready(function() {
    //页面加载完成，绑定vote按钮事件
    
});

function bindVote() {
    $(".b_zxotuij").unbind("click");//避免重复绑定
    $(".b_zxotuij").bind("click", function () {
        if (!LoginState) {
            alert("请先登录~");
            return;
        }
        var articleId = $(this).attr("articleid");
        var vote = $(this).attr("vote");
        var voteCount = $(this).find("cite")[0].innerText;
        $(this).find("cite").html(Number(voteCount) + 1);
        $.ajax({
            url: "/ArticleVote/UserVote",
            type: "get",
            data: { articleId: articleId, vote: vote },
            success: function (data) {
            },
            error: function (data) {
                alert(data);
            }
        });
    });
}