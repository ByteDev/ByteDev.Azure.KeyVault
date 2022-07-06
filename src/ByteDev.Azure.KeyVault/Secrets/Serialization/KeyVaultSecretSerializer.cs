using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ByteDev.Reflection;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    /// <summary>
    /// Represents a serializer for Azure Key Vault secrets.
    /// </summary>
    public class KeyVaultSecretSerializer : IKeyVaultSecretSerializer
    {
        private readonly SecretObjectFactory _secretObjectFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.Serialization.KeyVaultSecretSerializer" /> class.
        /// </summary>
        /// <param name="keyVaultClient">Key vault client.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="keyVaultClient" /> is null.</exception>
        public KeyVaultSecretSerializer(IKeyVaultSecretClient keyVaultClient)
        {
            if (keyVaultClient == null)
                throw new ArgumentNullException(nameof(keyVaultClient));

            _secretObjectFactory = new SecretObjectFactory(keyVaultClient);
        }

        /// <summary>
        /// Deserializes Azure Key Vault secrets to an object.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>New instance of type <typeparamref name="T" />.</returns>
        public Task<T> DeserializeAsync<T>(CancellationToken cancellationToken = default)
            where T : class, new()
        {
            return DeserializeAsync<T>(new DeserializeOptions(), cancellationToken);
        }

        /// <summary>
        /// Deserializes Azure Key Vault secrets to an object.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to.</typeparam>
        /// <param name="options">Deserialize options.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>New instance of type <typeparamref name="T" />.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="options" /> is null.</exception>
        public async Task<T> DeserializeAsync<T>(DeserializeOptions options, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (properties.Length == 0)
                return new T();

            var propertyNames = properties.Select(s => s.Name);

            var propertiesWithAttr = typeof(T).GetPropertiesWithAttribute<KeyVaultSecretAttribute>().ToList();

            var psns = new List<PropertySecretName>();
            
            foreach (var propertyName in propertyNames)
            {
                var attrProperty = propertiesWithAttr.SingleOrDefault(p => p.Name == propertyName);

                if (attrProperty == null)
                {
                    psns.Add(new PropertySecretName(propertyName, options.SecretNamePrefix + propertyName));
                }
                else
                {
                    if (!attrProperty.HasAttribute<KeyVaultSecretIgnoreAttribute>())
                        psns.Add(new PropertySecretName(propertyName, attrProperty.GetAttributeSecretName()));
                }
            }
            
            return await _secretObjectFactory.CreateAsync<T>(psns, cancellationToken);
        }
    }
}