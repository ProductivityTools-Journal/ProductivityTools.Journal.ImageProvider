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
            var client = StorageClient.Create();

            // Create a bucket with a globally unique name
            var bucketName = "ptjournaltest1";
           // var bucket = client.CreateBucket(ProjectId, bucketName);

            foreach (var obj in client.ListObjects(bucketName))
            {
                //await context.Response.WriteAsync(obj.Name);
                Console.WriteLine(obj.Name);
            }

            // Download file
            using (var stream = File.OpenWrite("Untitled.png"))
            {
                client.DownloadObject(bucketName, "Untitled.png", stream);
              
            }
            var fileInfo = new System.IO.FileInfo("Untitled.png");
            var fileInfo2 = new Microsoft.Extensions.FileProviders.Physical.PhysicalFileInfo(fileInfo);
            await context.Response.SendFileAsync(fileInfo2);


            await context.Response.WriteAsync("Hello World!");
        }
    }
}