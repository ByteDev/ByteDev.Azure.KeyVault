using System.Reflection;
using ByteDev.Reflection;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    internal static class PropertyInfoExtensions
    {
        public static string GetAttributeSecretName(this PropertyInfo source)
        {
            var attribute = source.GetAttribute<KeyVaultSecretNameAttribute>();

            return attribute.Name;
        }
    }
}