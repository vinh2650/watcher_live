using System;
using System.Configuration;
using System.IO;
using System.Web;
using Common.Helpers;
using MimeKit;

namespace ReplyEmailsService.Services
{
    public class AttachmentService
    {
        public string SaveAttachment(MimePart attachment)
        {
            if (attachment == null) return null;

            using (var memory = new MemoryStream())
            {
                attachment.ContentObject.DecodeTo(memory);
                memory.Seek(0, SeekOrigin.Begin);
                var byteArrayFromFileField = memory.GetBuffer();

                var guidFolder = Guid.NewGuid().ToString();

                // Initialize Blob
                var blobContainer = AzureHelper.DocumentBlobContainer;
                var blob = blobContainer.GetBlockBlobReference(guidFolder + "_" + attachment.FileName);

                // Set the blob content type
                blob.Properties.ContentType = MimeMapping.GetMimeMapping(attachment.FileName);

                // Upload file into blob storage, basically copying it from local disk into Azure
                using (Stream stream = new MemoryStream(byteArrayFromFileField))
                {
                    blob.UploadFromStream(stream);
                }

                return string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["MediaBaseUrl"],
                    ConfigurationManager.AppSettings["DocumentFolderName"], guidFolder + "_" + attachment.FileName);
            }
        }
    }
}
