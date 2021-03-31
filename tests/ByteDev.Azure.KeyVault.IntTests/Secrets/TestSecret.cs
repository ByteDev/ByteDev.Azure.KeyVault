using System;

namespace ByteDev.Azure.KeyVault.IntTests.Secrets
{
    public static class TestSecret
    {
        public const string NonExistingName = "ThisSecretDoesNotExist";

        public static string NewName(string prefix = "")
        {
            return prefix + Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        public static string NewValue()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}