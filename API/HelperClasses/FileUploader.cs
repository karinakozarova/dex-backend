using AngleSharp;
using AngleSharp.Io;
using API.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Models.Exceptions;
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
    /// Interface for file uploader
    /// </summary>
    public interface IFileUploader
    {
        /// <summary>
        /// Uploads single file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns> path of file location </returns>
        Task<string> CopyFileToDirectory(IFormFile file, string fileName);

        /// <summary>
        /// Method deletes the file from the file server
        /// </summary>
        /// <param name="file"></param>
        void DeleteFileFromDirectory(File file);
    }

    /// <summary>
    /// Class which is responsible for uploading files
    /// </summary>
    public class FileUploader : IFileUploader
    {

        private readonly string uploadPath;

        /// <summary>
        /// File Uploader
        /// </summary>
        /// <param name="env"></param>
        public FileUploader(IWebHostEnvironment env)
        {
            uploadPath = Path.Combine(env.WebRootPath, "Resources\\");
        }

        /// <summary>
        /// Uploads single file
        /// </summary>
        /// <param name="file"> File to upload </param>
        /// <param name="fileName"> Name of file </param> 
        /// <returns> path of file location </returns>
        public async Task<string> CopyFileToDirectory(IFormFile file, string fileName)
        {
            try
            {
                if(System.IO.File.Exists(uploadPath + fileName)) throw new FileExistException(fileName);
                await using Stream sourceStream = file.OpenReadStream();
                await using FileStream destinationStream = System.IO.File.Create(uploadPath + fileName);
                await sourceStream.CopyToAsync(destinationStream);

                return fileName;
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
        /// <returns> Bool which tells if file is deleted successfully or not </returns>
        public void DeleteFileFromDirectory(File file)
        {
            if(System.IO.File.Exists(Path.Combine(uploadPath, file.Name)))
            {
                System.IO.File.Delete(Path.Combine(uploadPath, file.Name));
                return;
            }
            throw new FileNotFoundException(file.Name);
        }

    }
}