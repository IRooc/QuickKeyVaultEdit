using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyVaultEditor.Pages
{
   public class IndexModel : PageModel
   {
      public KeyVaultServices KeyVaultService { get; }

      public IndexModel(KeyVaultServices keyVaultService)
      {
         KeyVaultService = keyVaultService;
      }

      public IActionResult OnPostGotoEdit()
      {
         KeyVaultService.SetSecretClient(new Uri(Request.Form["url"]));
         return RedirectToPage("KeyVault");
      }
   }
}
