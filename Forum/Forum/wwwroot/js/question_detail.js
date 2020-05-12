function DeleteMainComment(ids) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will be not able to restore the data.",
        icon: "warning",
        buttons: true,
        dangetModel: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: `/Forum/Home/DeleteMainComment/${ids}`,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $(`.mc_${ids}`).remove();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}



function DeleteSubComment(ids) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will be not able to restore the data.",
        icon: "warning",
        buttons: true,
        dangetModel: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: `/Forum/Home/DeleteSubComment/${ids}`,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $(`.sc_${ids}`).remove();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}



/*   working with modal   */
$('.edit-btn-mail').click(function (event) {
    event.preventDefault();
    msg = jQuery(event.target).parent().parent().next().html()
    $("#myModal .trumbowyg-editor").html(msg)
    $('#myModal').modal();

    var commentId = jQuery(event.target).attr("data-id");
    var userId = jQuery(event.target).attr("data-user");

    $(document).on("click", ".update-modal", function (event) {
        event.preventDefault();
        var message = $("#myModal .trumbowyg-editor").html()
        data = {
            Id: commentId,
            ApplicationUserId: userId,
            Message: message
        }

        $.ajax({
            type: "POST",
            url: `/Forum/Home/UpdateMainComment/`,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(data),
            success: function (data) {
                if (data.success) {
                    console.log('AAAAAAAAAAAAAAAAAAAAAAAAAAAAA')
                    // toastr.success(data.message);
                } else {
                    console.log('Errorrrrrrrrrrrrrrrr')
                    // toastr.error(data.message);
                }
            }
        })

    })
})
