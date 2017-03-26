using System;
using System.IO;
using DictationProcessorLib;

namespace DictationProcessorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // iterate through subfolders
            foreach( var subfolder in Directory.GetDirectories(GetUploadsPath()))
            {
                var uploadProcessor = new UploadProcessor(subfolder);
                uploadProcessor.Process();
            }
        }

        private static string GetUploadsPath()
        {
            string appPath = AppContext.BaseDirectory;
            string uploadsPath = appPath.Substring(0, appPath.IndexOf("Projects/") + 9) + "DictationProcessor";
            return Path.Combine(uploadsPath, "uploads");
        }
    }
}
