using System;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    /// <summary>
    /// Represents an attribute that prevents a property from being serialized or deserialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class KeyVaultSecretIgnoreAttribute : KeyVaultSecretAttribute
    {
    }
}