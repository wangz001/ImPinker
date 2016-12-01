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
    $.ajax({
        url: "/ArticleVote/ArticleCommentSubmit",
        type: "post",
        data: { articleId: articleId, content: content },
        success: function (data) {
            window.location.reload();
        },
        error: function (data) {
            alert(data);
        }
    });

}