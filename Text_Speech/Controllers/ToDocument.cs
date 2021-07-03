using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Text_Speech.Controller;
using Text_Speech.Services;

namespace Text_Speech.Controllers
{
    [SwaggerResponse((int)HttpStatusCode.OK, "Returns if sucessful")]
    [SwaggerResponse((int)HttpStatusCode.NotFound, "Returns if not found")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "Returns image if not accepted")]

    [Route("api/ttd")]
    [ApiController]
    public class ToDocument : ControllerBase
    {
        readonly TextToString toString;

        FileInfo documentFile;
        FileInfo documentFile1;

        readonly IBlob blob;

        public ToDocument(TextToString toString, IBlob blob)
        {
            this.toString = toString;
            this.blob = blob;
        }

        ///<param name="file">
        ///The text image can either be in jpeg or png
        ///</param>
        /// <summary>
        /// Converts a text image into a document File.
        /// The text image can either be in jpeg or png.
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        [HttpPost("toDocs")]
        public async Task<ActionResult<string>> TextToDocument(IFormFile file)
        {
            try
            {
                var sentence = await toString.Post(file);
                if (sentence == "file not found")
                {
                    return NotFound("File not found");
                }
                if (sentence == "The image must be in jpeg or png")
                {
                    return BadRequest("The image must be in jpeg or png");
                }
                ToDocs(sentence);
                var url = blob.GetUri("Document.Docx");
               
                return Ok($"Sucessful, copy on this {url}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        ///<param name="file">
        ///The text image can either be in jpeg or png
        ///</param>
        /// ///<param name="document">
        ///The docx or doc file
        ///</param>
        /// <summary>
        /// Converts a text image and stores it into an existing document File.
        /// The text image can either be in jpeg or png.
        /// A doc media type is only accepted
        /// </summary>
        /// 
        /// <returns>A string status</returns>
        [HttpPost("toExistingDocs")]
        public async Task<ActionResult<string>> TextToExistingDocument(IFormFile file,IFormFile document)
        {
            try
            {
                string mediatype = document.ContentType;
                var sentence = await toString.Post(file);
                if (sentence == "file not found" || document == null)
                {
                    return NotFound("File not found");
                }
                if (sentence == "The image must be in jpeg or png")
                {
                    return BadRequest("The image must be in jpeg or png");
                }
               if (mediatype != MimeMapping.KnownMimeTypes.Bin)
                {
                    return BadRequest("The document must be in a docx or doc file");
                }
                await blob.Upload(document);
                await ToExistingDocs(sentence,document);  
                var url = blob.GetUri("Document.Docx");
                return Ok($"Sucessful, copy on this {url}");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [NonAction]
        private void ToDocs(string sentence) 
        {
            documentFile = new FileInfo("Document.Docx");
            using (var file = new FileStream(documentFile.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.Write(sentence);
                    writer.Close();
                }
                file.Close();
            }
            blob.UploadFile(documentFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
        }
        [NonAction]
        private async Task ToExistingDocs(string sentence,IFormFile documentFile) 
        {
            var sentences = await blob.DownloadFile(documentFile.FileName);
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var line in sentences) 
            {
                stringBuilder.Append(line);
            }
            stringBuilder.Append(" ");
            stringBuilder.Append(sentence);
            ToDocs(stringBuilder.ToString());
        } 
    }
}
