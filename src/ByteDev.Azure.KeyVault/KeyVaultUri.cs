using System;

namespace ByteDev.Azure.KeyVault
{
    public static class KeyVaultUri
    {
        public static Uri Create(string keyVaultName)
        {
            if (string.IsNullOrEmpty(keyVaultName))
                throw new ArgumentException("Key vault name cannot be null or empty.");

            return new Uri("https://" + keyVaultName + ".vault.azure.net/");
        }
    }
}