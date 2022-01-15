{
   //remember scroll position on save
   const allSaveButtons = document.querySelectorAll('button[data-save]');
   for (let i = 0; i < allSaveButtons.length; i++) {
      const button = allSaveButtons[i];
      button.addEventListener('click', (e) => {
         localStorage.setItem('scrollHeight', window.scrollY);
      })
   }
   const curHeight = localStorage.getItem('scrollHeight');
   if (curHeight) {
      window.scrollTo(0, curHeight);
      localStorage.removeItem('scrollHeight');
   }
}

{
   //remove saved class
   const allSaved = document.querySelectorAll('tr.saved');
   if (allSaved.length > 0) {
      console.log('saved');
      for (let i = 0; i < allSaved.length; i++) {
         const saved = allSaved[i];
         setTimeout(() => {
            saved.className = '';
         }, 2000);
      }
   }
}
{
   //dismiss modal error message
   const dismisses = document.querySelectorAll('[data-dismiss]');
   for (let i = 0; i < dismisses.length; i++) {
      const dismiss = dismisses[i];
      dismiss.addEventListener('click', (e) => {
         document.querySelector('.modal.show').classList = "modal";
      });
   }
}
{
   //add ctrl+click to password fields to show/hide them
   const pwInputs = document.querySelectorAll('input[type="password"]');
   for (let i = 0; i < pwInputs.length; i++) {
      const input = pwInputs[i];
      input.title = 'Ctrl+click to show'
      input.addEventListener('click', (e) => {
         if (e.ctrlKey) {
            e.target.type = e.target.type == 'text' ? 'password' : 'text';
         }
      });
   }
}