﻿using System.Collections.Generic;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using ByteDev.Azure.KeyVault.Keys;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KeyNotFoundException = ByteDev.Azure.KeyVault.Keys.KeyNotFoundException;

namespace ByteDev.Azure.KeyVault.IntTests.Keys
{
    [TestFixture]
    public class KeyVaultKeyClientTests : KeyVaultTestBase
    {
        private const string ClearText = "Some test string";

        private readonly IList<string> _createdKeys = new List<string>();

        private IKeyVaultKeyClient _sut;

        [SetUp]
        public void SetUp()
        {
            var keyVaultUri = KeyVaultUri.Create(TestAzureKvSettings.KeyVaultName);

            _sut = new KeyVaultKeyClient(keyVaultUri, TestAzureKvSettings.ToClientSecretCredential());
        }

        [TearDown]
        public async Task TearDown()
        {
            foreach (var key in _createdKeys)
            {
                await _sut.DeleteIfExistsAsync(key, true);
                await _sut.PurgeIfDeletedAsync(key);
            }

            _createdKeys.Clear();
        }

        private async Task SaveKeyAsync(string name)
        {
            TrackKey(name);

            await _sut.CreateAsync(name, KeyType.Rsa);
        }

        private void TrackKey(string name)
        {
            _createdKeys.Add(name);
        }

        [TestFixture]
        public class DeleteAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(TestKey.NotExistName, true));
            }

            [Test]
            public async Task WhenKeyExists_ThenDelete()
            {
                var name = TestKey.NewName("Delete");

                await SaveKeyAsync(name);

                await _sut.DeleteAsync(name, true);

                var exists = await _sut.ExistsAsync(name);

                Assert.That(exists, Is.False);
            }
        }

        [TestFixture]
        public class DeleteIfExistsAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenDoNothing()
            {
                Assert.DoesNotThrowAsync(() => _sut.DeleteIfExistsAsync(TestKey.NotExistName, true));
            }

            [Test]
            public async Task WhenKeyExists_ThenDelete()
            {
                var name = TestKey.NewName("DeleteIfExists");

                await SaveKeyAsync(name);

                await _sut.DeleteIfExistsAsync(name, true);

                var exists = await _sut.ExistsAsync(name);

                Assert.That(exists, Is.False);
            }
        }

        [TestFixture]
        public class PurgeAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.PurgeAsync(TestKey.NotExistName));
            }

            [Test]
            public async Task WhenKeyHasBeenDeleted_ThenPurge()
            {
                var name = TestKey.NewName("Purge");

                await SaveKeyAsync(name);

                await _sut.DeleteAsync(name, true);

                await _sut.PurgeAsync(name);

                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.PurgeAsync(name));
            }
        }

        [TestFixture]
        public class PurgeIfDeletedAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenDoNothing()
            { 
                Assert.DoesNotThrowAsync(() => _sut.PurgeIfDeletedAsync(TestKey.NotExistName));
            }

            [Test]
            public async Task WhenKeyHasBeenDeleted_ThenPurge()
            {
                var name = TestKey.NewName("PurgeIfDeleted");

                await SaveKeyAsync(name);

                await _sut.DeleteAsync(name, true);

                await _sut.PurgeIfDeletedAsync(name);

                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.PurgeAsync(name));
            }
        }

        [TestFixture]
        public class ExistsAsync : KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenKeyDoesNotExist_ThenReturnFalse()
            {
                var result = await _sut.ExistsAsync(TestKey.NotExistName);

                Assert.That(result, Is.False);
            }

            [Test]
            public async Task WhenKeyExists_ThenReturnTrue()
            {
                var name = TestKey.NewName("Exists");

                await SaveKeyAsync(name);

                var result = await _sut.ExistsAsync(name);
                
                Assert.That(result, Is.True);
            }
        }

        [TestFixture]
        public class CreateAsync : KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenKeyDoesNotExist_ThenCreatesKey()
            {
                var name = TestKey.NewName("Create");

                TrackKey(name);

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
        public class GetAsync : KeyVaultKeyClientTests
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
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAsync(TestKey.NotExistName));
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

                Assert.That(resultStr, Is.Not.EqualTo(ClearText));
            }

            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.EncryptAsync(TestKey.NotExistName, EncryptionAlgorithm.RsaOaep, ClearText, Encoding.Unicode));
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

            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DecryptAsync(TestKey.NotExistName, EncryptionAlgorithm.RsaOaep, new byte[0], Encoding.Unicode));
            }
        }

        [TestFixture]
        public class SignAsync : KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenKeyExists_ThenSign()
            {
                var digestData = GetDigest(ClearText);

                var result = await _sut.SignAsync(TestKey.ExistingRsaKeyName, SignatureAlgorithm.RS256, digestData);

                var resultStr = Encoding.Unicode.GetString(result);

                Assert.That(resultStr, Is.Not.EqualTo(ClearText));
            }

            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                var digestData = GetDigest(ClearText);

                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SignAsync(TestKey.NotExistName, SignatureAlgorithm.RS256, digestData));
            }
        }

        [TestFixture]
        public class VerifyAsync : KeyVaultKeyClientTests
        {
            [Test]
            public async Task WhenSignatureIsValidForDigest_ThenReturnTrue()
            {
                byte[] digest1 = GetDigest(ClearText);
                byte[] signature = await _sut.SignAsync(TestKey.ExistingRsaKeyName, SignatureAlgorithm.RS256, digest1);

                byte[] digest2 = GetDigest(ClearText);
                var result = await _sut.VerifyAsync(TestKey.ExistingRsaKeyName, SignatureAlgorithm.RS256, digest2, signature);

                Assert.That(result, Is.True);   
            }

            [Test]
            public async Task WhenSignatureIsInvalidForForDigest_ThenReturnFalse()
            {
                byte[] digest = GetDigest(ClearText);
                byte[] signature = await _sut.SignAsync(TestKey.ExistingRsaKeyName, SignatureAlgorithm.RS256, digest);

                byte[] diffDigest = GetDigest(ClearText + "a");

                var result = await _sut.VerifyAsync(TestKey.ExistingRsaKeyName, SignatureAlgorithm.RS256, diffDigest, signature);

                Assert.That(result, Is.False);
            }

            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.VerifyAsync(TestKey.NotExistName, SignatureAlgorithm.RS256, new byte[0], new byte[0]));
            }
        }

        [TestFixture]
        public class WrapAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.WrapAsync(TestKey.NotExistName, KeyWrapAlgorithm.RsaOaep, GenSymmetricKey()));
            }
        }

        [TestFixture]
        public class UnwrapAsync : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenKeyDoesNotExist_ThenThrowException()
            {
                Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UnwrapAsync(TestKey.NotExistName, KeyWrapAlgorithm.RsaOaep, new byte[] { 1 }));
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

        private static byte[] GetDigest(string text)
        {
            using (HashAlgorithm hashAlgo = SHA256.Create())
            {
                return hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(text));
            }
        }
    }
}