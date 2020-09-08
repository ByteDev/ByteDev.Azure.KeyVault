using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace ByteDev.Azure.KeyVault.Secrets
{
    /// <summary>
    /// Represents a client for using Azure Key Vault secrets.
    /// </summary>
    public class KeyVaultSecretClient : IKeyVaultSecretClient
    {
        private readonly SecretClient _client;

        /// <summary>
        /// Key Vault URI.
        /// </summary>
        public Uri KeyVaultUri { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.KeyVaultSecretClient" /> class.
        /// The token credential DefaultAzureCredential will be used to authenticate the client.
        /// Key Vault URI will be taken from environment variable KEYVAULT_ENDPOINT.
        /// </summary>
        public KeyVaultSecretClient()
            : this(Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.KeyVaultSecretClient" /> class.
        /// The token credential DefaultAzureCredential will be used to authenticate the client.
        /// </summary>
        /// <param name="keyVaultUri">Key vault URI.</param>
        public KeyVaultSecretClient(string keyVaultUri)
            : this(keyVaultUri, new DefaultAzureCredential())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.KeyVaultSecretClient" /> class.
        /// </summary>
        /// <param name="keyVaultUri">Key vault URI.</param>
        /// <param name="tokenCredential">Token credential to use when authenticating the client.</param>
        public KeyVaultSecretClient(string keyVaultUri, TokenCredential tokenCredential)
        {
            if (string.IsNullOrEmpty(keyVaultUri))
                throw new ArgumentException("Key vault URI cannot be null or empty.", nameof(keyVaultUri));

            KeyVaultUri = new Uri(keyVaultUri);

            _client = new SecretClient(KeyVaultUri, tokenCredential);
        }

        #region Get

        /// <summary>
        /// Checks whether a secret exists.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                await GetAsync(name, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (SecretNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all secrets.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<IList<KeyVaultSecret>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            AsyncPageable<SecretProperties> secretProperties = _client.GetPropertiesOfSecretsAsync(cancellationToken);

            var secrets = new List<KeyVaultSecret>();

            await foreach (var secretProperty in secretProperties)
            {
                var secret = await GetAsync(secretProperty.Name, cancellationToken).ConfigureAwait(false);

                secrets.Add(secret);
            }

            return secrets;
        }

        /// <summary>
        /// Retrieves a secret. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        public async Task<KeyVaultSecret> GetAsync(string name, CancellationToken cancellationToken = default)
        {
            ValidateName(name);

            try
            {
                var response = await _client.GetSecretAsync(name, cancellationToken: cancellationToken).ConfigureAwait(false);

                return response.Value;
            }
            catch (RequestFailedException ex)
            {
                if (ex.IsNotFound())
                    throw new SecretNotFoundException(ex);

                throw;
            }
        }

        /// <summary>
        /// Retrieves a secret. If the secret does not exist then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<KeyVaultSecret> GetIfExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                return await GetAsync(name, cancellationToken).ConfigureAwait(false);
            }
            catch (SecretNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a secret's value. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        public async Task<string> GetValueAsync(string name, CancellationToken cancellationToken = default)
        {
            var secret = await GetAsync(name, cancellationToken).ConfigureAwait(false);
            
            return secret.Value;
        }

        /// <summary>
        /// Retrieves a secret's value. If the secret does not exist then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<string> GetValueIfExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var secret = await GetIfExistsAsync(name, cancellationToken).ConfigureAwait(false);

            return secret?.Value;
        }

        /// <summary>
        /// Retrieves a collection of secret's names and values. If any secret does not exist then it's value will be null.
        /// </summary>
        /// <param name="names">Collection of secret's names.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<IDictionary<string, string>> GetValuesIfExistsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            var nameValues = new Dictionary<string, string>();

            foreach (var name in names)
            {
                if (!nameValues.ContainsKey(name))
                {
                    var value = await GetValueIfExistsAsync(name, cancellationToken).ConfigureAwait(false);

                    nameValues.Add(name, value);
                }
            }

            return nameValues;
        }

        #endregion

        #region Get Deleted

        /// <summary>
        /// Checks whether a secret is soft deleted but yet to be purged.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<bool> IsDeletedAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                await GetDeletedAsync(name, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (SecretNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves a soft deleted secret. If the secret is not soft deleted then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        public async Task<DeletedSecret> GetDeletedAsync(string name, CancellationToken cancellationToken = default)
        {
            ValidateName(name);

            try
            {
                var response = await _client.GetDeletedSecretAsync(name, cancellationToken).ConfigureAwait(false);

                return response.Value;
            }
            catch (RequestFailedException ex)
            {
                if (ex.IsNotFound())
                    throw new SecretNotFoundException(ex);

                throw;
            }
        }

        /// <summary>
        /// Retrieves a soft deleted secret. If the secret is not soft deleted then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<DeletedSecret> GetDeletedIfExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                return await GetDeletedAsync(name, cancellationToken).ConfigureAwait(false);
            }
            catch (SecretNotFoundException)
            {
                return null;
            }
        }

        #endregion

        #region Set

        /// <summary>
        /// Sets a secret's value. If the secret does not exist then it is created.
        /// If the secret exists then its value is updated.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="value">New value for the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetValueAsync(string name, string value, CancellationToken cancellationToken = default)
        {
            ValidateName(name);

            return _client.SetSecretAsync(name, value, cancellationToken);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes all secrets.
        /// </summary>
        /// <param name="waitToComplete">Indicates if we should wait for all the secret delete operations to complete before returning.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task DeleteAllAsync(bool waitToComplete, CancellationToken cancellationToken = default)
        {
            var secrets = await GetAllAsync(cancellationToken).ConfigureAwait(false);

            var tasks = new List<Task>();

            foreach (var secret in secrets)
            {
                tasks.Add(DeleteIfExistsAsync(secret.Name, waitToComplete, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Deletes a secret. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="waitToComplete">Indicates if we should wait for the secret delete operation to complete before returning.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        public async Task DeleteAsync(string name, bool waitToComplete, CancellationToken cancellationToken = default)
        {
            ValidateName(name);

            try
            {
                var response = await _client.StartDeleteSecretAsync(name, cancellationToken).ConfigureAwait(false);

                if (waitToComplete)
                    await response.WaitForCompletionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException ex)
            {
                if (ex.IsNotFound())
                    throw new SecretNotFoundException(ex);

                throw;
            }
        }

        /// <summary>
        /// Deletes a secret. If the secret does not exist then no exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="waitToComplete">Indicates if we should wait for the secret delete operation to complete before returning.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task DeleteIfExistsAsync(string name, bool waitToComplete, CancellationToken cancellationToken = default)
        {
            try
            {
                await DeleteAsync(name, waitToComplete, cancellationToken).ConfigureAwait(false);
            }
            catch (SecretNotFoundException)
            {
                // Secret does not exist
            }
        }

        /// <summary>
        /// Soft deletes a secret and purges it. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        public async Task DeleteAndPurgeAsync(string name, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(name, true, cancellationToken).ConfigureAwait(false);

            await PurgeAsync(name, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Purge

        /// <summary>
        /// Purges a soft deleted secret. If the secret is not soft deleted then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        public async Task PurgeAsync(string name, CancellationToken cancellationToken = default)
        {
            ValidateName(name);

            try
            {
                await _client.PurgeDeletedSecretAsync(name, cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException ex)
            {
                if (ex.IsNotFound())
                    throw new SecretNotFoundException(ex);

                throw;
            }
        }

        /// <summary>
        /// Purges a soft deleted secret. If the secret is not soft deleted then no exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task PurgeIfDeletedAsync(string name, CancellationToken cancellationToken = default)
        {
            var isDeleted = await IsDeletedAsync(name, cancellationToken).ConfigureAwait(false);

            if (isDeleted)
                await PurgeAsync(name, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Purges all soft deleted secrets.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task PurgeAllDeletedAsync(CancellationToken cancellationToken = default)
        {
            AsyncPageable<DeletedSecret> deletedSecrets = _client.GetDeletedSecretsAsync(cancellationToken);

            var tasks = new List<Task>();

            await foreach (var deletedSecret in deletedSecrets)
            {
                tasks.Add(PurgeAsync(deletedSecret.Name, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        #endregion

        private static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Secret name cannot be null or empty.");
        }
    }
}
