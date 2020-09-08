using System;
using System.Threading.Tasks;
using ByteDev.Azure.KeyVault.Secrets;
using NUnit.Framework;

namespace ByteDev.Azure.KeyVault.UnitTests.Secrets
{
    [TestFixture]
    public class KeyVaultSecretClientTests
    {
        private IKeyVaultSecretClient _sut;

        [SetUp]
        public void SetUp()
        {
            var keyVaultUri = KeyVaultUri.Create("SomeKeyVaultName");

            _sut = new KeyVaultSecretClient(keyVaultUri.AbsoluteUri);
        }

        [TestFixture]
        public class GetAsync : KeyVaultSecretClientTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenIsNullOrEmpty_ThenThrowException(string name)
            {
                Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAsync(name));
            }
        }

        [TestFixture]
        public class GetValuesIfExistsAsync : KeyVaultSecretClientTests
        {
            [Test]
            public void WhenNamesIsNull_ThenThrowException()
            {
                Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetValuesIfExistsAsync(null));
            }

            [Test]
            public async Task WhenNameIsEmpty_ThenReturnEmpty()
            {
                var result = await _sut.GetValuesIfExistsAsync(new string[0]);

                Assert.That(result, Is.Empty);
            }
        }

        [TestFixture]
        public class SetValueAsync : KeyVaultSecretClientTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenIsNullOrEmpty_ThenThrowException(string name)
            {
                Assert.ThrowsAsync<ArgumentException>(() => _sut.SetValueAsync(name, "SomeValue"));
            }
        }

        [TestFixture]
        public class DeleteAsync : KeyVaultSecretClientTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenIsNullOrEmpty_ThenThrowException(string name)
            {
                Assert.ThrowsAsync<ArgumentException>(() => _sut.DeleteAsync(name, false));
            }
        }

        [TestFixture]
        public class GetDeletedAsync : KeyVaultSecretClientTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenIsNullOrEmpty_ThenThrowException(string name)
            {
                Assert.ThrowsAsync<ArgumentException>(() => _sut.GetDeletedAsync(name));
            }
        }

        [TestFixture]
        public class PurgeAsync : KeyVaultSecretClientTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenIsNullOrEmpty_ThenThrowException(string name)
            {
                Assert.ThrowsAsync<ArgumentException>(() => _sut.PurgeAsync(name));
            }
        }
    }
}