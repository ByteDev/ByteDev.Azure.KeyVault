using System;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    /// <summary>
    /// Represents an attribute that specifies the secret name to use 
    /// when serializing and deserializing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class KeyVaultSecretNameAttribute : KeyVaultSecretAttribute
    {
        /// <summary>
        /// The name of the secret.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.Serialization.SecretNameAttribute" /> class.
        /// </summary>
        /// <param name="name">Secret name.</param>
        public KeyVaultSecretNameAttribute(string name)
        {
            Name = name;
        }
    }
}