using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeyVaultEditor.Pages
{
    public class KeyVaultModel : PageModel
    {

        public KeyVaultModel(KeyVaultServices keyVaultService)
        {
            KeyVaultService = keyVaultService;
        }

        public KeyVaultServices KeyVaultService { get; }

        public List<string> Saved { get; set; } = new List<string>();
        public List<string> Failed { get; set; } = new List<string>();

        public void OnGet()
        {
            Saved = ((string[]?)TempData["Saved"])?.ToList() ?? new List<string>();
            Failed = ((string[]?)TempData["Failed"])?.ToList() ?? new List<string>();
        }

        public async Task<JsonResult> OnGetDownload()
        {
            var settings = await KeyVaultService.GetAllSecretsAsync();
            return new JsonResult(settings.Select(s => new { Key = s.Name, Value = s.Value }));
        }

        public async Task<IActionResult> OnPost()
        {
            var name = Request.Form["Name"].ToString();
            var newValue = Request.Form["Value"].ToString();
            var (success, message) = await KeyVaultService.StoreNewKeyVaultSecretValue(name, newValue);
            if (success)
            {
                Saved.Add(name);
            }
            else
            {
                Failed.Add($"{name}: {message}" );
            }
            TempData.Add("Saved", Saved);
            TempData.Add("Failed", Failed);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            var name = Request.Form["Name"].ToString();
            await KeyVaultService.DeleteSecretValue(name);
            TempData.Add("Deleted", name);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUpload()
        {
            var file = Request.Form.Files["Settings"];
            if (file != null)
            {
                var overWrite = Request.Form["Overwrite"] == "true";
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var settings = await KeyVaultService.GetAllSecretsAsync();
                    var data = reader.ReadToEnd();
                    var list = JsonSerializer.Deserialize<List<KV>>(data) ?? new List<KV>();
                    foreach (var kv in list.Where(k => k?.key != null && k?.value != null))
                    {
                        if (!settings.Any(s => s.Name == kv.key) || overWrite)
                        {
                            var (success, message) = await KeyVaultService.StoreNewKeyVaultSecretValue(kv.key!, kv.value!);
                            if (success)
                            {
                                Saved.Add(kv.key!);
                            }
                            else
                            {
                                Failed.Add($"{kv.key}: {message}");

                            }
                        }
                    }

                }
            }
            TempData.Add("Saved", Saved);
            TempData.Add("Failed", Failed);
            return RedirectToPage();
        }
    }

    record KV
    {
        public string? key { get; set; }
        public string? value { get; set; }
    }
}
