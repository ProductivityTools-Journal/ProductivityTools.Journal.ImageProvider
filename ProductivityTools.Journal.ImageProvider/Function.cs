using Google.Cloud.Functions.Framework;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System;
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
                await context.Response.WriteAsync(obj.Name);
                Console.WriteLine(obj.Name);
            }

            await context.Response.WriteAsync("Hello World!");
        }
    }
}