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

        public void OnGet()
        {
            Saved = ((string[]?)TempData["Saved"])?.ToList() ?? new List<string>();
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
            await KeyVaultService.StoreNewKeyVaultSecretValue(name, newValue);
            Saved.Add(name);
            TempData.Add("Saved", Saved);
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
                    var list = System.Text.Json.JsonSerializer.Deserialize<List<KV>>(data);
                    foreach(var kv in list)
                    {
                        if (!settings.Any(s => s.Name == kv.key) || overWrite)
                        {
                            await KeyVaultService.StoreNewKeyVaultSecretValue(kv.key, kv.value);
                            Saved.Add(kv.key);
                        }
                    }

                }
            }
            TempData.Add("Saved", Saved);
            return RedirectToPage();
        }
    }

    record KV
    {
        public string key { get; set; }
        public string value { get; set; }
    }}
