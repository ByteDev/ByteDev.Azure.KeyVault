# Release Notes

## 2.2.0 - 05 July 2022

Breaking changes:
- (None)

New features:
- Added `KeyVaultSecretSerializer` class.
- Added `KeyVaultKeyClient.DeleteAsync` method.
- Added `KeyVaultKeyClient.DeleteIfExistsAsync` method.
- Added `KeyVaultKeyClient.ExistsAsync` method.
- Added `KeyVaultKeyClient.PurgeAsync` method.
- Added `KeyVaultKeyClient.PurgeIfDeletedAsync` method.

Bug fixes / internal changes:
- Added package `ByteDev.Reflection` dependency.

## 2.1.0 - 28 June 2022

Breaking changes:
- (None)

New features:
- Added `KeyVaultSecretClient.GetValuesIfExistsAsync` method overload.
- Added constructor overload to `KeyVaultSecretClient`.
- Added constructor overload to `KeyVaultKeyClient`.

Bug fixes / internal changes:
- Update package dependencies:
  - Azure.Identity 1.6.0
  - Azure.Security.KeyVault.Keys 4.3.0
  - Azure.Security.KeyVault.Secrets 4.3.0

## 2.0.0 - 29 March 2022

Breaking changes:
- Removed overload for `KeyVaultKeyClient.SignAsync`.

New features:
- (None)

Bug fixes / internal changes:
- (None)

## 1.2.0 - 28 March 2022

Breaking changes:
- (None)

New features:
- Added `KeyVaultKeyClient.SignAsync` method.
- Added `KeyVaultKeyClient.VerifyAsync` method.

Bug fixes / internal changes:
- XML doc comment improvements.

## 1.1.0 - 31 March 2021

Breaking changes:
- (None)

New features:
- Added `KeyVaultSecretClient.SafeSetValueAsync` method.
- Added `KeyVaultSecretClient.GetSectionAsync` method.

Bug fixes / internal changes:
- XML doc comment improvements.

## 1.0.0 - 07 September 2020

Initial version.
