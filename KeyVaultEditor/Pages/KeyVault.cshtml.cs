using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

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
            return new JsonResult(settings.ToDictionary(s => s.Name.Replace("--", ConfigurationPath.KeyDelimiter), s => s.Value));
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
                Failed.Add($"{name}: {message}");
            }
            TempData.Add("Saved", Saved);
            TempData.Add("Failed", Failed);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDelete()
        {
            var name = Request.Form["Name"].ToString();
            await KeyVaultService.DeleteSecretValue(name);
            return RedirectToPage(new { deleted = name });
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
                    var list = JsonSerializer.Deserialize<JsonDocument>(data) ?? JsonDocument.Parse("{}");
                    await SaveElement(string.Empty, overWrite, settings, list.RootElement);

                }
            }
            TempData.Add("Saved", Saved);
            TempData.Add("Failed", Failed);
            return RedirectToPage();
        }

        private async Task SaveElement(string prefix, bool overWrite, IEnumerable<KeyVaultSecret> settings, JsonElement root)
        {
            foreach (var jobj in root.EnumerateObject())
            {
                if (jobj.Value.ValueKind == JsonValueKind.Object)
                {
                    await SaveElement(jobj.Name + "--", overWrite, settings, jobj.Value);
                }
                else if (jobj.Value.ValueKind == JsonValueKind.String ||
                         jobj.Value.ValueKind == JsonValueKind.Number ||
                         jobj.Value.ValueKind == JsonValueKind.True ||
                         jobj.Value.ValueKind == JsonValueKind.False)
                {
                    var key = prefix + jobj.Name.Replace(ConfigurationPath.KeyDelimiter,"--");
                    if (!settings.Any(s => s.Name == key) || overWrite)
                    {
                        var (success, message) = await KeyVaultService.StoreNewKeyVaultSecretValue(key, jobj.Value.ToString());
                        if (success)
                        {
                            Saved.Add(key);
                        }
                        else
                        {
                            Failed.Add($"{key}: {message}");
                        }
                    }
                }
                else
                {
                    Failed.Add($"{jobj.Name}: Unsupported value kind in Json {jobj.Value.ValueKind} with value {jobj.Value}");
                }
            }
        }
    }

}
