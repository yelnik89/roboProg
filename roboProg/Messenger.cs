using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using System.Net;

namespace roboProg
{
    class Messenger
    {
        private const string PROPERTY_URI = "/Services/";
        private const string THINGS_URI = "/Thingworx/Things/";

        private string authInfo;
        private string address;
        private string authorizationType;

        public Messenger(string authInfo, string address, string authorizationType)
        {
            this.authInfo = authInfo;
            this.address = address + THINGS_URI;
            this.authorizationType = authorizationType;
        }

        public async Task<string> getAllThings()
        {
            string json = await httpRequestAsync("GET", this.address);
            return json;
        }

        public async Task<string> reqestToService(string thingName, string serviceName, string json = "")
        {
            string address = getPropertyAddress(thingName, serviceName);
            string result = await httpRequestAsync("POST", address, json);

            return result;
        }

        private async Task<string> httpRequestAsync(string method, string url, string json = "")
        {
            string result = "error";
            HttpWebRequest req = createHttpRequest(method, url);
            await Task.Run(() => {
                if (method.Equals("POST") || method.Equals("PUT")) sendData(req, json);
                result = trySendRequest(req);
            });
            return result;
        }

        private HttpWebRequest createHttpRequest(string method, string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            authorization(req);
            req.Accept = "application/json";
            return req;
        }
        
        private string trySendRequest(HttpWebRequest req)
        {
            string result;
            try
            {
                result = request(req);
            }
            catch (Exception e)
            {
                log(e.Message);
                result = e.Message;
            }
            return result;
        }



        private string request(HttpWebRequest req)
        {
            using (var responseStream = req.GetResponse().GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                string result = reader.ReadToEnd();
                log(result);
                return result;
            }
        }

        private string getPropertyAddress(string name, string serviceName)
        {
            string address = this.address + name + PROPERTY_URI + serviceName;
            if (address.Equals("")) return "";
            log(address);
            return address;
        }

        private void authorization(HttpWebRequest req)
        {
            if (this.authorizationType.Equals("Basic")) req.Headers["Authorization"] = "Basic " + this.authInfo;
            if (this.authorizationType.Equals("appkey")) req.Headers["appkey"] = this.authInfo;
        }

        private void sendData(HttpWebRequest req, string json)
        {
            req.ContentType = "application/json";
            using (var requestStream = req.GetRequestStream())
            using (var streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(json);
            }
        }

        private string BasicAuthorization()
        {
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(this.authInfo));
            return "Basic " + authInfo;
        }

        private void log(string text)
        {
            Loger loger = Loger.GetInstance();
            loger.WriteLog(text);
        }
    }
}
