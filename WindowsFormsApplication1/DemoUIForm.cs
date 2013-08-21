using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTMClientDemo
{
    public partial class DemoUIForm : Form
    {
        string fyfUrl = "";

        public DemoUIForm()
        {
            InitializeComponent();
        }

        //this method is declared as async to allow the request to run in a separate thread
        private async void btnPost_Click(object sender, EventArgs e)
        {
            //indicate that the response is pending
            txtResponse.Text = "Sending request...";
            llblFyfUrl.Enabled = false;

            //get the username, API key, and data string from the form's text boxes
            string username = txtUsername.Text;
            string apiKey = txtApiKey.Text;
            string data = txtData.Text;

            //instantiate a new FTMClient to perform the request
            FTMClient client = new FTMClient(username, apiKey);

            //use the client to post the trip
            //the 'await Task.Run()' specifies that this will run in a separate thread, allowing the UI to update while the request is pending
            string response = await Task.Run(() => client.PostTrip(data));

            //display the response data in the form
            txtResponse.Text = response;

            try
            {
                //parse the response string into JSON
                JObject responseJson = JObject.Parse(response);

                //get the FTM Trip ID of the new trip
                string tripId = (string)responseJson.SelectToken("id");
                lblTripId.Text = tripId;

                //display the Find Your Fish URL for the new trip
                fyfUrl = (string)responseJson.SelectToken("fyf_url");
                llblFyfUrl.Text = fyfUrl;
                llblFyfUrl.Enabled = true;
            }
            catch (JsonReaderException parseError)
            {
                //if the response isn't a JSON object, it means there was an error in the request, so it's OK to do nothing here
            }
        }

        private void DemoUIForm_Load(object sender, EventArgs e)
        {

        }

        private void llblFyfUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(fyfUrl);
        }
    }
}
