var Plachtovac = Plachtovac || {};
Plachtovac.getBoundingClientRect = function (element) {
    return element.getBoundingClientRect();
};

Plachtovac.saveAsFile = function (filename, data, contentType) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:"+contentType+ ";base64," + data;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}