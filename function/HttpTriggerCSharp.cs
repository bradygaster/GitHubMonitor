
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Company.Function
{
    public static class HttpTriggerCSharp
    {
        [FunctionName("HttpTriggerCSharp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, 
            [SignalR(HubName="repomonitor")] IAsyncCollector<SignalRMessage> messages,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var repoUrl = data.repository.html_url;
            var repoName = data.repository.name;
            var senderLogin = data.sender.login;
            var senderAvatar = data.sender.avatar_url;
            var commitMessage = data.head_commit.message;
            var msg = string.Format($"Repo {repoName} ({repoUrl}) updated by {senderLogin} with commit message '{commitMessage}'");

            await messages.AddAsync(new SignalRMessage {
                Target = "pushReceived",
                Arguments = new [] { msg }
            });

            return (ActionResult)new OkObjectResult(msg);
        }
    }
}
