using System;

namespace ByteDev.Azure.KeyVault.IntTests.Keys
{
    public static class TestKey
    {
        public const string ExistingRsaKeyName = "TestRsa2048";

        public const string ExistingEcKeyName = "TestEcP256";

        public const string NotExistName = "ThisKeyDoesNotExist";

        public static string NewName(string prefix = null)
        {
            if (prefix == null)
                return Guid.NewGuid().ToString("N");

            return prefix + "-" + Guid.NewGuid().ToString("N");
        }
    }
}