//发表评论
function commentClick() {
    if (!LoginState) {
        $("#loginmessage").show();
        return;
    }
    var articleId = $("#articleId").val();
    articleId = articleId.replace("travels_", "");
    var content = $("#content").val();
    if (content==null||content=='') {
        return;
    }
    dosubmin(articleId, 0, content);
}

function dosubmin(articleid,commentid,content) {
    $.ajax({
        url: "/ArticleVote/ArticleCommentSubmit",
        type: "post",
        data: { articleId: articleid, content: content,commentid:commentid },
        success: function (data) {
            window.location.reload();
        },
        error: function (data) {
            alert(data);
        }
    });
}

$(document).ready(function () {
    //回复某个评论
    $(".all-comment .reply-btn").bind('click', function () {
        if (!LoginState) {
            $("#loginmessage").show();
            return;
        }
        var articleId = $(this).attr("articleid");
        var commentId = $(this).attr("commentid");
        var html = $('script[type="text/templateReComment"]').html();
        $(this.parentElement.parentElement).append(html);
        $("#reply-content").focus();
        $("#pub-reply").bind("click", function() {
            var content = $("#reply-content").val();
            if (content != null && content != '') {
                dosubmin(articleId, commentId, content);
            }
        });
    });

    //给评论点赞
    bindArticleCommentVote();
});