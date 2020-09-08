using System;
using Azure;

namespace ByteDev.Azure.KeyVault
{
    /// <summary>
    /// Extension methods for <see cref="T:Azure.RequestFailedException" />.
    /// </summary>
    public static class RequestFailedExceptionExtensions
    {
        /// <summary>
        /// Indicates if the exception is for entity not found.
        /// </summary>
        /// <param name="source">Exception.</param>
        /// <returns>True if is not found; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
        public static bool IsNotFound(this RequestFailedException source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Status == 404;
        }

        /// <summary>
        /// Indicates if the exception is for entity not deleted.
        /// </summary>
        /// <param name="source">Exception.</param>
        /// <returns>True if is not deleted; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
        public static bool IsNotDeleted(this RequestFailedException source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Status == 400 && source.Message.Contains("ObjectMustBeDeletedPriorToBeingPurged");
        }
    }
}