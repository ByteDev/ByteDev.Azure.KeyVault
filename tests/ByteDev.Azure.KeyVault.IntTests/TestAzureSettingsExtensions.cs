using Azure.Identity;
using ByteDev.Testing;

namespace ByteDev.Azure.KeyVault.IntTests
{
    public static class TestAzureSettingsExtensions
    {
        public static ClientSecretCredential ToClientSecretCredential(this TestAzureSettings source)
        {
            return ToClientSecretCredential(source, null);
        }

        public static ClientSecretCredential ToClientSecretCredential(this TestAzureSettings source, TokenCredentialOptions tokenCredentialOptions)
        {
            return new ClientSecretCredential(
                source.TenantId, 
                source.ClientId, 
                source.ClientSecret,
                tokenCredentialOptions);
        }
    }
}