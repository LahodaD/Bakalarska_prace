function openErrorDialog(strMessage) {
    var myDiv = document.getElementById('MyModalErrorAlert');
    myDiv.innerHTML = strMessage;
    $('#myModalError').modal('show');
}

$(document).ready(function () {
    var msg = "@TempData["ErrorMessage"]";
    if (msg) {
        openErrorDialog(msg);
    }
});