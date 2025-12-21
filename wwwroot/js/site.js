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
   //scroll into view and remove saved class
   const allSaved = document.querySelectorAll('tr.saved');
   if (allSaved.length > 0) {
      console.log('saved');
      allSaved[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
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
{
    //remember previous items
    const gotoEditButton = document.getElementById('gotoEdit');
    const storedItemsDiv = document.getElementById('storedItemsDiv');
    let storedItems = [];


    const selectItem = (e) => {
        const url = e.target.dataset.storedvalue;
        if (e.ctrlKey) {
            const ix = storedItems.indexOf(url);
            if (ix >= 0) {
                storedItems.splice(ix, 1);
                localStorage.setItem('storedItems', JSON.stringify(storedItems));
            }
            e.target.remove();
        } else {
            document.querySelector('input[name="url"]').value = url;
            gotoEditButton.click();
        }
    }

    if (storedItemsDiv) {
        storedItems = JSON.parse(localStorage.getItem('storedItems') ?? "[]");
        storedItems.sort();
        for (let i = 0; i < storedItems.length; i++) {
            const el = document.createElement('button');
            el.type = 'button';
            el.title = 'Ctrl click to remove';
            el.onclick = selectItem;
            el.dataset.storedvalue = storedItems[i];
            el.innerText = storedItems[i];
            storedItemsDiv.appendChild(el);
        }
    }
    if (gotoEditButton) {
        gotoEditButton.addEventListener('click', () => {
            const newUrl = document.querySelector('input[name="url"]').value;
            if (storedItems.indexOf(newUrl) === -1) {
                storedItems.push(newUrl);
                localStorage.setItem('storedItems', JSON.stringify(storedItems));
            }
        })
    }

}