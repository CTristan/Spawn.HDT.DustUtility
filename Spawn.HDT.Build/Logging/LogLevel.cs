namespace Spawn.SDK.Logging
{
    /// <summary>
    /// The log level enumeration.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// For regular informations.
        /// </summary>
        Trace = 1,
        /// <summary>
        /// The user needs to be warned.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// An error occured.
        /// </summary>
        Error = 4,
        /// <summary>
        /// The user has sent a message.
        /// </summary>
        Message = 8,
        /// <summary>
        /// The user entered a command
        /// </summary>
        Command = 16,
        /// <summary>
        /// A debug message is being logged.
        /// </summary>
        Debug = 32
    } 
}
