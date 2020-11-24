[![Build status](https://ci.appveyor.com/api/projects/status/github/bytedev/ByteDev.Azure.KeyVault?branch=master&svg=true)](https://ci.appveyor.com/project/bytedev/ByteDev-Azure-KeyVault/branch/master)
[![NuGet Package](https://img.shields.io/nuget/v/ByteDev.Azure.KeyVault.svg)](https://www.nuget.org/packages/ByteDev.Azure.KeyVault)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/ByteDev/ByteDev.Azure.KeyVault/blob/master/LICENSE)

# ByteDev.Azure.KeyVault

.NET Standard library providing some Azure Key Vault functionality.

## Installation

ByteDev.Azure.KeyVault has been written as a .NET Standard 2.1 library and is based on the `Azure.Security.KeyVault.Secrets` package.

ByteDev.Azure.KeyVault is hosted as a package on nuget.org.  To install from the Package Manager Console in Visual Studio run:

`Install-Package ByteDev.Azure.KeyVault`

Further details can be found on the [nuget page](https://www.nuget.org/packages/ByteDev.Azure.KeyVault/).

## Release Notes

Releases follow semantic versioning.

Full details of the release notes can be viewed on [GitHub](https://github.com/ByteDev/ByteDev.Azure.KeyVault/blob/master/docs/RELEASE-NOTES.md).

## Usage

### Secrets

Secrets functionality is accessed through the `KeyVaultSecretClient` class in namespace `ByteDev.Azure.KeyVault.Secrets`.

Methods:
- ExistsAsync
- GetAllAsync
- GetAsync
- GetIfExistsAsync
- GetValueAsync
- GetValueIfExistsAsync
- GetValuesIfExistsAsync
- IsDeletedAsync
- GetDeletedAsync
- GetDeletedIfExistsAsync
- SetValueAsync
- DeleteAllAsync
- DeleteAsync
- DeleteIfExistsAsync
- DeleteAndPurgeAsync
- IsDeletedAsync
- PurgeAsync
- PurgeIfDeletedAsync
- PurgeAllDeletedAsync

Example usage:

```csharp
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

### Keys

Keys functionality is accessed through the `KeyVaultKeyClient` class in namespace `ByteDev.Azure.KeyVault.Keys`.

Methods:
- CreateAsync
- GetAsync
- EncryptAsync
- DecryptAsync
- WrapAsync
- UnwrapAsync

Example usage:

```csharp
IKeyVaultKeyClient client = new KeyVaultKeyClient(keyVaultUri);

const string clearText = "test string";

// Encrypt/decrypt some text using the Key Vault key

byte[] cipher = await client.EncryptAsync("Test1", EncryptionAlgorithm.RsaOaep, clearText, Encoding.Unicode);

string result = await client.DecryptAsync("Test1", EncryptionAlgorithm.RsaOaep, cipher, Encoding.Unicode);

// result == "test string"
```