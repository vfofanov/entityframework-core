using System;
using System.Runtime.Serialization;

namespace Stenn.EntityConventions.Contacts
{
        [Serializable]
        public class EntityConventionsException : Exception
        {
            public EntityConventionsException()
            {
            }

            public EntityConventionsException(string message) 
                : base(message)
            {
            }

            public EntityConventionsException(string message, Exception inner) 
                : base(message, inner)
            {
            }

            protected EntityConventionsException(
                SerializationInfo info,
                StreamingContext context) : base(info, context)
            {
            }
        }
}