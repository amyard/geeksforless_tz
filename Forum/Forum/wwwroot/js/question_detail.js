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