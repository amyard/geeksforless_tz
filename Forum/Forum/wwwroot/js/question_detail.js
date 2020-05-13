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

    var commentId = jQuery(this).attr("data-id");
    $(".main-comm-modal").val(   jQuery(event.target).attr("data-id")  );
})



$('.edit-btn-sub').click(function (event) {
    event.preventDefault();
    msg = jQuery(event.target).parent().parent().next().html()
    $("#subModal .trumbowyg-editor").html(msg)
    $('#subModal').modal();

    $(".sub-comm-modal").val(jQuery(event.target).attr("data-id"));
    $(".main-comm-modal").val(jQuery(event.target).attr("data-mainId"));
})
