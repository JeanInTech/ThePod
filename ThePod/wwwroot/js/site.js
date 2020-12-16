var limit = 3;
$('input.single-checkbox').on('change', function (evt) {
    if ($(this).siblings(':checked').length >= limit) {
        this.checked = false;
    }
});


function searchByTags() {
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("SearchByTags");
    filter = input.value.toUpperCase();
    table = document.getElementById("reviewsTable");
    tr = table.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[4];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}