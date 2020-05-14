﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Atlas.MatchPrediction.Functions.Functions
{
    public class HealthCheck
    {
        [FunctionName("HealthCheck")]
        public static OkObjectResult Check([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            const string responseMessage = "This HTTP triggered function executed successfully";
            return new OkObjectResult(responseMessage);
        }
    }
}