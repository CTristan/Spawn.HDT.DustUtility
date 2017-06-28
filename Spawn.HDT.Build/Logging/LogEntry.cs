#region Using
using System;
#endregion

namespace Spawn.SDK.Logging
{
    public struct LogEntry
    {
        #region Member Variables
        private DateTime m_dtLogTime;
        private LogLevel m_level;
        private string m_strChannel;
        private string m_strMessage;
        private object[] m_vArgs;
        private string m_strLogMessage;
        #endregion

        #region Properties
        #region LogTime
        /// <summary>
        /// Gets the log time.
        /// </summary>
        /// <value>
        /// The log time.
        /// </value>
        public DateTime LogTime
        {
            get { return m_dtLogTime; }
        }
        #endregion

        #region Level
        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public LogLevel Level
        {
            get { return m_level; }
        }
        #endregion

        #region Channel
        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public string Channel
        {
            get { return m_strChannel; }
        } 
        #endregion

        #region Message
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return m_strMessage; }
        }
        #endregion

        #region LogMessage
        /// <summary>
        /// Gets the log message.
        /// </summary>
        /// <returns></returns>
        public string LogMessage
        {
            get { return m_strLogMessage; }
        }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry" /> class.
        /// </summary>
        /// <param name="logTime">The log time.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The args.</param>
        public LogEntry(DateTime logTime, LogLevel level, string channel, string message, object[] arguments)
        {
            m_dtLogTime = logTime;
            m_level = level;
            m_strChannel = channel;
            m_strMessage = string.Format(message, arguments);
            m_vArgs = arguments;

            if (!string.IsNullOrEmpty(channel))
            {
                m_strLogMessage = string.Format("{0} [{1}::{2}] {3}", m_dtLogTime.ToLongTimeString(), m_level, m_strChannel, string.Format(m_strMessage, m_vArgs));
            }
            else
            {
                m_strLogMessage = string.Format("{0} [{1}] {2}", m_dtLogTime.ToLongTimeString(), m_level, string.Format(m_strMessage, m_vArgs));
            }
        }
        #endregion
    }
}
