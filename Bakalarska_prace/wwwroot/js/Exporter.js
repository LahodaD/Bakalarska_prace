function showFileDialog() {
    var fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.style.display = 'none';
    fileInput.accept = 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'; // Přidání atributu accept pro .docx soubory
    fileInput.addEventListener('change', function (event) {
        var selectedFile = event.target.files[0]; // získání vybraného souboru
        uploadFile(selectedFile); // volání funkce pro nahrání souboru na server
    });
    document.body.appendChild(fileInput);
    fileInput.click();
    document.body.removeChild(fileInput);
}

function uploadFile(file, urlAction) {
    if (file) {
        var formData = new FormData(); // vytvoření FormData objektu pro odeslání souboru na server
        formData.append('fileInput', file); // přidání souboru do FormData objektu s klíčem 'fileInput'

        // Odešlete FormData objekt na server pomocí AJAX nebo form submit
        // Například pomocí jQuery AJAX:
        $.ajax({
            url: urlAction, // URL akce v controlleru, která zpracuje nahrání souboru
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                // Zde můžete provést další akce s vrácenými daty
            },
            error: function (error) {
                // Zde můžete zpracovat chybu
            }
        });
    }
}

function getValue()
{
    var retVal = prompt("Enter your name : ", "your name here");
    document.write("You have entered : " + retVal);
}

