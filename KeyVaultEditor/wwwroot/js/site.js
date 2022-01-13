// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var allSaved = document.querySelectorAll('tr.saved');

if (allSaved.length > 0) {
    console.log('saved');
    for (let i = 0; i < allSaved.length; i++) {
        const saved = allSaved[i];
        setTimeout(() => {
            saved.classList = 'saved-light';
            setTimeout(() => {
                saved.classList = 'saved-lighter';
                setTimeout(() => {
                    saved.classList = null;
                }, 3000);
            }, 3000);
        }, 2000);
    }
}