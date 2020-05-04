function deleteAction(model, ids) {
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
                url: `/Forum/${model}/Delete/${ids}`,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $(`#actionAdminTable-th-id_${ids}`).parent().remove();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}