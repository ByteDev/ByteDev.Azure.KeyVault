namespace ByteDev.Azure.KeyVault.IntTests
{
    public class TestSettings
    {
        public string KeyVaultName { get; set; }

        /// <summary>
        /// ID of an Azure Active Directory application (AD application appId).
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// One of the application's client secrets (AD application app secret).
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// ID of the application's Azure Active Directory tenant.
        /// </summary>
        public string TenantId { get; set; }
    }
}