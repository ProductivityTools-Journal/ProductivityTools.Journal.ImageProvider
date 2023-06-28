using Google.Api;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Storage.V1;
using Google.Events.Protobuf.Cloud.Datastream.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;
using FirebaseAdmin.Auth;
using FirebaseAdmin;

namespace ProductivityTools.Journal.ImageProvider
{
    public class Function : IHttpFunction
    {

        const string ProjectId = "ptjournal-b53b0";

        private string GetValue(HttpContext context, string key, string throwMessage)
        {
            
            var keyExists = context.Request.Query.Keys.Contains(key);
            if (keyExists == false)
            {
                throw new Exception(throwMessage);
            }
            var value = context.Request.Query[key];
            return value;
        }

        private async Task<string> ValidateBearer(string idToken)
        {
            if (FirebaseAuth.DefaultInstance == null)
            {
                FirebaseApp.Create();
            }
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            string uid = decodedToken.Uid;
            if (decodedToken.Claims.ContainsKey("email"))
            {
                var email = decodedToken.Claims["email"];
                return email.ToString();
            }
            else
            {
                throw new Exception("No email in claims");
            }
            throw new Exception("Some error but not sure what");
        }
    

        public async Task HandleAsync(HttpContext context)
        {
            var token= GetValue(context, "token", "Please provide filename");
            string userEmail = "pwujczyk@gmail.com";//await ValidateBearer(token);
            string user = userEmail.Replace("@", "-").Replace(".", "-");
            var fileName = GetValue(context, "fileName", "Please provide filename");
            //await context.Response.WriteAsync(fileName);
            //await context.Response.WriteAsync(userEmail);
            var fullName = $"{user}/{fileName}";
            var bucketName = string.Format($"ptjournalimages");
            var client = StorageClient.Create();
            context.Response.ContentType = "image/jpeg";
            using (var stream = new MemoryStream())
            {
                client.DownloadObject(bucketName, fullName, stream);
                await context.Response.BodyWriter.WriteAsync(stream.ToArray());
            }
        }
    }
}
/*
 * notes:
 *    foreach (var obj in client.ListObjects(bucketName))
            {
                Console.WriteLine(obj.Name);

                await context.Response.WriteAsync(obj.Name);
            }
 */