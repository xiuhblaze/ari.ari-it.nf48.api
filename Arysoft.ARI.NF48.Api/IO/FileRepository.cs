using Arysoft.ARI.NF48.Api.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Arysoft.ARI.NF48.Api.IO
{
    public class FileRepository
    {
        public static string UploadFile(HttpPostedFile file, string virtualPath, string newFilename, string[] allowedExtensions = null)
        {
            if (file == null || file.ContentLength == 0)
                throw new BusinessException("The file to upload is empty");

            if (allowedExtensions != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!allowedExtensions.Contains(extension))
                    throw new BusinessException($"The file extension '{extension}' is not allowed");
            }

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
                    throw new BusinessException($"FileRepository.DeleteFile: {ex.Message}");
                }
            }
            // else throw new BusinessException($"The file '{filename}' to delete not exist in route '{deletePath}'.");
            
            return true;
        } // DeleteFile

        public static bool DeleteDirectory(string virtualPath)
        {
            string deletePath = HostingEnvironment.MapPath(virtualPath);

            if (Directory.Exists(deletePath))
            {
                try
                {
                    Directory.Delete(deletePath, true);
                }
                catch (Exception ex)
                {
                    throw new BusinessException($"FileRepository.DeleteDirectory: {ex.Message}");
                }
            }
            // else throw new BusinessException($"The directory trying to delete does not exist '{deletePath}'");

            return true;
        } // DeleteDirectory
    }
}