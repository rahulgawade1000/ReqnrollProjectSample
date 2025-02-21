using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;
using RestSharp;
using Aerospike.Client;
using Newtonsoft.Json.Linq;

namespace ReqnrollScript.StepDefinitions;
[Binding]
public class ApiValidationSteps
{
        private string _apiUrl;
        private JObject _apiResponse;
        private AerospikeClient _aerospikeClient;
        private Key _aerospikeKey;
        private string _namespace = "test";  // Change as per your namespace
        private string _setName = "users";   // Change as per your dataset

        public ApiValidationSteps()
        {
            // Initialize Aerospike Client
            _aerospikeClient = new AerospikeClient("127.0.0.1", 3000); // Update with actual host/port
        }

        [Given(@"I have an API endpoint ""(.*)""")]
        public void GivenIHaveAnApiEndpoint(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        [When(@"I fetch data for user ""(.*)""")]
        public void WhenIFetchDataForUser(string userId)
        {
            string fullUrl = $"{_apiUrl}/{userId}";
            var client = new RestClient(fullUrl);
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "API call failed!");

            _apiResponse = JObject.Parse(response.Content);
        }

        [Then(@"the API response should match the Aerospike dataset for ""(.*)""")]
        public void ThenTheAPIResponseShouldMatchTheAerospikeDatasetFor(string userId)
        {
            _aerospikeKey = new Key(_namespace, _setName, userId);

            // Fetch Data from Aerospike
            Record aerospikeRecord = _aerospikeClient.Get(null, _aerospikeKey);
            Assert.IsNotNull(aerospikeRecord, $"No record found in Aerospike for user {userId}!");

            // Example: Comparing fields
            Assert.AreEqual(_apiResponse["name"].ToString(), aerospikeRecord.GetString("name"), "Name does not match!");
            Assert.AreEqual(_apiResponse["age"].ToString(), aerospikeRecord.GetInt("age").ToString(), "Age does not match!");
            Assert.AreEqual(_apiResponse["email"].ToString(), aerospikeRecord.GetString("email"), "Email does not match!");
        }
    }
}
