using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;

namespace ByteDev.Azure.KeyVault.Secrets
{
    /// <summary>
    /// Represents an interface for using Azure Key Vault secrets.
    /// </summary>
    public interface IKeyVaultSecretClient
    {
        /// <summary>
        /// Key Vault URI.
        /// </summary>
        Uri KeyVaultUri { get; }

        #region Get

        /// <summary>
        /// Checks whether a secret exists.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be true if the secret exists; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all secrets.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be a list of secrets.</returns>
        Task<IList<KeyVaultSecret>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all secrets for a particular section. For example the section name for secret names:
        /// "MySection--Secret1" and "MySection--Secret2" is "MySection".
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be a list of secrets.</returns>
        Task<IList<KeyVaultSecret>> GetSectionAsync(string sectionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the secret.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        Task<KeyVaultSecret> GetAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret. If the secret does not exist then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the secret.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        Task<KeyVaultSecret> GetIfExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret's current value. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the secret's current value.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        Task<string> GetValueAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret's current value. If the secret does not exist then null result is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the secret's value.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        Task<string> GetValueIfExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a dictionary of secret's names and current values.
        /// If any secret does not exist then it's value will be null.
        /// </summary>
        /// <param name="names">Collection of secret's names.</param>
        /// <param name="awaitEachCall">True each call to Key Vault for each name will be awaited in turn. False all tasks will be awaited at the end of the operation. True by default.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be a dictionary of name values.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="names" /> is null.</exception>
        Task<IDictionary<string, string>> GetValuesIfExistsAsync(IEnumerable<string> names, bool awaitEachCall = true, CancellationToken cancellationToken = default);
        
        #endregion

        #region Get Deleted

        /// <summary>
        /// Determines whether a secret is soft deleted but yet to be purged.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result true if the secret is deleted; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        Task<bool> IsDeletedAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a soft deleted secret. If the secret is not soft deleted then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the deleted secret.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        Task<DeletedSecret> GetDeletedAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a soft deleted secret. If the secret is not soft deleted then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation. Result will be the deleted secret or null.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        Task<DeletedSecret> GetDeletedIfExistsAsync(string name, CancellationToken cancellationToken = default);

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
        Task SetValueAsync(string name, string value, CancellationToken cancellationToken = default);

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
        Task<bool> SafeSetValueAsync(string name, string value, CancellationToken cancellationToken = default);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes all secrets.
        /// </summary>
        /// <param name="waitToComplete">Indicates if we should wait for all the secret delete operations to complete before returning.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteAllAsync(bool waitToComplete, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a secret. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="waitToComplete">Indicates if we should wait for the secret delete operation to complete before returning.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        Task DeleteAsync(string name, bool waitToComplete, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a secret. If the secret does not exist then no exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="waitToComplete">Indicates if we should wait for the secret delete operation to complete before returning.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        Task DeleteIfExistsAsync(string name, bool waitToComplete, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a secret and purges it. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> cannot be null or empty.</exception>
        /// <exception cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException">Secret could not be found.</exception>
        Task DeleteAndPurgeAsync(string name, CancellationToken cancellationToken = default);
        
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
        Task PurgeAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Purges a soft deleted secret. If the secret is not soft deleted then no exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task PurgeIfDeletedAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Purges all soft deleted secrets.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task PurgeAllDeletedAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}