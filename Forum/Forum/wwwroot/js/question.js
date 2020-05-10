var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Post/GetPostForAdmin"
        },
        "columns": [
            { 'data': "id", "width": "5%" },
            { 'data': "title", "width": "25%" },
            { 'data': "category.title", "width": "10%" },
            {
                'data': "imageUrl",
                "render": function (data, type, row) {
                    return '<img src="' + data + '" style="height: 80px;"/>'
                },
                "width": "10%"
            },
            { 'data': "applicationUser.fullName", "width": "10%" },
            {
                'data': "created",
                "render": function (data) {
                    return convertDate(data);
                },
                "width": "10%"
            },
            {
                "data": "modified",
                "render": function (data) {
                    return convertDateModified(data);
                },
                "width": "10%"
            },
            {
                'data': "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Post/AddOrEdit/${data}" class="btn btn-outline-dark">Edit</a>
                        <a href="/Admin/Post/Details/${data}" class="btn btn-outline-primary">Details</a>
                        <a class="btn btn-outline-danger" onclick='deleteAction("Admin", "Post", ${data})'>Delete</a>
                        `
                },
                "width": "20%"
            },
        ]
    });
}



function convertDate(data) {
    return new Date(data).toISOString().slice(0, 16).replace('T', ' ');
}

function convertDateModified(data) {
    var res = data == '0001-01-01T00:00:00'
        ? ""
        : convertDate(data)
    return res;
}