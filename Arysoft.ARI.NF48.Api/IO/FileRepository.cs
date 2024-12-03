using Arysoft.ARI.NF48.Api.Exceptions;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Arysoft.ARI.NF48.Api.IO
{
    public class FileRepository
    {
        public static string UploadFile(HttpPostedFile file, string virtualPath, string newFilename)
        {
            if (file == null || file.ContentLength == 0)
                throw new BusinessException("The file to upload is empty");

            string uploadPath = HostingEnvironment.MapPath(virtualPath);

            try
            {
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var extension = Path.GetExtension(file.FileName);
                newFilename += extension;
                 var fullPath = Path.Combine(uploadPath, newFilename);

                file.SaveAs(fullPath);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"FileRepository.UploadFileAsync: {ex.Message}");
            }

            return newFilename;
        } // UploadFile

        public static bool DeleteFile(string virtualPath, string filename)
        {
            string deletePath = HostingEnvironment.MapPath(virtualPath);

            if (string.IsNullOrEmpty(filename))
                throw new BusinessException("Filename missing");

            string fullPath = Path.Combine(deletePath, filename);

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"File.Repository.DeleteFile: {ex.Message}");
                }
            }
            else throw new BusinessException($"The file '{filename}' to delete not exist in route '{deletePath}'.");
            
            return true;
        } // DeleteFile
    }
}