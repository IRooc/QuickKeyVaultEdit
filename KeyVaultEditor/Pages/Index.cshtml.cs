using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyVaultEditor.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(KeyVaultServices keyVaultService)
        {
            KeyVaultService = keyVaultService;
        }

        public KeyVaultServices KeyVaultService { get; }
        public void OnGet()
        {
        }
    }
}
