using System.Threading.Tasks;
using Azure;
using ByteDev.Azure.KeyVault.Secrets;
using NUnit.Framework;

namespace ByteDev.Azure.KeyVault.IntTests.Secrets
{
    [TestFixture]
    public class KeyVaultSecretClientTests
    {
        private IKeyVaultSecretClient _sut;

        private TestSettings TestSettings { get; set; }

        private async Task<string> SaveSecretAsync(string name, string value = null)
        {
            if (value == null)
                value = TestSecret.NewValue();

            await _sut.SetValueAsync(name, value);

            return value;
        }

        private Task DeleteSecretAsync(string name)
        {
            return _sut.DeleteAsync(name, true);
        }

        private Task DeleteAllSecretsAsync()
        {
            return _sut.DeleteAllAsync(true);
        }

        [OneTimeSetUp]
        public void ClassSetUp()
        {
            // TODO: use Testing package
            TestSettings = TestSettingsSerializer.Deserialize();
        }

        [SetUp]
        public void SetUp()
        {
            var keyVaultUri = KeyVaultUri.Create(TestSettings.KeyVaultName);

            _sut = new KeyVaultSecretClient(keyVaultUri.AbsoluteUri, ClientSecretCredentialFactory.CreateFor(TestSettings));
        }
        
        // [Test]
        // public async Task AdHocCleanUp()
        // {
        //     await _sut.DeleteAllAsync(true);
        //     await _sut.PurgeAllDeletedAsync();
        // }

        [TestFixture]
        public class ExistsAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenReturnTrue()
            {
                var name = TestSecret.NewName("E");

                await SaveSecretAsync(name);

                var result = await _sut.ExistsAsync(name);

                Assert.That(result, Is.True);
            }

            [Test]
            public async Task WhenSecretDoesNotExist_ThenReturnFalse()
            {
                var result = await _sut.ExistsAsync(TestSecret.NonExistingName);

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class GetAllAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenNoSecretsExist_ThenReturnEmpty()
            {
                await DeleteAllSecretsAsync();

                var result = await _sut.GetAllAsync();

                Assert.That(result, Is.Empty);
            }

            [Test]
            public async Task WhenSecretsExist_ThenReturnAll()
            {
                await DeleteAllSecretsAsync();

                var name1 = TestSecret.NewName("GA");
                var name2 = TestSecret.NewName("GA");
                var name3 = TestSecret.NewName("GA");

                await SaveSecretAsync(name1);
                await SaveSecretAsync(name2);
                await SaveSecretAsync(name3);

                var result = await _sut.GetAllAsync();

                Assert.That(result.Count, Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class GetSectionAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenNoSecretsExist_ThenReturnEmpty()
            {
                await DeleteAllSecretsAsync();

                var result = await _sut.GetSectionAsync("MySection");

                Assert.That(result, Is.Empty);
            }

            [Test]
            public async Task WhenSectionSecretsExist_ThenReturnSectionSecrets()
            {
                await DeleteAllSecretsAsync();

                var name1 = TestSecret.NewName("MySection1--GS");
                var name2 = TestSecret.NewName("MySection2--GS");
                var name3 = TestSecret.NewName("MySection2--GS");

                await SaveSecretAsync(name1);
                await SaveSecretAsync(name2);
                await SaveSecretAsync(name3);

                var result = await _sut.GetSectionAsync("MySection2");

                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[0].Name, Is.EqualTo(name2).Or.EqualTo(name3));
                Assert.That(result[1].Name, Is.EqualTo(name2).Or.EqualTo(name3));
            }
        }

        [TestFixture]
        public class GetAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenReturnSecret()
            {
                var name = TestSecret.NewName("GV");

                var value = await SaveSecretAsync(name);

                var result = await _sut.GetAsync(name);

                Assert.That(result.Value, Is.EqualTo(value));
            }

            [Test]
            public void WhenSecretDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<SecretNotFoundException>(() => _sut.GetAsync(TestSecret.NonExistingName));
            }
        }

        [TestFixture]
        public class GetIfExistsAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenReturnSecret()
            {
                var name = TestSecret.NewName("GV");

                var value = await SaveSecretAsync(name);

                var result = await _sut.GetIfExistsAsync(name);

                Assert.That(result.Value, Is.EqualTo(value));
            }

            [Test]
            public async Task WhenSecretDoesNotExist_ThenReturnNull()
            {
                var result = await _sut.GetIfExistsAsync(TestSecret.NonExistingName);

                Assert.That(result, Is.Null);
            }
        }

        [TestFixture]
        public class GetValueAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenReturnValue()
            {
                var name = TestSecret.NewName("GV");

                var value = await SaveSecretAsync(name);

                var result = await _sut.GetValueAsync(name);

                Assert.That(result, Is.EqualTo(value));
            }

            [Test]
            public void WhenSecretDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<SecretNotFoundException>(() => _sut.GetValueAsync(TestSecret.NonExistingName));
            }
        }

        [TestFixture]
        public class GetValueIfExistsAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenReturnValue()
            {
                var name = TestSecret.NewName("GV");

                var value = await SaveSecretAsync(name);

                var result = await _sut.GetValueIfExistsAsync(name);

                Assert.That(result, Is.EqualTo(value));
            }

            [Test]
            public async Task WhenSecretDoesNotExist_ThenReturnNull()
            {
                var result = await _sut.GetValueIfExistsAsync(TestSecret.NonExistingName);

                Assert.That(result, Is.Null);
            }
        }

        [TestFixture]
        public class GetValuesIfExistsAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenNamesExist_ThenReturnNameValues()
            {
                var name1 = TestSecret.NewName("GV");
                var name2 = TestSecret.NewName("GV");
                var name3 = TestSecret.NewName("GV");

                var value1 = await SaveSecretAsync(name1);
                var value2 = await SaveSecretAsync(name2);

                var result = await _sut.GetValuesIfExistsAsync(new[] { name1, name2, name3 });

                Assert.That(result.Count, Is.EqualTo(3));
                Assert.That(result[name1], Is.EqualTo(value1));
                Assert.That(result[name2], Is.EqualTo(value2));
                Assert.That(result[name3], Is.Null);
            }

            [Test]
            public async Task WhenSameNameTwice_ThenAddOnlyOnce()
            {
                var name1 = TestSecret.NewName("GV");
                var name2 = TestSecret.NewName("GV");

                var value1 = await SaveSecretAsync(name1);
                var value2 = await SaveSecretAsync(name2);

                var result = await _sut.GetValuesIfExistsAsync(new[] { name1, name2, name2 });

                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[name1], Is.EqualTo(value1));
                Assert.That(result[name2], Is.EqualTo(value2));
            }
        }

        [TestFixture]
        public class SetValueAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretDoesNotExists_ThenCreateSecret()
            {
                var name = TestSecret.NewName("SV");
                var value = TestSecret.NewValue();

                await _sut.SetValueAsync(name, value);

                var result = await _sut.GetValueAsync(name);

                Assert.That(result, Is.EqualTo(value));
            }

            [Test]
            public async Task WhenSecretExists_ThenUpdateSecret()
            {
                var name = TestSecret.NewName("SV");

                await SaveSecretAsync(name);

                await _sut.SetValueAsync(name, "newValue");

                var result = await _sut.GetValueAsync(name);

                Assert.That(result, Is.EqualTo("newValue"));
            }
        }

        [TestFixture]
        public class SafeSetValueAsync : KeyVaultSecretClientTests
        {
            private string _name;

            [SetUp]
            public new void SetUp()
            {
                _name = TestSecret.NewName("SSV");
            }

            [Test]
            public async Task WhenSecretDoesNotExists_ThenCreateSecret()
            {
                var newValue = TestSecret.NewValue();

                var result = await _sut.SafeSetValueAsync(_name, newValue);

                Assert.That(result, Is.True);
                Assert.That(await _sut.GetValueAsync(_name), Is.EqualTo(newValue));
            }

            [Test]
            public async Task WhenSecretExists_AndDiffValue_ThenCreateNewVersion()
            {
                const string oldValue = "123";
                const string newValue = "12345";

                await SaveSecretAsync(_name, oldValue);
                
                var result = await _sut.SafeSetValueAsync(_name, newValue);

                Assert.That(result, Is.True);
                Assert.That(await _sut.GetValueAsync(_name), Is.EqualTo(newValue));
            }

            [Test]
            public async Task WhenSecretExists_AndSameValue_ThenDoNothing()
            {
                const string oldValue = "123";
                const string newValue = "123";

                await SaveSecretAsync(_name, oldValue);
                
                var result = await _sut.SafeSetValueAsync(_name, newValue);

                Assert.That(result, Is.False);
                Assert.That(await _sut.GetValueAsync(_name), Is.EqualTo(newValue));
            }
        }

        [TestFixture]
        public class DeleteAllAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretsExists_ThenDeletesAll()
            {
                var name1 = TestSecret.NewName("DA");
                var name2 = TestSecret.NewName("DA");

                await SaveSecretAsync(name1);
                await SaveSecretAsync(name2);

                await _sut.DeleteAllAsync(true);

                Assert.That(await _sut.ExistsAsync(name1), Is.False);
                Assert.That(await _sut.ExistsAsync(name2), Is.False);
            }

            [Test]
            public async Task WhenNoSecretsExist_ThenDoNothing()
            {
                await DeleteAllSecretsAsync();

                await _sut.DeleteAllAsync(true);

                var result = await _sut.GetAllAsync();

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class DeleteAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenDelete()
            {
                var name = TestSecret.NewName("D");

                await SaveSecretAsync(name);

                await _sut.DeleteAsync(name, true);

                var exists = await _sut.ExistsAsync(name);

                Assert.That(exists, Is.False);
            }

            [Test]
            public void WhenSecretDoesNotExist_ThenThrowExcetion()
            {
                Assert.ThrowsAsync<SecretNotFoundException>(() => _sut.DeleteAsync(TestSecret.NonExistingName, false));
            }
        }

        [TestFixture]
        public class DeleteIfExistsAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenDelete()
            {
                var name = TestSecret.NewName("DIE");

                await SaveSecretAsync(name);

                await _sut.DeleteIfExistsAsync(name, true);

                var exists = await _sut.ExistsAsync(name);

                Assert.That(exists, Is.False);
            }

            [Test]
            public void WhenSecretDoesNotExist_ThenDoNothing()
            {
                Assert.DoesNotThrowAsync(() => _sut.DeleteIfExistsAsync(TestSecret.NonExistingName, false));
            }
        }

        [TestFixture]
        public class DeleteAndPurgeAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretExists_ThenDeleteAndPurge()
            {
                var name = TestSecret.NewName("DAP");

                await SaveSecretAsync(name);

                await _sut.DeleteAndPurgeAsync(name);

                var exists = await _sut.ExistsAsync(name);

                Assert.That(exists, Is.False);
            }

            [Test]
            public void WhenSecretDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<SecretNotFoundException>(() => _sut.DeleteAndPurgeAsync(TestSecret.NonExistingName));
            }
        }

        [TestFixture]
        public class IsDeletedAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretIsDeleted_ThenReturnTrue()
            {
                var name = TestSecret.NewName("ID");

                await SaveSecretAsync(name);
                await DeleteSecretAsync(name);

                var result = await _sut.IsDeletedAsync(name);

                Assert.That(result, Is.True);
            }

            [Test]
            public async Task WhenSecretDoesNotExist_ThenReturnFalse()
            {
                var result = await _sut.IsDeletedAsync(TestSecret.NonExistingName);

                Assert.That(result, Is.False);
            }

            [Test]
            public async Task WhenSecretIsNotDeleted_ThenReturnFalse()
            {
                var name = TestSecret.NewName("ID");

                await SaveSecretAsync(name);

                var result = await _sut.IsDeletedAsync(name);

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class GetDeletedAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretIsDeleted_ThenReturnDeletedSecret()
            {
                var name = TestSecret.NewName("GD");

                await SaveSecretAsync(name);
                await DeleteSecretAsync(name);

                var result = await _sut.GetDeletedAsync(name);

                Assert.That(result.Name, Is.EqualTo(name));
            }

            [Test]
            public void WhenSecretIsNotDeleted_ThenThrowException()
            {
                Assert.ThrowsAsync<SecretNotFoundException>(() => _sut.GetDeletedAsync(TestSecret.NonExistingName));
            }
        }

        [TestFixture]
        public class GetDeletedIfExistsAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenSecretIsDeleted_ThenReturnDeletedSecret()
            {
                var name = TestSecret.NewName("GD");

                await SaveSecretAsync(name);
                await DeleteSecretAsync(name);

                var result = await _sut.GetDeletedIfExistsAsync(name);

                Assert.That(result.Name, Is.EqualTo(name));
            }

            [Test]
            public async Task WhenSecretIsNotDeleted_ThenReturnNull()
            {
                var result = await _sut.GetDeletedIfExistsAsync(TestSecret.NonExistingName);

                Assert.That(result, Is.Null);
            }
        }

        [TestFixture]
        public class PurgeAsync : KeyVaultSecretClientTests
        {
            [Test]
            public void WhenSecretDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<SecretNotFoundException>(() => _sut.PurgeAsync(TestSecret.NonExistingName));
            }

            [Test]
            public async Task WhenSecretExistsButNotDeleted_ThenThrowException()
            {
                var name = TestSecret.NewName("P");

                await SaveSecretAsync(name);

                var ex = Assert.ThrowsAsync<RequestFailedException>(() => _sut.PurgeAsync(name));

                Assert.That(ex.IsNotDeleted(), Is.True);
            }

            [Test]
            public async Task WhenSecretIsDeleted_ThenPurge()
            {
                var name = TestSecret.NewName("P");

                await SaveSecretAsync(name);
                await _sut.DeleteAsync(name, true);

                await _sut.PurgeAsync(name);

                var isDeleted = await _sut.IsDeletedAsync(name);

                Assert.That(isDeleted, Is.False);
            }
        }

        [TestFixture]
        public class PurgeIfDeleted : KeyVaultSecretClientTests
        {
            [Test]
            public void WhenSecretDoesNotExist_ThenDoNothing()
            {
                Assert.DoesNotThrowAsync(() => _sut.PurgeIfDeletedAsync(TestSecret.NonExistingName));
            }

            [Test]
            public async Task WhenSecretExistsButNotDeleted_ThenDoNothing()
            {
                var name = TestSecret.NewName("P");

                await SaveSecretAsync(name);

                Assert.DoesNotThrowAsync(() => _sut.PurgeIfDeletedAsync(name));
            }

            [Test]
            public async Task WhenSecretIsDeleted_ThenPurge()
            {
                var name = TestSecret.NewName("P");

                await SaveSecretAsync(name);
                await DeleteSecretAsync(name);

                await _sut.PurgeIfDeletedAsync(name);

                Assert.That(await _sut.ExistsAsync(name), Is.False);
                Assert.That(await _sut.IsDeletedAsync(name), Is.False);
            }
        }

        [TestFixture]
        public class PurgeAllDeletedAsync : KeyVaultSecretClientTests
        {
            [Test]
            public async Task WhenDeletedSecretsExist_ThenPurgeAll()
            {
                var name1 = TestSecret.NewName("PAD");
                var name2 = TestSecret.NewName("PAD");

                await SaveSecretAsync(name1);
                await SaveSecretAsync(name2);

                await DeleteSecretAsync(name1);
                await DeleteSecretAsync(name2);

                await _sut.PurgeAllDeletedAsync();

                Assert.That(await _sut.ExistsAsync(name1), Is.False);
                Assert.That(await _sut.ExistsAsync(name2), Is.False);
                Assert.That(await _sut.IsDeletedAsync(name1), Is.False);
                Assert.That(await _sut.IsDeletedAsync(name2), Is.False);
            }
        }
    }
}