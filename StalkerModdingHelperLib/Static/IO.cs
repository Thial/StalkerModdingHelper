using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StalkerModdingHelperLib.Static
{
    public static class IO
    {
        /// <summary>
        /// Reads a file and returns the content in the form of a string.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>The file data as string.</returns>
        public static async Task<string> ReadFileAsync(string filePath)
        {
            using var inputFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(inputFileStream, Encoding.Default);
            return await streamReader.ReadToEndAsync();
        }
        
        /// <summary>
        /// Writes a file by using the provided file data.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <param name="fileData">The file data to write into the file.</param>
        public static async Task WriteFileAsync(string filePath, string fileData)
        {
            using var outputFileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var streamWriter = new StreamWriter(outputFileStream, Encoding.Default);
            await streamWriter.WriteAsync(fileData);
        }
        
        /// <summary>
        /// Compares two files by checking their MD5 hashes.
        /// </summary>
        /// <param name="file1Path">The path of the first file.</param>
        /// <param name="file2Path">The path of the second file.</param>
        /// <returns>A boolean specifying if the files match or not.</returns>
        public static bool FileEquals(this string file1Path, string file2Path)
        {
            var file1 = new FileInfo(file1Path);
            var file2 = new FileInfo(file2Path);
            var firstHash = MD5.Create().ComputeHash(file1.OpenRead());
            var secondHash = MD5.Create().ComputeHash(file2.OpenRead());
            return !firstHash.Where((t, i) => t != secondHash[i]).Any();
        }

        /// <summary>
        /// Validates the format of the path.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns>True if the path is valid.</returns>
        public static bool IsPathValid(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}