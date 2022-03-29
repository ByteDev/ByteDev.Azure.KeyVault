using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace ByteDev.Azure.KeyVault.Keys
{
    public interface IKeyVaultKeyClient
    {
        /// <summary>
        /// Key Vault URI.
        /// </summary>
        Uri KeyVaultUri { get; }

        /// <summary>
        /// Create a new key. If the key name already exists then a new version is created
        /// for the key.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="keyType">Key type.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<KeyVaultKey> CreateAsync(string keyName, KeyType keyType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a key. If the key does not exist then an exception is thrown.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="keyName" /> is null or empty.</exception>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        Task<KeyVaultKey> GetAsync(string keyName, CancellationToken cancellationToken = default);

        #region Encrypt / Decrypt

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
        Task<byte[]> EncryptAsync(string keyName,
            EncryptionAlgorithm algorithm,
            string clearText,
            Encoding textEncoding,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Encrypt bytes using an existing Key Vault key.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Encryption algorithm to use.</param>
        /// <param name="clearData">Byte array of data to encrypt.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        Task<byte[]> EncryptAsync(string keyName,
            EncryptionAlgorithm algorithm,
            byte[] clearData,
            CancellationToken cancellationToken = default);

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
        Task<string> DecryptAsync(string keyName,
            EncryptionAlgorithm algorithm,
            byte[] cipher,
            Encoding textEncoding,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Decrypt bytes using an existing Key Vault key and return as a byte array.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Encryption algorithm to use.</param>
        /// <param name="cipher">Cipher to decrypt.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        Task<byte[]> DecryptAsync(string keyName,
            EncryptionAlgorithm algorithm,
            byte[] cipher,
            CancellationToken cancellationToken = default);

        #endregion

        #region Sign / Verify

        /// <summary>
        /// Sign a given digest using a Key Vault key. Returns the signature as a byte array.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Signature algorithm to use.</param>
        /// <param name="digest">Digest to sign.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        Task<byte[]> SignAsync(string keyName,
            SignatureAlgorithm algorithm,
            byte[] digest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verify a digest's signature. Returns true if valid.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Signature algorithm to use.</param>
        /// <param name="digest">Digest corresponding to the signature.</param>
        /// <param name="signature">Signature to verify the digest against.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        Task<bool> VerifyAsync(string keyName,
            SignatureAlgorithm algorithm,
            byte[] digest,
            byte[] signature,
            CancellationToken cancellationToken = default);

        #endregion

        #region Wrap / Unwrap

        /// <summary>
        /// Wrap a symmetric key using an existing Key Vault asymmetric key.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Wrap algorithm to use.</param>
        /// <param name="symmetricKeyData">Symmetric key data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        Task<byte[]> WrapAsync(string keyName,
            KeyWrapAlgorithm algorithm,
            byte[] symmetricKeyData,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Unwrap a previously wrapped symmetric key using an existing Key Vault asymmetric key.
        /// </summary>
        /// <param name="keyName">Name of existing Key Vault key.</param>
        /// <param name="algorithm">Wrap algorithm to use.</param>
        /// <param name="symmetricWrappedKeyData">Wrapped symmetric key data.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException">Key could not be found.</exception>
        Task<byte[]> UnwrapAsync(string keyName,
            KeyWrapAlgorithm algorithm,
            byte[] symmetricWrappedKeyData,
            CancellationToken cancellationToken = default);

        #endregion
    }
}