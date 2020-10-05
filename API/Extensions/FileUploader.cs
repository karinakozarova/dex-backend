using AngleSharp;
using AngleSharp.Io;
using API.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Models.File;

namespace API.Extensions
{
    /// <summary>
    /// File already exists exception
    /// </summary>
    [Serializable]
    public class FileExistException : Exception
    {
        /// <summary>
        /// File already exist constructor
        /// </summary>
        /// <param name="name"></param>
        public FileExistException(string name)
            : base(String.Format("File {0} already exists", name))
        { }
    }

    /// <summary>
    /// Interface for file uploader
    /// </summary>
    public interface IFileUploader
    {

        /// <summary>
        /// Remove specials characters from string
        /// </summary>
        /// <param name="str"></param>
        /// <returns> String without special characters</returns>
        string RemoveSpecialCharacters(string str);


        /// <summary>
        /// Uploads single file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns> path of file location </returns>
        Task<string> UploadSingleFile(IFormFile file, string fileName);

        /// <summary>
        /// Method deletes the file from the file server
        /// </summary>
        /// <param name="file"></param>
        void DeleteFile(File file);

    }

    /// <summary>
    /// Class which is responsible for uploading files
    /// </summary>
    public class FileUploader : IFileUploader
    {

        private readonly string UploadPath;
        private readonly string ServerRoot;

        public FileUploader(IWebHostEnvironment env)
        {
            UploadPath = Path.Combine(env.WebRootPath, "Resources\\");
            ServerRoot = Path.Combine("https://localhost:5001/Resources/");
        }

        /// <summary>
        /// Removes special characters for string to avoid problems
        /// </summary>
        /// <param name="str"></param>
        /// <returns> String without special characters</returns>
        public string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach(char c in str)
            {
                if((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Uploads single file
        /// </summary>
        /// <param name="file"> File to upload </param>
        /// <param name="fileName"> Name of file </param> 
        /// <returns> path of file location </returns>
        public async Task<string> UploadSingleFile(IFormFile file, string fileName)
        {
            try
            {
                if(!System.IO.File.Exists(UploadPath + fileName))
                {
                    await using(Stream sourceStream = file.OpenReadStream())
                    {
                        await using(FileStream destinationStream = System.IO.File.Create(UploadPath + fileName))
                        {
                            await sourceStream.CopyToAsync(destinationStream);
                        }
                    }

                    return ServerRoot + fileName;
                }

                throw new FileExistException(fileName);
            } catch(Exception e)
            {
                Log.Logger.Error(e, "Unexpected error");
                throw e;
            }
        }

        /// <summary>
        /// Method deletes the file from the file server
        /// </summary>
        /// <param name="file"></param>
        /// <returns> Bool which tells if file is deleted succesfully or not </returns>
        public void DeleteFile(File file)
        {
            if(System.IO.File.Exists(Path.Combine(UploadPath, file.Name)))
            {
                System.IO.File.Delete(Path.Combine(UploadPath, file.Name));
                return;
            }
            throw new FileNotFoundException(file.Name);
        }

    }
}
