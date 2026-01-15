# Quick editor for a KeyVault in Azure

Only works on keyvault that have Get, List, Set and Delete AccessPolicies set for you user.

You can create or update individual settings or upload a batch of settings through a Json file

Run the project either in visual studio or do `dotnet run` in the project root folder.
Then specify the keyvault URL in the interface and click `Goto Edit` and edit your keyvault.

## Note

This is a demo project and should not be used in production environments.

Specificly a `:` in your Key name will be changed to `--` in the keyvault. This so having a keyvault as
source for configuration in a dotnet project it will just work.