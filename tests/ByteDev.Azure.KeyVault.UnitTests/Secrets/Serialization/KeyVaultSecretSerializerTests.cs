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
                WhenKvClientReturnsNameValues(new Dictionary<string, string>());

                var result = await _sut.DeserializeAsync<TestPerson>();

                Assert.That(result.Name, Is.Null);
                Assert.That(result.Age, Is.EqualTo(0));
                Assert.That(result.Postcode, Is.Null);
            }

            [Test]
            public async Task WhenKvClientGetsValues_ThenSetMatchingProperties()
            {
                var nameValues = new Dictionary<string, string>
                {
                    {nameof(TestPerson.Name), "John"}, 
                    {nameof(TestPerson.Age), "50" }
                };
                
                WhenKvClientReturnsNameValues(nameValues);

                var result = await _sut.DeserializeAsync<TestPerson>();

                Assert.That(result.Name, Is.EqualTo("John"));
                Assert.That(result.Age, Is.EqualTo(50));
                Assert.That(result.Postcode, Is.Null);
            }

            private void WhenKvClientReturnsNameValues(IDictionary<string, string> nameValues)
            {
                _kvClient.GetValuesIfExistsAsync(
                        Arg.Any<IEnumerable<string>>(),
                        Arg.Any<bool>(),
                        Arg.Any<CancellationToken>())
                    .Returns(nameValues);
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
}