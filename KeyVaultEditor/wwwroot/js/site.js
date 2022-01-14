// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var allSaved = document.querySelectorAll('tr.saved,tr.failed');

if (allSaved.length > 0) {
    console.log('saved');
    for (let i = 0; i < allSaved.length; i++) {
        const saved = allSaved[i];
        setTimeout(() => {
            saved.classList += '-light';
            setTimeout(() => {
                saved.classList += 'er';
                setTimeout(() => {
                    saved.classList = null;
                }, 3000);
            }, 3000);
        }, 2000);
    }
}

var dismisses = document.querySelectorAll('[data-dismiss]');
for (let i = 0; i < dismisses.length; i++) {
    var dismiss = dismisses[i];
    dismiss.addEventListener('click', (e) => {
        document.querySelector('.modal.show').classList = "modal";
    });
}