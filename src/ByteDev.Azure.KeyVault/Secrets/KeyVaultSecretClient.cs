using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Constructor

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
        /// <exception cref="T:System.ArgumentException"><paramref name="keyVaultUri" /> cannot be null or empty.</exception>
        public KeyVaultSecretClient(string keyVaultUri, TokenCredential tokenCredential)
        {
            if (string.IsNullOrEmpty(keyVaultUri))
                throw new ArgumentException("Key vault URI cannot be null or empty.", nameof(keyVaultUri));

            KeyVaultUri = new Uri(keyVaultUri);

            _client = new SecretClient(KeyVaultUri, tokenCredential);
        }

        #endregion

        #region Get

        /// <summary>
        /// Checks whether a secret exists.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be true if the secret exists; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// <returns>The task object representing the asynchronous operation. Result will be a list of secrets.</returns>
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
        /// Retrieves all secrets for a particular section. For example the section name for secret names:
        /// "MySection--Secret1" and "MySection--Secret2" is "MySection".
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be a list of secrets.</returns>
        public async Task<IList<KeyVaultSecret>> GetSectionAsync(string sectionName, CancellationToken cancellationToken = default)
        {
            AsyncPageable<SecretProperties> secretProperties = _client.GetPropertiesOfSecretsAsync(cancellationToken);

            var secrets = new List<KeyVaultSecret>();

            var sectionPrefix = CreateSectionPrefix(sectionName);

            await foreach (var secretProperty in secretProperties)
            {
                if (secretProperty.Name.StartsWith(sectionPrefix))
                {
                    var secret = await GetAsync(secretProperty.Name, cancellationToken).ConfigureAwait(false);

                    secrets.Add(secret);
                }
            }

            return secrets;
        }

        /// <summary>
        /// Retrieves a secret. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the secret.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// <returns>The task object representing the asynchronous operation. Result will be the secret.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// Retrieves a secret's current value. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the secret's current value.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        public async Task<string> GetValueAsync(string name, CancellationToken cancellationToken = default)
        {
            var secret = await GetAsync(name, cancellationToken).ConfigureAwait(false);
            
            return secret.Value;
        }

        /// <summary>
        /// Retrieves a secret's current value. If the secret does not exist then null result is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the secret's value.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        public async Task<string> GetValueIfExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var secret = await GetIfExistsAsync(name, cancellationToken).ConfigureAwait(false);

            return secret?.Value;
        }

        /// <summary>
        /// Retrieves a dictionary of secret's names and current values.
        /// If any secret does not exist then it's value will be null.
        /// </summary>
        /// <param name="names">Collection of secret's names.</param>
        /// <param name="awaitEachCall">True each call to Key Vault for each name will be awaited in turn. False all tasks will be awaited at the end of the operation. True by default.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be a dictionary of name values.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="names" /> is null.</exception>
        public async Task<IDictionary<string, string>> GetValuesIfExistsAsync(IEnumerable<string> names, bool awaitEachCall = true, CancellationToken cancellationToken = default)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            if (awaitEachCall)
                return await GetValuesIfExistsAwaitedAsync(names, cancellationToken);

            return await GetValuesIfExistsNotAwaitedAsync(names, cancellationToken);
        }

        #endregion

        #region Get Deleted

        /// <summary>
        /// Determines whether a secret is soft deleted but yet to be purged.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result true if the secret is deleted; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// <returns>The task object representing the asynchronous operation. Result will be the deleted secret.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// <returns>The task object representing the asynchronous operation. Result will be the deleted secret or null.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// Sets a secret's current value. If the secret does not exist then it is created.
        /// If the secret exists then a new current version is created.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="value">New value for the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        public Task SetValueAsync(string name, string value, CancellationToken cancellationToken = default)
        {
            ValidateName(name);

            return _client.SetSecretAsync(name, value, cancellationToken);
        }

        /// <summary>
        /// Sets a secret's current value in a idempotent manner.
        /// If the secret does not exist then it is created.
        /// If the secret exists and it's current value is different to the new one then a new current version is created and result true will be returned.
        /// If the secret exists and it's current value is equal to the new value then no set will be performed and result false will be returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="value">New value for the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be true if a secret was set; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        public async Task<bool> SafeSetValueAsync(string name, string value, CancellationToken cancellationToken = default)
        {
            ValidateName(name);

            var oldValue = await GetValueIfExistsAsync(name, cancellationToken);

            if (oldValue != value)
            {
                await SetValueAsync(name, value, cancellationToken);
                return true;
            }

            return false;
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
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// Deletes a secret and purges it. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
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

        private static string CreateSectionPrefix(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                return string.Empty;

            if (sectionName.EndsWith("--"))
                return sectionName;

            return sectionName + "--";
        }

        private async Task<IDictionary<string, string>> GetValuesIfExistsAwaitedAsync(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var name in names)
            {
                if (!dictionary.ContainsKey(name))
                {
                    var value = await GetValueIfExistsAsync(name, cancellationToken).ConfigureAwait(false);

                    dictionary.Add(name, value);
                }
            }

            return dictionary;
        }

        private async Task<IDictionary<string, string>> GetValuesIfExistsNotAwaitedAsync(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            var nameTasks = new List<Tuple<string, Task<string>>>();

            foreach (var name in names.Distinct())
            {
                var task = GetValueIfExistsAsync(name, cancellationToken);

                nameTasks.Add(new Tuple<string, Task<string>>(name, task));
            }

            IEnumerable<Task<string>> tasks = nameTasks.Select(i => i.Item2);

            await Task.WhenAll(tasks);

            return nameTasks.ToDictionary();
        }
    }
}
