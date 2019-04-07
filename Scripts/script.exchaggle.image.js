var imageUploader = $('#image-uploader');
var imagePreviewSection = $('#image-preview-section');
var imagePreview = $('#image-preview');
var imageRemover = $('#image-remover');
var imageKeep = $('#image-keep');

function imageRender(input) {
    if (input.files && input.files[0]) {
        if (isImage(input.files[0].name)) {
            var reader = new FileReader();
            reader.onload = function (e) {
                imagePreview.attr('src', e.target.result).attr('alt', 'Uploaded Image');
            }
            reader.readAsDataURL(input.files[0]);
            imagePreviewSection.show();
            imageRemover.removeClass('disabled');
            imageKeep.val(1);
        }
    }
}

function imageRemove() {
    imagePreviewSection.hide();
    imagePreview.attr('src', '#').attr('alt', 'No Image');
    imageUploader.replaceWith(imageUploader.clone(true));
    imageRemover.addClass('disabled');
    imageKeep.val(0);
}

function getExtension(filename) {
    var parts = filename.split('.');
    return parts[parts.length - 1];
}

function isImage(filename) {
    var ext = getExtension(filename);
    switch (ext.toLowerCase()) {
        case 'jpg':
        case 'gif':
        case 'bmp':
        case 'png':
            return true;
    }
    return false;
}

$(function () {
    imageUploader.change(function () {
        imageRender(this);
    });
    imageRemover.click(function () {
        imageRemove();
    });
});