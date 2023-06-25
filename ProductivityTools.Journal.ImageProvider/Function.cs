using Google.Cloud.Functions.Framework;
using Google.Cloud.Storage.V1;
using Google.Events.Protobuf.Cloud.Datastream.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProductivityTools.Journal.ImageProvider
{
    public class Function : IHttpFunction
    {
        const string ProjectId = "ptjournal-b53b0";
        public async Task HandleAsync(HttpContext context)
        {
            var fileNameExists = context.Request.Query.Keys.Contains("fileName");
            if (fileNameExists == false)
            {
                throw new Exception("Please provide filename");
            }
            var fileName= context.Request.Query["fileName"];
            await context.Response.WriteAsync(fileName);



            var client = StorageClient.Create();
            var bucketName = "ptjournaltest1";
            foreach (var obj in client.ListObjects(bucketName))
            {
                Console.WriteLine(obj.Name);
                
                await context.Response.WriteAsync(obj.Name);
            }
            return;
            context.Response.ContentType= "image/jpeg";
            using (var stream = new MemoryStream())
            {
                client.DownloadObject(bucketName, "Untitled.png", stream);
                await context.Response.BodyWriter.WriteAsync(stream.ToArray());
            }

            
        }
    }
}