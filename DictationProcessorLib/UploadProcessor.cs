using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO.Compression;

namespace DictationProcessorLib
{
    public class UploadProcessor
    {
        public string subfolder { get; set; }

        public UploadProcessor(string subfolder)
        {
            this.subfolder = subfolder;
        }

        public void Process()
        {            
            // get metadata file
            var metadataFilePath = Path.Combine(subfolder, "metadata.json");
            Console.WriteLine($"Reading {metadataFilePath}");

            // extract metadata, including audio file info, from metadata file
            var metadataCollection = GetMetadata(metadataFilePath);

            // for each audio file listed in metadata:
            foreach (var metadata in metadataCollection)
            {
                // - get absolute file path
                var audioFilePath = Path.Combine(subfolder, metadata.File.FileName);
                // - verify file checksum
                var md5Checksum = GetChecksum(audioFilePath);
                if(md5Checksum.Replace("-","").ToLower() != metadata.File.Md5Checksum)
                {
                    throw new Exception("Incorrect Checksum!");
                }
                // - generate a unique Id
                var uniqueId = Guid.NewGuid();
                metadata.File.FileName = uniqueId + ".WAV";
                var user = Environment.GetEnvironmentVariable("USER");
                var newPath = Path.Combine($"/home/{user}/Projects/DictationProcessor/ready_for_transcription", uniqueId + ".WAV");
                // - compress
                CreateCompressedFile(audioFilePath, newPath);
                // - create a standalone metadata file 
                SaveSingleMetadata(metadata, newPath + ".json");
            }            
        }

        
        private static List<Metadata> GetMetadata(string metadataFilePath)
        {
            var metadataFileStream = File.Open(metadataFilePath, FileMode.Open);
                
            var settings = new DataContractJsonSerializerSettings{
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ssZ")
            };
            var serializer = new DataContractJsonSerializer(typeof(List<Metadata>), settings);
            var metadataCollection = (List<Metadata>)serializer.ReadObject(metadataFileStream);
            return metadataCollection;
        }

        private static string GetChecksum(string audioFilePath)
        {
            var fileStream = File.Open(audioFilePath, FileMode.Open);
            var md5 = System.Security.Cryptography.MD5.Create();
            var md5Bytes = md5.ComputeHash(fileStream);
            string result = BitConverter.ToString(md5Bytes);

            fileStream.Dispose();
            return result;
        }

        private static void CreateCompressedFile(string inputFilePath, string outputFilePath)
        {
            outputFilePath += ".gz";
            Console.WriteLine($"Creating {outputFilePath} ");

            var inputFileStream = File.Open(inputFilePath, FileMode.Open);
            var outputFileStream = File.Create(outputFilePath);
            var gzipStream = new GZipStream(outputFileStream, CompressionLevel.Optimal);

            inputFileStream.CopyTo(gzipStream);
        }

        private static void SaveSingleMetadata(Metadata metadata, string metadataFilePath)
        {
            Console.WriteLine($"Creating {metadataFilePath}");
            var metadataFileStream = File.Open(metadataFilePath, FileMode.Create);
                
            var settings = new DataContractJsonSerializerSettings{
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ssZ")
            };
            var serializer = new DataContractJsonSerializer(typeof(Metadata), settings);
            serializer.WriteObject(metadataFileStream, metadata);
        }
    }
}
