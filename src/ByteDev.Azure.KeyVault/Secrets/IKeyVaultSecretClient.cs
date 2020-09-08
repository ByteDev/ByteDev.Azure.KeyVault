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
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all secrets.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<IList<KeyVaultSecret>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<KeyVaultSecret> GetAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret. If the secret does not exist then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<KeyVaultSecret> GetIfExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret's value. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<string> GetValueAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a secret's value. If the secret does not exist then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<string> GetValueIfExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a collection of secret's names and values. If any secret does not exist then it's value will be null.
        /// </summary>
        /// <param name="names">Collection of secret's names.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<IDictionary<string, string>> GetValuesIfExistsAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);

        #endregion

        #region Get Deleted

        /// <summary>
        /// Checks whether a secret is soft deleted but yet to be purged.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<bool> IsDeletedAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a soft deleted secret. If the secret is not soft deleted then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<DeletedSecret> GetDeletedAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a soft deleted secret. If the secret is not soft deleted then null is returned.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<DeletedSecret> GetDeletedIfExistsAsync(string name, CancellationToken cancellationToken = default);

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
        Task SetValueAsync(string name, string value, CancellationToken cancellationToken = default);

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
        Task DeleteAsync(string name, bool waitToComplete, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a secret. If the secret does not exist then no exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="waitToComplete">Indicates if we should wait for the secret delete operation to complete before returning.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteIfExistsAsync(string name, bool waitToComplete, CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft deletes a secret and purges it. If the secret does not exist then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteAndPurgeAsync(string name, CancellationToken cancellationToken = default);
        
        #endregion

        #region Purge

        /// <summary>
        /// Purges a soft deleted secret. If the secret is not soft deleted then an exception is thrown.
        /// </summary>
        /// <param name="name">Name of the secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
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