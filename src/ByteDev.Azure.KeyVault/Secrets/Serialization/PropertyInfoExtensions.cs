using System.Linq;
using System.Reflection;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    internal static class PropertyInfoExtensions
    {
        public static string GetAttributeName(this PropertyInfo source)
        {
            var attribute = (SecretNameAttribute)source
                .GetCustomAttributes(typeof(SecretNameAttribute), false)
                .Single();

            return attribute.Name;
        }
    }
}