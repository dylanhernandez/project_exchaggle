using Exchaggle.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Exchaggle.Services
{
    public class ImageService
    {
        private ExchaggleDbContext db;
        private List<string> imageTypes;

        public ImageService(ExchaggleDbContext databaseContext)
        {
            db = databaseContext;
            imageTypes = new List<string>() {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };
        }

        public Image AddImage(HttpPostedFileBase uploadedImage, Item referenceItem)
        {
            if (uploadedImage.ContentLength > 0)
            {
                if (imageTypes.Contains(uploadedImage.ContentType))
                {
                    try
                    {
                        string fileExtension = getExtension(uploadedImage.ContentType);
                        string fileStamp = string.Format("{0}", new Random().Next(10000, 99999));
                        string fileName = string.Format(Resources.Imaging.ImageFileName, referenceItem.Name, fileStamp, fileExtension);
                        string uploadDirectory = "~\\Storage";
                        string uploadSource = "\\Storage";
                        string imagePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadDirectory), fileName);
                        string imageUrl = Path.Combine(uploadSource, fileName);
                        WriteFileFromStream(uploadedImage.InputStream, imagePath);
                        Image newImage = new Image();
                        newImage.ItemId = referenceItem.ItemId;
                        newImage.ImageName = fileName;
                        newImage.ImageSource = imageUrl;
                        db.Image.Add(newImage);
                        db.SaveChanges();
                        return newImage;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }            
            return null;
        }

        public void DeleteImage(int referenceItemId)
        {
            Image uploadedImage = db.Image.Where(i => i.ItemId == referenceItemId).FirstOrDefault();
            if (uploadedImage != null)
            {
                string uploadDirectory = "~\\Storage";
                string imagePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadDirectory), uploadedImage.ImageName);
                DeleteFileInfo(imagePath);
                db.Image.Remove(uploadedImage);
                db.SaveChanges();
            }
        }

        public Image ServeImage(Item referenceItem)
        {
            if (referenceItem != null)
            {
                Image uploadedImage = db.Image.Where(i => i.ItemId == referenceItem.ItemId).FirstOrDefault();
                if (uploadedImage != null)
                {
                    return uploadedImage;
                }
            }
            return null;
        }

        public Image ServeImage(int referenceItemId)
        {
            Image uploadedImage = db.Image.Where(i => i.ItemId == referenceItemId).FirstOrDefault();
            if (uploadedImage != null)
            {
                return uploadedImage;
            }
            return null;
        }

        private string getExtension(string contentType)
        {
            string [] components = contentType.Split('/');
            if (!string.IsNullOrWhiteSpace(components[1]))
            {
                return components[1];
            }
            return string.Empty;
        }

        private void WriteFileFromStream(Stream stream, string toFile)
        {
            using (FileStream fileToSave = new FileStream(toFile, FileMode.Create))
            {
                stream.CopyTo(fileToSave);
            }
        }

        private void DeleteFileInfo(string toFile)
        {
            FileInfo queryFileInformation = new FileInfo(toFile);
            queryFileInformation.Delete();
        }
    }
}