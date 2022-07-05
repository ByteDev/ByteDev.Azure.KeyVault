using System.Linq;
using System.Reflection;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    internal static class PropertyInfoExtensions
    {
        public static string GetAttributeName(this PropertyInfo source)
        {
            var attribute = (KeyVaultSecretNameAttribute)source
                .GetCustomAttributes(typeof(KeyVaultSecretNameAttribute), false)
                .Single();

            return attribute.Name;
        }
    }
}