﻿function commentClick() {
    var articleId = $("#articleId").val();
    articleId = articleId.replace("travels_", "");
    var content = $("#content").val();
    $.ajax({
        url: "/ArticleVote/ArticleCommentSubmit",
        type: "post",
        data: { articleId: articleId, content: content },
        success: function (data) {
            alert("bingo！");
        },
        error: function (data) {
            alert(data);
        }
    });

}