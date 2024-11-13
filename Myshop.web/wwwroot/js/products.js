var datatable;
$(document).ready(function () {
    loadData();
});
function loadData()
{
    datatable = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price", "className": "text-left" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Product/Edit/${data}" class="btn btn-success">Edit</a>
                        <a onClick=DeletedItem("/Admin/Product/DeleteProduct/${data}") class="btn btn-danger">Delete</a>

                    `;
                }
            }
        ]
    });
}


function DeletedItem(url)
{
    Swal.fire({
        title: "Are you sure?",
        text: "You want to Delete this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        datatable.ajax.reload();
                        toaster.success(data.message);
                    }
                    else {
                        toaster.error(data.message);
                    }
                }
            });
            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });
}
