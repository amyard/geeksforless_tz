var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Forum/Post/GetPostForAdmin"
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
            { 'data': "created", "width": "10%" },
            { 'data': "modified", "width": "10%" },
            {
                'data': "id",
                "render": function (data) {
                    return `
                        <a href="/Forum/Post/AddOrEdit/${data}" class="btn btn-outline-dark">Edit</a>
                        <a href="/Forum/Post/AddOrEdit/${data}" class="btn btn-outline-primary">Details</a>
                        <a class="btn btn-outline-danger" onclick='deleteAction("Post", ${data})'>Delete</a>
                        `
                },
                "width": "20%"
            },
        ]
    });
}
