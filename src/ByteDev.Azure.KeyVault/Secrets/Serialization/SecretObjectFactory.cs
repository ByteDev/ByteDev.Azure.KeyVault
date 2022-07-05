using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteDev.Reflection;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    internal class SecretObjectFactory
    {
        private readonly IKeyVaultSecretClient _keyVaultClient;

        public SecretObjectFactory(IKeyVaultSecretClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }

        public async Task<T> CreateAsync<T>(IList<PropertySecretName> propertySecretNames, CancellationToken cancellationToken) 
            where T : class, new()
        {
            var obj = new T();

            var secretsDictionary = await _keyVaultClient.GetValuesIfExistsAsync(propertySecretNames.Select(p => p.SecretName), false, cancellationToken);

            foreach (var secret in secretsDictionary)
            {
                if (secret.Value != null)
                {
                    var psn = propertySecretNames.SingleOrDefault(p => p.SecretName == secret.Key);

                    if (psn != null)
                        obj.SetPropertyValue(psn.PropertyName, secret.Value);
                }
            }

            return obj;
        }
    }
}