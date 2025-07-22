using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
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
        public async Task<JsonResult> OnGetDownloadAzureEnv()
        {
            var settings = await KeyVaultService.GetAllSecretsAsync();
            Dictionary<string, string> value = settings.ToDictionary(s => s.Name.Replace("--", ConfigurationPath.KeyDelimiter), s => s.Value);

            return new JsonResult(value.Select(kv =>
            {
                return new
                {
                    name = kv.Key,
                    value = kv.Value,
                    slotSetting = true
                };
            }));
        }

        public async Task<IActionResult> OnPost()
        {
            var form = await HttpContext.Request.ReadFormAsync();
            var name = form["Name"].ToString();
            var newValue = form["Value"].ToString();
            if (name.Contains(ConfigurationPath.KeyDelimiter))
            {
                name = name.Replace(ConfigurationPath.KeyDelimiter, "--");
            }
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
            var form = await HttpContext.Request.ReadFormAsync();
            var name = form["Name"].ToString();
            await KeyVaultService.DeleteSecretValue(name);
            return RedirectToPage(new { deleted = name });
        }
        public async Task<IActionResult> OnPostUpload()
        {
            var form = await HttpContext.Request.ReadFormAsync();
            var file = form.Files["Settings"];
            if (file != null)
            {
                var overWrite = form["Overwrite"] == "true";
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
        public async Task<IActionResult> OnPostMultiple()
        {
            var form = await HttpContext.Request.ReadFormAsync();
            var overWrite = form["Overwrite"] == "true";
            string? newValues = form["Multiple"];
            if (!string.IsNullOrWhiteSpace(newValues))
            {
                var settings = await KeyVaultService.GetAllSecretsAsync();

                newValues = newValues.Trim();
                if (!newValues.StartsWith("{")) //if just values were pasted, wrap them in braces
                {
                    newValues = $"{{{newValues}}}";
                }
                var newSettings = JsonDocument.Parse(newValues);
                var flattened = new Dictionary<string, string>();
                FlattenJsonElement(newSettings.RootElement, flattened, string.Empty, "--");
                var list = JsonDocument.Parse(JsonSerializer.Serialize(flattened)); //this is weird, but ok...
                await SaveElement(string.Empty, overWrite, settings, list.RootElement);
            }
            TempData.Add("Saved", Saved);
            TempData.Add("Failed", Failed);
            return RedirectToPage();

        }

        private void FlattenJsonElement(JsonElement element, Dictionary<string, string> result, string prefix, string delimiter = ":")
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        //recurse
                        string newPrefix = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}{delimiter}{property.Name}";
                        FlattenJsonElement(property.Value, result, newPrefix, delimiter);
                    }
                    break;

                default:
                    // For primitives and arrays, assign the value directly
                    result[prefix] = element.ToString();
                    break;
            }
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
                    var key = prefix + jobj.Name.Replace(ConfigurationPath.KeyDelimiter, "--");
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
