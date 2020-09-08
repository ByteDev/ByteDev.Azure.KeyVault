using System;
using System.Runtime.Serialization;

namespace ByteDev.Azure.KeyVault.Secrets
{
    /// <summary>
    /// Represents when a Key Vault secret could not be found.
    /// </summary>
    [Serializable]
    public class SecretNotFoundException : Exception
    {
        private const string DefaultMessage = "Secret could not be found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException" /> class.
        /// </summary>
        public SecretNotFoundException() : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SecretNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>       
        public SecretNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException" /> class.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>       
        public SecretNotFoundException(Exception innerException) : base(DefaultMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Secrets.SecretNotFoundException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected SecretNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}