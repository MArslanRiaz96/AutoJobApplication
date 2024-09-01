window.saveAsFile = function (filename, byteBase64) {
    if (!navigator.onLine) {
        alert("You're offline. Please check your internet connection.");
        return;
    }

    var link = document.createElement('a');
    link.download = filename;
    link.href = 'data:application/octet-stream;base64,' + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
