using System;
using System.Runtime.Serialization;

namespace ByteDev.Azure.KeyVault.Keys
{
    /// <summary>
    /// Represents when a Key Vault key could not be found.
    /// </summary>
    [Serializable]
    public class KeyNotFoundException : Exception
    {
        private const string DefaultMessage = "Key could not be found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException" /> class.
        /// </summary>
        public KeyNotFoundException() : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public KeyNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>       
        public KeyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException" /> class.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>       
        public KeyNotFoundException(Exception innerException) : base(DefaultMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Azure.KeyVault.Keys.KeyNotFoundException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected KeyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}