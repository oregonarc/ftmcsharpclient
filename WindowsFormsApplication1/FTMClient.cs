using Newtonsoft.Json; //this library is used to handle JSON encoding/decoding
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FTMClientDemo
{
    class FTMClient
    {
        //this URL is for the staging site, and will need to be changed for use with the live site
        public string baseUri = "https://ftmstaging.fishtrax.org/api/v2/";
        //public string baseUri = "http://grover.oregonarc.com:8080/api/v2/";

        //variables to hold the username and API key
        public string userName, apiKey;

        // Instantiate the FTMClient with a given username and API key
        // (in this demo, these come from the form's text fields)
        public FTMClient(string userName, string apiKey)
        {
            this.userName = userName;
            this.apiKey = apiKey;
        }

        // Post a new trip with the given JSON data string
        public string PostTrip(string dataJson)
        {
            //add the appropriate endpoint to the base URL for the staging site
            string uri = baseUri + "trip/";

            //create a new request object
            var request = HttpWebRequest.Create(uri);

            //set the request method and define the type of content it will contain
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            //put the authorization information (username and API key) into the request's headers
            request.Headers["Authorization"] = "ApiKey " + userName + ":" + apiKey;

            //use a stream writer to serialize the JSON data into the request
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(dataJson);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //force the system to trust all SSL certificates
            //you should only do this if you trust the server you are communicating with
            //in this case, it's necessary since the staging site does not have a 'proper' SSL certificate
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

            //use try-catch blocks to handle various HTTP errors
            try
            {
                //perform the actual HTTP request
                var response = request.GetResponse();

                //once the request has completed, retrieve the data out of the response object
                var responseStream = response.GetResponseStream();
                string responseString;
                using (var streamReader = new StreamReader(responseStream))
                {
                    responseString = streamReader.ReadToEnd();
                }

                //deserialize the response data into a string
                JsonConvert.DeserializeObject(responseString);

                //return the retrieved data
                return responseString;
            }

            //handle errors when performing the request and return an error message
            catch (WebException e)
            {
                return "Error performing request: " + e.Message;
            }
        }
    }
}
