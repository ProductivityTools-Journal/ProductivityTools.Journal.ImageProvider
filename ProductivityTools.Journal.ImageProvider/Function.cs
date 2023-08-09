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
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Primitives;
//https://us-central1-ptjournal-b53b0.cloudfunctions.net/CloudBuildFunction/?bearer=eyJhbGciOiJSUzI1NiIsImtp
//http://127.0.0.1:8080/?bearer=eyJhbGciOiJSUzI1NiIsImtpZCI6IjYyM2YzNmM4MTZlZTNkZWQ2YzU0NTkyZTM4ZGFlZjcyZjE1YTBmMTMiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiUGF3ZWwgV3VqY3p5ayIsInBpY3R1cmUiOiJodHRwczovL2xoMy5nb29nbGV1c2VyY29udGVudC5jb20vYS9BQVRYQUp4eVl0dXdzTTQtZDM0b2xWWi1MdDNkc2RmcGZONm01VXJMNlMtQT1zOTYtYyIsImlzcyI6Imh0dHBzOi8vc2VjdXJldG9rZW4uZ29vZ2xlLmNvbS9wdGpvdXJuYWwtYjUzYjAiLCJhdWQiOiJwdGpvdXJuYWwtYjUzYjAiLCJhdXRoX3RpbWUiOjE2OTEzMzMxNDgsInVzZXJfaWQiOiJvY1RGd21lMEFxWXd4ckpzeXlOb0haWk9zYzgzIiwic3ViIjoib2NURndtZTBBcVl3eHJKc3l5Tm9IWlpPc2M4MyIsImlhdCI6MTY5MTM0OTk2NiwiZXhwIjoxNjkxMzUzNTY2LCJlbWFpbCI6InB3dWpjenlrQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7Imdvb2dsZS5jb20iOlsiMTAzNDE5MjE5NTI1NDQ4MDczODg1Il0sImVtYWlsIjpbInB3dWpjenlrQGdtYWlsLmNvbSJdfSwic2lnbl9pbl9wcm92aWRlciI6Imdvb2dsZS5jb20ifX0.Jj518I6ARKpCHt3uIr53hdN2fYeFq1dLGzpeXjFvvHIobjaJzCPrUWbPj-4R5ZJ1EZljOInlPZMkmqi_Yq2G788-BnphnwPCNseMsWJNkgdrAlAXYCTRdrBD4MSZZhBGRz-eQx1uoshjqWXDW0i32OkFiuFiiQGOXhH0DnH3DhmMtexi-EjdV9lhwWhoxTS6Ler_IiPaLNNH09Pt9xeCfO5A9DwW4L4QSM4ZPghCSSUHfkWyL4mHW-YDKaKFKk7NnYrctNXX1d586pwPtkdB7phio2I5QtbdzUHsOwVExkBTs6tVh3nLS37dz8AhiGk5cTR7F-z_YtaLwB-6urQmtQ
namespace ProductivityTools.Journal.ImageProvider
{
    public class Function : IHttpFunction
    {
        //const string ProjectId = "ptjournal-b53b0";
        static FirebaseAuth fierbaseApp = null;
       

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
            if (fierbaseApp == null)
            {
                var app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                });
                fierbaseApp = FirebaseAuth.GetAuth(app);
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  {app.Name}");
                Console.WriteLine($"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  {app.Options.ProjectId}");
            }
            if (fierbaseApp == null)
            {
                throw new Exception("firebase default instance is empty");
            }
           // var decodedToken = await fierbaseApp.VerifyIdTokenAsync(idToken);
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
            ////var cookies = context.Request.Cookies;
            ////StringValues bearer = string.Empty;
            ////context.Request.Query.TryGetValue("bearer",out bearer);
            string userEmail = await ValidateBearer("fdsa");
            return;
            //cookies.TryGetValue("token", out bearer);
            //string userEmail = "pwujczyk@gmail.com";//

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