using System;
using ByteDev.Azure.KeyVault.Keys;
using NUnit.Framework;

namespace ByteDev.Azure.KeyVault.UnitTests.Keys
{
    [TestFixture]
    public class KeyVaultKeyClientTests
    {
        private Uri _keyVaultUri;

        [SetUp]
        public void SetUp()
        {
            _keyVaultUri = KeyVaultUri.Create("SomeKeyVaultName");
        }

        [TestFixture]
        public class Constructor_Uri : KeyVaultKeyClientTests
        {
            [Test]
            public void WhenUriIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = new KeyVaultKeyClient(null as Uri));
            }

            [Test]
            public void WhenTokenCredentialIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = new KeyVaultKeyClient(_keyVaultUri, null));
            }

            [Test]
            public void WhenCreated_ThenSetsKeyVaultUriProperty()
            {
                var sut = new KeyVaultKeyClient(_keyVaultUri);

                Assert.That(sut.KeyVaultUri, Is.EqualTo(_keyVaultUri));
            }
        }

        [TestFixture]
        public class Constructor_String : KeyVaultKeyClientTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenUriIsNullOrEmpty_ThenThrowException(string uri)
            {
                Assert.Throws<ArgumentException>(() => _ = new KeyVaultKeyClient(uri));
            }

            [Test]
            public void WhenTokenCredentialIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = new KeyVaultKeyClient(_keyVaultUri.AbsoluteUri, null));
            }

            [Test]
            public void WhenCreated_ThenSetsKeyVaultUriProperty()
            {
                var sut = new KeyVaultKeyClient(_keyVaultUri.AbsoluteUri);

                Assert.That(sut.KeyVaultUri, Is.EqualTo(_keyVaultUri));
            }
        }
    }
}