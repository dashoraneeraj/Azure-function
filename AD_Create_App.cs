using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Configuration;

namespace Ad_Create_App.Function
{
    public static class CreateADUserClass
    {
        [FunctionName("CreateADUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public static string CreateADUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] ReqObj reqObj)
        {

            string sResult = CreateUser(reqObj.displayName, reqObj.mailNickname, reqObj.userPrincipalName, reqObj.passwordProfile, reqObj.accountEnabled, reqObj.accessToken);

            if (sResult == "Pass")
            {
                return "User Created Successfully";
            }
            else
            {
                return "User Creation Failed";
            }
 
        }

         private static string CreateUser(string sDisplayName, string sMailNickName, string sUserPrincpalName, PasswordProfile passwordProfile, bool bAccountEnabled, string sAccessToken)
        {
            try
            {

                //Creating Json object
                JObject jObjectbody = new JObject();
                jObjectbody.Add("accountEnabled", Convert.ToBoolean(bAccountEnabled));
                jObjectbody.Add("displayName", sDisplayName);
                jObjectbody.Add("mailNickname", sMailNickName);
                jObjectbody.Add("userPrincipalName", sUserPrincpalName);
                jObjectbody.Add("passwordProfile", JsonConvert.SerializeObject(passwordProfile));

                //Creating client with URL
                var client = new RestClient("https://graph.microsoft.com/v1.0/users");
                client.Timeout = -1;

                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + sAccessToken);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jObjectbody, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                return "Pass";
                //Console.WriteLine(response.Content);
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                return "Failed";

            }
        }
    }

        public class ReqObj
    {
        public bool accountEnabled { get; set; }
        public string displayName { get; set; }
        public string mailNickname { get; set; }
        public string userPrincipalName { get; set; }
        public PasswordProfile passwordProfile { get; set; }
        public string accessToken {get; set;}
    }

     public class PasswordProfile
    {
        public bool forceChangePasswordNextSignIn { get; set; }
        public string password { get; set; }
    }
}

