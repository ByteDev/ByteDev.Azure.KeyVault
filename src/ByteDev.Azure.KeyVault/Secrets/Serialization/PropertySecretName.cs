namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    internal class PropertySecretName
    {
        public string PropertyName { get; }

        public string SecretName { get; }

        public PropertySecretName(string propertyName, string secretName)
        {
            PropertyName = propertyName;
            SecretName = secretName;
        }
    }
}