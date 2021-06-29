using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Text_Speech.Model;
using Text_Speech.Services;

namespace Text_Speech.Controller
{
    public class TextToString
    {
        private readonly IBlob blob;

        readonly static string computerVisionSubscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
        readonly static string computerVisonEndpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");
        readonly string computerVisionUriBase = computerVisonEndpoint + "vision/v3.0/ocr";

     
        public TextToString(IBlob blob)
        {
            this.blob = blob;
        }
      
        [NonAction]
        public async Task<string> Post(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return "file not found";
                }
                string mediatype = file.ContentType;
                if (mediatype != MimeMapping.KnownMimeTypes.Jpeg && mediatype != MimeMapping.KnownMimeTypes.Png)
                {
                    return "The image must be in jpeg or png";
                }

                await blob.Upload(file);
                var downloadedfile = await blob.Download(file);

                string analyzedDetails = await MakeAnalysisRequest(downloadedfile);
                Analysis analysis = JsonConvert.DeserializeObject<Analysis>(analyzedDetails);

                var sentence = Getwords(analysis);


                return sentence;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        // To process the data on the images and retrieve the texts in string
        [NonAction]
        private async Task<string> MakeAnalysisRequest(byte[] file)
        {
            try
            {
                var client = GetClient(computerVisionSubscriptionKey);
                HttpResponseMessage httpResponse;
                using (ByteArrayContent Bytecontent = new ByteArrayContent(file))
                {
                    Bytecontent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponse = await client.PostAsync(computerVisionUriBase, Bytecontent);
                }
                string responseContent = await httpResponse.Content.ReadAsStringAsync();
                var stringContent = JToken.Parse(responseContent).ToString();
                return stringContent;
            }
            catch (Exception e)
            {
                
                return e.Message;
            }
        }
      
        [NonAction]
        private HttpClient GetClient(string key)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            return client;
        }

       
        [NonAction]
        private string Getwords(Analysis analysis)
        {
            var extractedWords = analysis.Region.SelectMany(data => data.Line.SelectMany(data => data.Word).Select(data => data.Text)).ToList();
            string compliedSentence = String.Join(" ", extractedWords);
            return compliedSentence;
        }

    }
}
