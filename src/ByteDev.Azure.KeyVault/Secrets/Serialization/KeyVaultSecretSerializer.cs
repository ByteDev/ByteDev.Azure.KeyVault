using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ByteDev.Reflection;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    public class KeyVaultSecretSerializer
    {
        private readonly IKeyVaultSecretClient _keyVaultClient;

        public KeyVaultSecretSerializer(IKeyVaultSecretClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient ?? throw new ArgumentNullException(nameof(keyVaultClient));
        }

        public Task<T> DeserializeAsync<T>(CancellationToken cancellationToken = default)
            where T : class, new()
        {
            return DeserializeAsync<T>(null, cancellationToken);
        }

        public async Task<T> DeserializeAsync<T>(string settingPrefix, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            var obj = new T();

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (properties.Length == 0)
                return obj;

            var propertyNames = properties.Select(s => s.Name).ToList();
            var kvNames = properties.Select(s => settingPrefix + s.Name);

            var dictionary = await _keyVaultClient.GetValuesIfExistsAsync(kvNames, false, cancellationToken);
            
            int index = 0;

            foreach (var item in dictionary)
            {
                if (item.Value != null)
                    obj.SetPropertyValue(propertyNames[index], item.Value);

                index++;
            }

            return obj;
        }
    }
}