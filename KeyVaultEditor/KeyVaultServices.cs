using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace KeyVaultEditor
{
   public class KeyVaultServices
   {
      private SecretClient? secretClient;
      private readonly ILogger<KeyVaultServices> logger;
      public string? Url { get; set;}

      public KeyVaultServices(ILogger<KeyVaultServices> logger)
      {
         this.logger = logger;
      }

      public void SetSecretClient(Uri url)
      {
         Url = url.AbsoluteUri;
         logger.LogInformation("Accessing {keyvault}", url);
         secretClient = new SecretClient(url, new DefaultAzureCredential());
      }

      public async Task<bool> DeleteSecretValue(string name)
      {
         var op = await secretClient.StartDeleteSecretAsync(name);
         var res = await op.WaitForCompletionAsync();

         return res?.Value?.Name == name;
      }

      public async Task<IList<KeyVaultSecret>> GetAllSecretsAsync()
      {
         var secrets = new List<KeyVaultSecret>();
         var secretProperties = secretClient.GetPropertiesOfSecretsAsync();

         await foreach (var secretProperty in secretProperties)
         {
            var response = await secretClient.GetSecretAsync(secretProperty.Name);
            secrets.Add(response);
         }

         return secrets;
      }

      public async Task<(bool, string?)> StoreNewKeyVaultSecretValue(string name, string value)
      {
         try
         {
            var secret = await secretClient.SetSecretAsync(name, value);
            return (secret != null, null);
         }
         catch (Azure.RequestFailedException rfe)
         {
            logger.LogError(rfe, "Failed to add {name}", name);
            return (false, rfe.Message);
         }
      }
   }
}