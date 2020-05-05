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


/*************************************************/
/**********      USER CONTROLLER         *********/
/*************************************************/
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "email", "width": "20%" },
            {
                "data": "imageUrl",
                "render": function (data, type, row) {
                    return '<img src="/' +data+ '" style="height: 80px;"/>'
                },
                "width": "20%"
            },
            { "data": "role", "width": "20%" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        // user is curreckly locked
                        return `
                        <div class="text-center">
                            <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width: 100%;">
                                <i class="fas fa-lock-open"></i> Unlock
                            </a>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center">
                            <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width: 100%;">
                                <i class="fas fa-lock"></i> Lock
                            </a>
                        </div>
                        `;
                    }
                }, "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            } else {
                toastr.error(data.message);
            }
        }
    })
}