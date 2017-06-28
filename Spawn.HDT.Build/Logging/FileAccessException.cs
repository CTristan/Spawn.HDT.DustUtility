#region Using
using System;
#endregion

namespace Spawn.SDK.Logging
{
    [Serializable]
    public class FileAccessException : Exception
    {
        #region Constructor
        public FileAccessException(string Message, Exception InnerException) : base(Message, InnerException) { } 
        #endregion
    }
}
