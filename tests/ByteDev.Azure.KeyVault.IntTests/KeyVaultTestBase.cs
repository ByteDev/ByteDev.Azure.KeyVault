using ByteDev.Testing.Settings;
using ByteDev.Testing.Settings.Entities;
using ByteDev.Testing.Settings.Providers;

namespace ByteDev.Azure.KeyVault.IntTests
{
    public class KeyVaultTestBase
    {
        public TestAzureKeyVaultSettings TestAzureKvSettings { get; }

        public KeyVaultTestBase()
        {
            TestAzureKvSettings = new TestSettings()
                .AddProvider(new JsonFileSettingsProvider(@"Z:\Dev\ByteDev.Azure.KeyVault.IntTests.settings.json"))
                .GetAzureKeyVaultSettings();
        }
    }
}