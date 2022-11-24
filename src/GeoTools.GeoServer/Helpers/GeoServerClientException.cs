using GeoTools.GeoServer.Resources;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace GeoTools.GeoServer.Helpers
{
    internal class GeoServerClientException : Exception
    {
        public int StatusCode { get; }

        public GeoServerClientException(int statusCode, string message, Exception innerException)
            : base(message, innerException) => StatusCode = statusCode;

        public GeoServerClientException(int statusCode, SerializationInfo info, StreamingContext context)
            : base(info, context) => StatusCode = statusCode;

        protected GeoServerClientException(SerializationInfo info, StreamingContext context)
            : base(info, context) => StatusCode = info.GetInt32(nameof(StatusCode));

        public override string Message
        {
            get
            {
                string s = base.Message;
                string resourceString = string.Format(Messages.Exception_GeoServerClientException, StatusCode);
                return s + Environment.NewLine + resourceString;
            }
        }

        [System.Security.SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            Contract.EndContractBlock();
            base.GetObjectData(info, context);
            info.AddValue(nameof(StatusCode), StatusCode, typeof(int));
        }
    }
}
