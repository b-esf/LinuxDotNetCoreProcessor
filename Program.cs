using System;
using System.IO;

namespace DictationProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            // iterate through subfolders of /mnt/uploads
            
            foreach( var subfolder in Directory.GetDirectories(GetUploadsPath()))
            {

                // get metadata file
                var metadataFilePath = Path.Combine(subfolder, "metadata.json");
                Console.WriteLine($"Reading {metadataFilePath}");

                // extract metadata, including audio file info, from metadata file
                var metadataFileStream = File.Open(metadataFilePath, FileMode.Append);
                

                // for each audio file listed in metadata:
                // - get absolute file path
                // - verify file checksum
                // - generate a unique checksum
                // - compress iterate
                // - create a standalone metadata file 
            }
        }

        private static string GetUploadsPath()
        {
            string appPath = AppContext.BaseDirectory;
            string uploadsPath = appPath.Substring(0, appPath.IndexOf("/bin"));
            return Path.Combine(uploadsPath, "uploads");
        }
    }
}
