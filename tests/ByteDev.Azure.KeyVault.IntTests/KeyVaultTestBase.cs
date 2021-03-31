using System.Reflection;
using ByteDev.Azure.KeyVault.IntTests.Secrets;
using ByteDev.Testing;

namespace ByteDev.Azure.KeyVault.IntTests
{
    public class KeyVaultTestBase
    {
        public TestAzureKvSettings TestAzureKvSettings { get; set; }

        public KeyVaultTestBase()
        {
            var assembly = Assembly.GetAssembly(typeof(KeyVaultSecretClientTests));

            var testSettings = new TestSettings(assembly)
            {
                FilePaths = new[] {@"Z:\Dev\ByteDev.Azure.KeyVault.IntTests.settings.json"}
            };
            
            TestAzureKvSettings = testSettings.GetSettings<TestAzureKvSettings>();
        }
    }
}