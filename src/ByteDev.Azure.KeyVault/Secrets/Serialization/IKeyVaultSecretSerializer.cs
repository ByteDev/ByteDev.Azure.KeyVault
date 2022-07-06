using System.Threading;
using System.Threading.Tasks;

namespace ByteDev.Azure.KeyVault.Secrets.Serialization
{
    public interface IKeyVaultSecretSerializer
    {
        /// <summary>
        /// Deserializes Azure Key Vault secrets to an object.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>New instance of type <typeparamref name="T" />.</returns>
        Task<T> DeserializeAsync<T>(CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        /// Deserializes Azure Key Vault secrets to an object.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to.</typeparam>
        /// <param name="options">Deserialize options.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>New instance of type <typeparamref name="T" />.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="options" /> is null.</exception>
        Task<T> DeserializeAsync<T>(DeserializeOptions options, CancellationToken cancellationToken = default)
            where T : class, new();
    }
}