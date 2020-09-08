using Azure.Identity;

namespace ByteDev.Azure.KeyVault.IntTests
{
    public static class ClientSecretCredentialFactory
    {
        public static ClientSecretCredential CreateFor(TestSettings settings)
        {
            return new ClientSecretCredential(
                settings.TenantId, 
                settings.ClientId, 
                settings.ClientSecret);
        }
    }
}