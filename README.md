[![Build status](https://ci.appveyor.com/api/projects/status/github/bytedev/ByteDev.Azure.KeyVault?branch=master&svg=true)](https://ci.appveyor.com/project/bytedev/ByteDev-Azure-KeyVault/branch/master)
[![NuGet Package](https://img.shields.io/nuget/v/ByteDev.Azure.KeyVault.svg)](https://www.nuget.org/packages/ByteDev.Azure.KeyVault)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/ByteDev/ByteDev.Azure.KeyVault/blob/master/LICENSE)

# ByteDev.Azure.KeyVault

.NET Standard library that provides some extended Azure Key Vault functionality build on top of the official Azure packages.

## Installation

ByteDev.Azure.KeyVault has been written as a .NET Standard 2.1 library.

ByteDev.Azure.KeyVault is hosted as a package on nuget.org.  To install from the Package Manager Console in Visual Studio run:

`Install-Package ByteDev.Azure.KeyVault`

Further details can be found on the [nuget page](https://www.nuget.org/packages/ByteDev.Azure.KeyVault/).

## Release Notes

Releases follow semantic versioning.

Full details of the release notes can be viewed on [GitHub](https://github.com/ByteDev/ByteDev.Azure.KeyVault/blob/master/docs/RELEASE-NOTES.md).

## Usage

### Secrets

Secrets functionality is accessed through the `KeyVaultSecretClient` class.

Methods:

- DeleteAllAsync
- DeleteAndPurgeAsync
- DeleteAsync
- DeleteIfExistsAsync
- ExistsAsync
- GetAllAsync
- GetAsync
- GetDeletedAsync
- GetDeletedIfExistsAsync
- GetIfExistsAsync
- GetSectionAsync
- GetValueAsync
- GetValueIfExistsAsync
- GetValuesIfExistsAsync
- IsDeletedAsync
- PurgeAllDeletedAsync
- PurgeAsync
- PurgeIfDeletedAsync
- SafeSetValueAsync
- SetValueAsync

Example usage:

```csharp
using ByteDev.Azure.KeyVault.Secrets;

// ...

IKeyVaultSecretClient client = new KeyVaultSecretClient(keyVaultUri);

// Create a secret
await client.SetValueAsync("Test1", "Some value");

// Get the secret's value
string value = await client.GetValueAsync("Test1");

// Delete the secret
await client.DeleteAsync("Test1", true);

// Purge the soft deleted secret
await client.PurgeAsync("Test1");
```

---

### Secrets.Serialization

Deserialize Azure Key Vault secrets directly to a new class instance.

```csharp
// Entitiy class (class you want to deserialize to)

public class Person
{
    public string Name { get; set; }

    [KeyVaultSecretName("email")]
    public string EmailAddress { get; set; }

    [KeyVaultSecretIgnore]
    public string Mobile { get; set; }
}
```

The class above will check Azure Key Vault for the following named secrets:
- `Name`
- `email`

The `Mobile` property will not be set on deserialization as it has been decorated with a `KeyVaultSecretIgnoreAttribute`.

```csharp
using ByteDev.Azure.KeyVault.Secrets;
using ByteDev.Azure.KeyVault.Secrets.Serialization;

// ...

IKeyVaultSecretClient client = new KeyVaultSecretClient(keyVaultUri);

var serializer = new KeyVaultSecretSerializer(client);

var person = await serializer.DeserializeAsync<Person>();

// person.Name == (Value of "Name" secret)
// person.EmailAddress == (Value of "email" secret)
// person.Mobile == null
```

---

### Keys

Keys functionality is accessed through the `KeyVaultKeyClient` class.

Methods:

- CreateAsync
- DeleteAsync
- DeleteIfExistsAsync
- EncryptAsync / DecryptAsync
- ExistsAsync
- GetAsync
- PurgeAsync
- PurgeIfDeletedAsync
- SignAsync / VerifyAsync
- WrapAsync / UnwrapAsync

Example usage:

```csharp
using ByteDev.Azure.KeyVault.Keys;

// ...

IKeyVaultKeyClient client = new KeyVaultKeyClient(keyVaultUri);

const string keyName = "MyKey";
const string clearText = "test string";

// Encrypt/decrypt some text using the Key Vault key

byte[] cipher = await client.EncryptAsync(keyName, EncryptionAlgorithm.RsaOaep, clearText, Encoding.Unicode);

string result = await client.DecryptAsync(keyName, EncryptionAlgorithm.RsaOaep, cipher, Encoding.Unicode);

// result == "test string"
```