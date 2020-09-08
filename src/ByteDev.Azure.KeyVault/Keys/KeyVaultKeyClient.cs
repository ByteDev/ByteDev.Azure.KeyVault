using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace ByteDev.Azure.KeyVault.Keys
{
    /// <summary>
    /// Represents a client for using Azure Key Vault keys.
    /// </summary>
    public class KeyVaultKeyClient : IKeyVaultKeyClient
    {
        private readonly KeyClient _client;
        private readonly TokenCredential _tokenCredential;

        /// <summary>
        /// Key Vault URI.
        /// </summary>
        public Uri KeyVaultUri { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyVaultSecretClient" /> class.
        /// The token credential DefaultAzureCredential will be used to authenticate the client.
        /// Key Vault URI will be taken from environment variable KEYVAULT_ENDPOINT.
        /// </summary>
        public KeyVaultKeyClient()
            : this(Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyVaultSecretClient" /> class.
        /// The token credential DefaultAzureCredential will be used to authenticate the client.
        /// </summary>
        /// <param name="keyVaultUri">Key vault URI.</param>
        public KeyVaultKeyClient(string keyVaultUri)
            : this(keyVaultUri, new DefaultAzureCredential())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyVaultSecretClient" /> class.
        /// </summary>
        /// <param name="keyVaultUri">Key vault URI.</param>
        /// <param name="tokenCredential">Token credential to use when authenticating the client.</param>
        public KeyVaultKeyClient(string keyVaultUri, TokenCredential tokenCredential)
        {
            if (string.IsNullOrEmpty(keyVaultUri))
                throw new ArgumentException("Key vault URI cannot be null or empty.", nameof(keyVaultUri));

            _tokenCredential = tokenCredential ?? throw new ArgumentNullException(nameof(tokenCredential));

            KeyVaultUri = new Uri(keyVaultUri);

            _client = new KeyClient(KeyVaultUri, _tokenCredential);
        }

        /// <summary>
        /// Create a new key. If the key name already exists then a new version is created
        /// for the key.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="keyType">Key type.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<KeyVaultKey> CreateAsync(string keyName, KeyType keyType, CancellationToken cancellationToken = default)
        {
            var response = await _client.CreateKeyAsync(keyName, keyType, cancellationToken: cancellationToken);

            return response.Value;
        }

        /// <summary>
        /// Retrieves a key. If the key does not exist then an exception is thrown.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        public async Task<KeyVaultKey> GetAsync(string keyName, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(keyName))
                throw new ArgumentException("Key name cannot be null or empty.", nameof(keyName));

            try
            {
                var response = await _client.GetKeyAsync(keyName, cancellationToken: cancellationToken);

                return response.Value;
            }
            catch (RequestFailedException ex)
            {
                if (ex.IsNotFound())
                    throw new KeyNotFoundException(ex);

                throw;
            }
        }

        #region Encrypt

        /// <summary>
        /// Encrypt text using an existing Key Vault key.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Encryption algorithm to use.</param>
        /// <param name="clearText">Clear text to encrypt.</param>
        /// <param name="textEncoding">Encoding for the text.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        public Task<byte[]> EncryptAsync(string keyName, 
            EncryptionAlgorithm algorithm,
            string clearText, 
            Encoding textEncoding, 
            CancellationToken cancellationToken = default)
        {
            byte[] clearData = textEncoding.GetBytes(clearText);

            return EncryptAsync(keyName, algorithm, clearData, cancellationToken);
        }

        /// <summary>
        /// Encrypt bytes using an existing Key Vault key.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Encryption algorithm to use.</param>
        /// <param name="clearData">Byte array of data to encrypt.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        public async Task<byte[]> EncryptAsync(string keyName, 
            EncryptionAlgorithm algorithm,
            byte[] clearData, 
            CancellationToken cancellationToken = default)
        {
            var cryptoClient = await CreateCryptoClientAsync(keyName, cancellationToken);

            var encryptResult = await cryptoClient.EncryptAsync(algorithm, clearData, cancellationToken).ConfigureAwait(false);
            
            return encryptResult.Ciphertext;
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypt bytes using an existing Key Vault key and return as a string.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Encryption algorithm to use.</param>
        /// <param name="cipher">Cipher to decrypt.</param>
        /// <param name="textEncoding">Encoding for the text.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        public async Task<string> DecryptAsync(string keyName, 
            EncryptionAlgorithm algorithm,
            byte[] cipher, 
            Encoding textEncoding, 
            CancellationToken cancellationToken = default)
        {
            byte[] clearData = await DecryptAsync(keyName, algorithm, cipher, cancellationToken).ConfigureAwait(false);

            return textEncoding.GetString(clearData);
        }

        /// <summary>
        /// Decrypt bytes using an existing Key Vault key and return as a byte array.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Encryption algorithm to use.</param>
        /// <param name="cipher">Cipher to decrypt.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        public async Task<byte[]> DecryptAsync(string keyName, 
            EncryptionAlgorithm algorithm,
            byte[] cipher, 
            CancellationToken cancellationToken = default)
        {
            var cryptoClient = await CreateCryptoClientAsync(keyName, cancellationToken);

            var dencryptResult = await cryptoClient.DecryptAsync(algorithm, cipher, cancellationToken).ConfigureAwait(false);

            return dencryptResult.Plaintext;
        }

        #endregion

        /// <summary>
        /// Wrap a symmetric key using an existing Key Vault asymmetric key.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Wrap algorithm to use.</param>
        /// <param name="symmetricKeyData">Symmetric key data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        public async Task<byte[]> WrapAsync(string keyName, 
            KeyWrapAlgorithm algorithm,
            byte[] symmetricKeyData, 
            CancellationToken cancellationToken = default)
        {
            var cryptoClient = await CreateCryptoClientAsync(keyName, cancellationToken).ConfigureAwait(false);

            var wrapResult = await cryptoClient.WrapKeyAsync(algorithm, symmetricKeyData, cancellationToken).ConfigureAwait(false);

            return wrapResult.EncryptedKey;
        }

        /// <summary>
        /// Unwrap a previously wrapped symmetric key using an existing Key Vault asymmetric key.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Wrap algorithm to use.</param>
        /// <param name="symmetricWrappedKeyData">Wrapped symmetric key data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        public async Task<byte[]> UnwrapAsync(string keyName, 
            KeyWrapAlgorithm algorithm,
            byte[] symmetricWrappedKeyData, 
            CancellationToken cancellationToken = default)
        {
            var cryptoClient = await CreateCryptoClientAsync(keyName, cancellationToken).ConfigureAwait(false);

            var unwrapResult = await cryptoClient.UnwrapKeyAsync(algorithm, symmetricWrappedKeyData, cancellationToken).ConfigureAwait(false);

            return unwrapResult.Key;
        }


        private async Task<CryptographyClient> CreateCryptoClientAsync(string keyName, CancellationToken cancellationToken)
        {
            var key = await GetAsync(keyName, cancellationToken).ConfigureAwait(false);

            return CreateCryptoClient(key);
        }

        private CryptographyClient CreateCryptoClient(KeyVaultKey key)
        {
            return new CryptographyClient(key.Id, _tokenCredential);
        }
    }
}