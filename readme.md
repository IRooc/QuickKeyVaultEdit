# Quick editor for a KeyVaults in Azure

Only works on keyvault that have Get, List, Set and Delete AccessPolicies set for you user.

You can create or update individual settings or upload a batch of settings through a Json file

Just set the `AzureKeyvaultUrl` in the appsettings.json and run the project either in visual studio or do `dotnet run` in the KeyVaultEditor folder

## Settings

Currently reads a Key Vault URL set in appsettings.json called `AzureKeyVaultUrl`


## Note

This is a demo project and should not be used in production environments. 