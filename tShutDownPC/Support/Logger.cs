using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tShutDownPC.Enums;

namespace tShutDownPC.Support
{
    public static class Logger
    {
        private const string LOG_FOLDER_NAME = "Logs";

        /// <summary>
        /// Write log about shutdown event
        /// </summary>
        /// <param name="shutdownType">type of shutdowh</param>
        public static void WriteLog(ShutdownType shutdownType)
        {
            var pathToLogDirectory = CreateLogDirectoryIfNotExists(); //get path to the log directory
            WriteLogInFile(shutdownType, pathToLogDirectory); //write log about shutdown event
        }

        /// <summary>
        /// Creats and return path to the log directory
        /// </summary>
        /// <returns>path to the log directory</returns>
        private static string CreateLogDirectoryIfNotExists()
        {
            try
            {
                var pathToLogDirectory = $@"{Directory.GetCurrentDirectory()}\{LOG_FOLDER_NAME}";

                if (!Directory.Exists(pathToLogDirectory))
                {
                    Directory.CreateDirectory(pathToLogDirectory);
                }

                return pathToLogDirectory;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void WriteLogInFile(ShutdownType shutdownType, string pathToLogDirectory)
        {
            if (!string.IsNullOrEmpty(pathToLogDirectory))
            {
                var filePath = $"{DateTime.Now.ToString("dd-MM-yyyy")}.log"; //path to log file

                if (!File.Exists(filePath)) //if file is not exists
                    File.Create(filePath); //create it

                using (var textWriter = new StreamWriter(filePath))
                {
                    textWriter.WriteLine($"{DateTime.Now} - {shutdownType.ToString()}");
                }
            }
        }
    }
}
