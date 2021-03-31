using System.Text.Json.Serialization;
using ByteDev.Testing;

namespace ByteDev.Azure.KeyVault.IntTests
{
    public class TestAzureKvSettings : TestAzureSettings
    {
        [JsonPropertyName("keyVaultName")]
        public string KeyVaultName { get; set; }
    }
}