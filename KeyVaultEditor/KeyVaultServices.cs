using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace KeyVaultEditor
{
    public class KeyVaultServices
    {
        private readonly SecretClient secretClient;
        private readonly ILogger<KeyVaultServices> logger;
        public string Url { get; }

        public KeyVaultServices( ILogger<KeyVaultServices> logger, IConfiguration configuration)
        {
            this.logger = logger;
            Url = configuration["AzureKeyVaultUrl"];
            logger.LogInformation("Accessing {keyvault}", Url);
            secretClient = new SecretClient(new Uri(Url), new DefaultAzureCredential());
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

        public async Task<(bool,string?)> StoreNewKeyVaultSecretValue(string name, string value)
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