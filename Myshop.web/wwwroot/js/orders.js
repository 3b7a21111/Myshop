var datatable;
$(document).ready(function () {
    loadData();
});
function loadData()
{
    datatable = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData",
            "dataSrc": "data"
        },
        "columns": [
            { "data": "id", "className": "text-left" },
            { "data": "name" },
            { "data": "phone", "className": "text-left" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "totalPrice", "className": "text-left" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Order/Details?orderid=${data}" class="btn btn-warning">Details</a>
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
