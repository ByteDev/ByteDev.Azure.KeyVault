namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    public class DeserializeOptions
    {
        /// <summary>
        /// Prefix to apply to all secret names when communicating with Key Vault.
        /// This prefix will not be applied to names set using <see cref="T:ByteDev.Azure.KeyVault.Secrets.Serialization.SecretNameAttribute" />.
        /// </summary>
        public string SecretNamePrefix { get; set; }
    }
}