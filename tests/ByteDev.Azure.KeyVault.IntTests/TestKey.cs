﻿using System;

namespace ByteDev.Azure.KeyVault.IntTests
{
    public static class TestKey
    {
        public const string ExistingRsaKeyName = "TestRsa2048";

        public const string ExistingEcKeyName = "TestEcP256";

        public const string NonExistingName = "ThisKeyDoesNotExist";

        public static string NewName(string prefix = "")
        {
            return prefix + Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}