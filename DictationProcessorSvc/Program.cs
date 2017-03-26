using System;
using DictationProcessorLib;
using System.IO;

namespace DictationProcessorSvc
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = Environment.GetEnvironmentVariable("USER");
            var fileSystemWatcher = new FileSystemWatcher($"/home/{user}/Projects/DictationProcessor/uploads","metadata.json");
            fileSystemWatcher.IncludeSubdirectories = true;

            while(true)
            {
                var result = fileSystemWatcher.WaitForChanged(WatcherChangeTypes.Created);
                Console.WriteLine($"New Metadata file {result.Name}.");
                var fullMetadataFilePath = Path.Combine($"/home/{user}/Projects/DictationProcessor/uploads", result.Name);
                var subfolder = Path.GetDirectoryName(fullMetadataFilePath);
                var processor = new UploadProcessor(subfolder);
                processor.Process();
            }            
        }
    }
}
