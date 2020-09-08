using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using ByteDev.Azure.KeyVault.Keys;
using NUnit.Framework;

namespace ByteDev.Azure.KeyVault.IntTests.Keys
{
    [TestFixture]
    public class KeyVaultKeyClientTests
    {
        private const string ClearText = "Some test string";

        private IKeyVaultKeyClient _sut;

        private TestSettings TestSettings { get; set; }

        [OneTimeSetUp]
        public void ClassSetUp()
        {
            TestSettings = TestSettingsSerializer.Deserialize();
        }

        [SetUp]
        public void SetUp()
        {
            var keyVaultUri = KeyVaultUri.Create(TestSettings.KeyVaultName);

            _sut = new KeyVaultKeyClient(keyVaultUri.AbsoluteUri, ClientSecretCredentialFactory.CreateFor(TestSettings));
        }

        [TestFixture]
        public class CreateAsync : KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenKeyDoesNotExist_ThenCreatesKey()
            {
                var name = TestKey.NewName("C");

                await _sut.CreateAsync(name, KeyType.Rsa);

                var result = await _sut.GetAsync(name);

                Assert.That(result.Name, Is.EqualTo(name));
            }

            [Test]
            public async Task WhenKeyExists_ThenNewVersionIsCreated()
            {
                var originalKey = await _sut.GetAsync(TestKey.ExistingRsaKeyName);

                var newKey = await _sut.CreateAsync(TestKey.ExistingRsaKeyName, KeyType.Rsa);

                Assert.That(newKey.Properties.Version, Is.Not.EqualTo(originalKey.Properties.Version));
            }
        }

        [TestFixture]
        public class GetAsync: KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenKeyExists_ThenReturnKey()
            {
                var result = await _sut.GetAsync(TestKey.ExistingRsaKeyName);

                Assert.That(result.Name, Is.EqualTo(TestKey.ExistingRsaKeyName));
            }

            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAsync(TestKey.NonExistingName));
            }
        }

        [TestFixture]
        public class EncryptAsync : KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenKeyExists_ThenEncrypt()
            {
                byte[] result = await _sut.EncryptAsync(TestKey.ExistingRsaKeyName, EncryptionAlgorithm.RsaOaep, ClearText, Encoding.Unicode);

                var resultStr = Encoding.Unicode.GetString(result);

                Assert.That(result, Is.Not.EqualTo(resultStr));
            }
        }

        [TestFixture]
        public class DecryptAsync : KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenKeyExists_ThenDecryptCipher()
            {
                var cipher = await _sut.EncryptAsync(TestKey.ExistingRsaKeyName, EncryptionAlgorithm.RsaOaep, ClearText, Encoding.Unicode);

                var result = await _sut.DecryptAsync(TestKey.ExistingRsaKeyName, EncryptionAlgorithm.RsaOaep, cipher, Encoding.Unicode);

                Assert.That(result, Is.EqualTo(ClearText));
            }
        }

        [TestFixture]
        public class WrapAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.WrapAsync(TestKey.NonExistingName, KeyWrapAlgorithm.RsaOaep, GenSymmetricKey()));
            }
        }

        [TestFixture]
        public class UnwrapAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UnwrapAsync(TestKey.NonExistingName, KeyWrapAlgorithm.RsaOaep, new byte[] { 1 }));
            }

            [Test]
            public async Task WhenKeyExists_ThenWrapAndUnwrap()
            {
                byte[] symmetricKeyData = GenSymmetricKey();

                var symmetricWrappedKeyData = await _sut.WrapAsync(TestKey.ExistingRsaKeyName, KeyWrapAlgorithm.RsaOaep, symmetricKeyData);

                var unwrappedSymmetricKeyData = await _sut.UnwrapAsync(TestKey.ExistingRsaKeyName, KeyWrapAlgorithm.RsaOaep, symmetricWrappedKeyData);

                Assert.That(symmetricKeyData.Length, Is.EqualTo(unwrappedSymmetricKeyData.Length));
                Assert.That(unwrappedSymmetricKeyData, Is.EquivalentTo(symmetricKeyData));
            }
        }

        private static byte[] GenSymmetricKey()
        {
            return AesManaged.Create().Key;
        }
    }
}