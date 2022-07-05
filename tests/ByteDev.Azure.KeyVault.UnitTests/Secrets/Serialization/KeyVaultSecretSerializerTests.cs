using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ByteDev.Azure.KeyVault.Secrets;
using ByteDev.Azure.KeyVault.Secrets.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace ByteDev.Azure.KeyVault.UnitTests.Secrets.Serialization
{
    [TestFixture]
    public class KeyVaultSecretSerializerTests
    {
        private IKeyVaultSecretClient _kvClient;
        private KeyVaultSecretSerializer _sut;

        [SetUp]
        public void SetUp()
        {
            _kvClient = Substitute.For<IKeyVaultSecretClient>();

            _sut = new KeyVaultSecretSerializer(_kvClient);
        }

        private void WhenKvClientReturnsSecrets(IDictionary<string, string> secrets)
        {
            _kvClient.GetValuesIfExistsAsync(
                    Arg.Any<IEnumerable<string>>(),
                    Arg.Any<bool>(),
                    Arg.Any<CancellationToken>())
                .Returns(secrets);
        }

        [TestFixture]
        public class Constructor : KeyVaultSecretSerializerTests
        {
            [Test]
            public void WhenIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = new KeyVaultSecretSerializer(null));
            }
        }

        [TestFixture]
        public class DeserializeAsync : KeyVaultSecretSerializerTests
        {
            [Test]
            public void WhenOptionsIsNull_ThenThrowException()
            {
                Assert.ThrowsAsync<ArgumentNullException>(() => _ = _sut.DeserializeAsync<TestPerson>(null));
            }

            [Test]
            public async Task WhenTypeHasNoProperties_ThenDoNotCallKv()
            {
                var result = await _sut.DeserializeAsync<TestPersonNoProperties>();

                Assert.That(result, Is.Not.Null);

                await _kvClient
                    .DidNotReceiveWithAnyArgs()
                    .GetValuesIfExistsAsync(
                        Arg.Any<IEnumerable<string>>(),
                        Arg.Any<bool>(),
                        Arg.Any<CancellationToken>());
            }

            [Test]
            public async Task WhenKvClientReturnsEmpty_ThenDoNotSetProperties()
            {
                WhenKvClientReturnsSecrets(new Dictionary<string, string>());

                var result = await _sut.DeserializeAsync<TestPerson>();

                Assert.That(result.Name, Is.Null);
                Assert.That(result.Age, Is.EqualTo(0));
                Assert.That(result.Postcode, Is.Null);
            }

            [Test]
            public async Task WhenKvClientGetsValues_ThenSetMatchingProperties()
            {
                var secrets = new Dictionary<string, string>
                {
                    {nameof(TestPerson.Name), "John"}, 
                    {nameof(TestPerson.Age), "50" }
                };
                
                WhenKvClientReturnsSecrets(secrets);

                var result = await _sut.DeserializeAsync<TestPerson>();

                Assert.That(result.Name, Is.EqualTo("John"));
                Assert.That(result.Age, Is.EqualTo(50));
                Assert.That(result.Postcode, Is.Null);
            }
        }

        [TestFixture]
        public class DeserializeAsync_SecretNameAttribute : KeyVaultSecretSerializerTests
        {
            [Test]
            public async Task WhenHasMatchingAttributeName_ThenSetProperty()
            {
                var secrets = new Dictionary<string, string>
                {
                    {"email", "someone@somewhere.com" }
                };

                WhenKvClientReturnsSecrets(secrets);

                var result = await _sut.DeserializeAsync<TestPersonWithAttributes>();
                
                Assert.That(result.EmailAddress, Is.EqualTo("someone@somewhere.com"));
            }

            [TestCase("EmailAddress")]
            [TestCase("Email")]
            public async Task WhenAttributeNameDoesNotMatch_ThenDoNotSetProperty(string secretName)
            {
                var secrets = new Dictionary<string, string>
                {
                    {secretName, "someone@somewhere.com" }
                };

                WhenKvClientReturnsSecrets(secrets);

                var result = await _sut.DeserializeAsync<TestPersonWithAttributes>();
                
                Assert.That(result.EmailAddress, Is.Null);
            }

            [Test]
            public async Task WhenUsesAttributesAndNot_ThenSetMatchingProperties()
            {
                var secrets = new Dictionary<string, string>
                {
                    {nameof(TestPersonWithAttributes.Name), "John"},
                    {nameof(TestPersonWithAttributes.Address), "123 High Street"},
                    {"email", "someone@somewhere.com" }
                };

                WhenKvClientReturnsSecrets(secrets);

                var result = await _sut.DeserializeAsync<TestPersonWithAttributes>();

                Assert.That(result.Name, Is.EqualTo("John"));
                Assert.That(result.Address, Is.Null);
                Assert.That(result.EmailAddress, Is.EqualTo("someone@somewhere.com"));
            }
        }

        [TestFixture]
        public class DeserializeAsync_SecretIgnoreAttribute : KeyVaultSecretSerializerTests
        {
            [Test]
            public async Task WhenHasMatchingName_AndIgnoreAttribute_ThenDoNotSetProperty()
            {
                var secrets = new Dictionary<string, string>
                {
                    {nameof(TestPersonWithAttributes.Mobile), "01234567"}
                };

                WhenKvClientReturnsSecrets(secrets);

                var result = await _sut.DeserializeAsync<TestPersonWithAttributes>();

                Assert.That(result.Mobile, Is.Null);
            }

            [Test]
            public async Task WhenHasBothIgnoreAndNameAttributes_ThenDoesNotSetProperty()
            {
                var secrets = new Dictionary<string, string>
                {
                    {"mobile", "01234567"}
                };

                WhenKvClientReturnsSecrets(secrets);
                
                var result = await _sut.DeserializeAsync<TestPersonWithIgnoreAndNameAttributes>();

                Assert.That(result.Mobile, Is.Null);
            }
        }
    }

    public class TestPersonNoProperties
    {
    }

    public class TestPerson
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Postcode { get; set; }
    }

    public class TestPersonWithAttributes
    {
        public string Name { get; set; }

        [KeyVaultSecretName("address")]
        public string Address { get; set; }

        [KeyVaultSecretName("email")]
        public string EmailAddress { get; set; }

        [KeyVaultSecretIgnore]
        public string Mobile { get; set; }
    }

    public class TestPersonWithIgnoreAndNameAttributes
    {
        [KeyVaultSecretIgnore]
        [KeyVaultSecretName("mobile")]
        public string Mobile { get; set; }
    }
}