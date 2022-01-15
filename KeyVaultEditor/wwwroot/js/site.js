
var allSaved = document.querySelectorAll('tr.saved,tr.failed');

if (allSaved.length > 0) {
   console.log('saved');
   for (let i = 0; i < allSaved.length; i++) {
      const saved = allSaved[i];
      setTimeout(() => {
         saved.className = '';
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

var pwInputs = document.querySelectorAll('input[type="password"]');
for (let i = 0; i < pwInputs.length; i++) {
   var input = pwInputs[i];
   input.title = 'Ctrl+click to show'
   input.addEventListener('click', (e) => {
      if (e.ctrlKey) {
         e.target.type = e.target.type == 'text' ? 'password' : 'text';
      }
   });
}