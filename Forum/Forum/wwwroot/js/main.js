function deleteAction(area, model, ids) {
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
                url: `/${area}/${model}/Delete/${ids}`,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);

                        // Category delete
                        if ($(".actionAdminTable").length != 0) {
                            $(`#actionAdminTable-th-id_${ids}`).parent().remove();
                        } else {
                            // delete Question
                            $.each($("td.sorting_1"), function (item, value) {
                                jQuery(value).html() == ids
                                    ? jQuery(value).parent().remove()
                                    : null
                            })
                        }

                        // delete post
                        if (area == "Forum" && model == "Home") {
                            window.location.href = window.location.origin;
                        }  
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}