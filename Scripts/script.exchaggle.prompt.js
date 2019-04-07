$('.confirmation-delete-item').on('click', function (event) {
    event.preventDefault();
    getRedirect("Delete?", "This will remove all associated Trades and cannot be undone.", ($(this).attr("href") || "/"));
});
$('.confirmation-remove').on('click', function (event) {
    event.preventDefault();
    getRedirect("Remove?", "This cannot be undone.", ($(this).attr("href") || "/"));
});
$('.confirmation-offer-confirm').on('click', function (event) {
    event.preventDefault();
    getRedirect("Confirm Trade?", "Do you want to accept this offer? All other offers will be removed.", ($(this).attr("href") || "/"));
});
$('.confirmation-redirect').on('click', function (event) {
    event.preventDefault();
    getRedirect("Continue?", "This action will redirect you to a new page.", ($(this).attr("href") || "/"));
});

function getRedirect(promptTitle, promptText, promptLink) {
    swal({
        title: '<span style="color:white">' + promptTitle + '</span>',
        html: '<span style="color:white">' + promptText + '</span>',
        showCancelButton: true,
        confirmButtonColor: '#1177ff',
        cancelButtonColor: '#3b6897',
        background: "#275482"
    }).then(function (isConfirm) {
        if (isConfirm) {
            window.location.href = promptLink;
        }
    })
}