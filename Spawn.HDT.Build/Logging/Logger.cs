#region Using
using System;
using System.IO;
#endregion

namespace Spawn.SDK.Logging
{
    /// <summary>
    /// Offers the ability to log data and information.
    /// </summary>
    public class Logger
    {
        #region Member Variables
        private string m_strName = string.Empty;
        private DirectoryInfo m_logDirectory = null;
        private string m_strFilePath = string.Empty;
        private FileInfo m_logFile = null;

        private static object s_objLock = null;
        #endregion

        #region Properties
        #region LogFileDirectory
        /// <summary>
        /// Gets the full path of the directory that contains the log files.
        /// </summary>
        /// <value>
        /// The log file directory.
        /// </value>
        public string LogFileDirectory => m_logDirectory.FullName;
        #endregion

        #region LogFileName
        /// <summary>
        /// Gets the full name of the log file.
        /// </summary>
        /// <value>
        /// The name of the log file.
        /// </value>
        public string LogFileName => m_logFile.FullName;
        #endregion

        #region IsDebugMode
        /// <summary>
        /// Gets or sets a value indicating whether this instance is a debug logger.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a debug logger; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugMode { get; set; }
        #endregion

        #region WriteToConsole
        /// <summary>
        /// Gets or sets a value indicating whether this instance should write the log messages to the console.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance should write the log messages to the console; otherwise, <c>false</c>.
        /// </value>
        public bool WriteToConsole { get; set; }
        #endregion
        #endregion

        #region Default
        private static Logger s_default = null;

        /// <summary>
        /// Gets the default logger.
        /// </summary>
        /// <value>
        /// The default logger.
        /// </value>
        public static Logger Default
        {
            get
            {
                if (s_default == null)
                {
                    s_default = new Logger("Default");
                }
                else { }

                return s_default;
            }
        }
        #endregion

        #region Debug
        private static Logger s_debug = null;

        /// <summary>
        /// Gets the debug logger.
        /// </summary>
        /// <value>
        /// The debug logger.
        /// </value>
        public static Logger Debug
        {
            get
            {
                if (s_debug == null)
                {
                    s_debug = new Logger("Debug")
                    {
                        IsDebugMode = true
                    };
                }
                else { }

                return s_debug;
            }
        }
        #endregion

        #region Events
        public class LogEvent : EventArgs
        {
            public LogEntry Log { get; private set; }

            public LogEvent(LogEntry entry)
            {
                Log = entry;
            }
        }

        public event EventHandler<LogEvent> Logging;

        private void OnLogging(LogEntry entry)
        {
            if (Logging != null)
            {
                Logging(this, new LogEvent(entry));
            }
            else { }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="Logger" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentException">The directory doesn't exist!;FilePath</exception>
        public Logger(string name)
        {
            if (Directory.Exists(Environment.CurrentDirectory))
            {
                m_strName = name;

                string strBaseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                
                m_logDirectory = new DirectoryInfo(Path.Combine(strBaseDir, "Logs"));

                if (!m_logDirectory.Exists)
                {
                    m_logDirectory.Create();
                }
                else { }

                SetLogFileName();

                s_objLock = new object();
            }
            else
            {
                throw new ArgumentException("The directory doesn't exist!", "FilePath");
            }
        }
        #endregion

        #region Log
        /// <summary>
        /// Logs with the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="strMessage">The message.</param>
        /// <param name="vArgs">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="FileAccessException">Can't access log file!</exception>
        public LogEntry Log(LogLevel level, string strMessage, params object[] vArgs)
        {
            return Log(level, string.Empty, strMessage, vArgs);
        }

        /// <summary>
        /// Logs with the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="strChannel">The channel.</param>
        /// <param name="strMessage">The message.</param>
        /// <param name="vArgs">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="FileAccessException">Can't access log file!</exception>
        public LogEntry Log(LogLevel level, string strChannel, string strMessage, params object[] vArgs)
        {
            LogEntry retEntry = new LogEntry();

            lock (s_objLock)
            {
                SetLogFileName();

                try
                {
                    retEntry = new LogEntry(DateTime.Now, level, strChannel, strMessage, vArgs);

                    if (WriteToConsole)
                    {
                        LogToConsole(retEntry);
                    }
                    else { }

                    if (IsDebugMode)
                    {
                        System.Diagnostics.Debug.WriteLine(retEntry.LogMessage);
                    }
                    else { }

                    using (StreamWriter writer = new StreamWriter(m_strFilePath, File.Exists(m_strFilePath)))
                    {
                        writer.WriteLine(retEntry.LogMessage);
                        writer.Flush();
                    }
                }
                catch (IOException ex)
                {
                    throw new FileAccessException("Can't access log file!", ex);
                }
            }

            OnLogging(retEntry);

            return retEntry;
        }
        #endregion

        #region LogToConsole
        /// <summary>
        /// Writes to console.
        /// </summary>
        /// <param name="retEntry">The ret entry.</param>
        private void LogToConsole(LogEntry retEntry)
        {
            switch(retEntry.Level)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            Console.WriteLine(retEntry.LogMessage);

            Console.ForegroundColor = ConsoleColor.Gray;
        } 
        #endregion

        #region SetLogFileName
        /// <summary>
        /// Sets the name of the log file.
        /// </summary>
        private void SetLogFileName()
        {
            m_strFilePath = m_strFilePath = Path.Combine(m_logDirectory.FullName, string.Format("{0}_{1}.txt", m_strName, DateTime.Now.ToShortDateString()));

            m_logFile = new FileInfo(m_strFilePath);

            if (!m_logFile.Exists)
            {
                using (m_logFile.Create()) { }
            }
            else { }
        } 
        #endregion
    }
}
