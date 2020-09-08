using System;

namespace ByteDev.Azure.KeyVault.IntTests
{
    /// <summary>
    /// Setup environment if using DefaultAzureCredential.
    /// </summary>
    public static class TestEnvironment
    {
        public static void SetUp(TestSettings settings)
        {
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", settings.ClientId);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", settings.ClientSecret);
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", settings.TenantId);
        }

        public static void TearDown()
        {
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", null);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", null);
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", null);
        }
    }
}