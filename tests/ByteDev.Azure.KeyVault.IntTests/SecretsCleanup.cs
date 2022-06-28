using System.Threading.Tasks;
using ByteDev.Azure.KeyVault.Secrets;
using NUnit.Framework;

namespace ByteDev.Azure.KeyVault.IntTests
{
    [TestFixture]
    public class SecretsCleanup : KeyVaultTestBase
    {
        [Ignore("Run ad-hoc")]
        [Test]
        public async Task AdHocCleanUp()
        {
            var keyVaultUri = KeyVaultUri.Create(TestAzureKvSettings.KeyVaultName);

            var client = new KeyVaultSecretClient(keyVaultUri, TestAzureKvSettings.ToClientSecretCredential());

            await client.DeleteAllAsync(true);
            await client.PurgeAllDeletedAsync();
        }
    }
}