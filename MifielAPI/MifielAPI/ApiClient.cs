using MifielAPI.Exceptions;
using MifielAPI.Utils;
using System;
using System.Net.Http;
using System.Globalization;

namespace MifielAPI
{
    public class ApiClient
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        private string _apiVersion = "/api/v1/";
        private CultureInfo _usCulture = new CultureInfo("en-US");
        private string url;

        public string Url
        {
            get { return url; }

            set
            {
                if (MifielUtils.IsValidUrl(value))
                    url = MifielUtils.RemoveLastSlashFromUrl(value);
                else
                    throw new MifielException("Invalid Url format");
            }
        }

        public ApiClient(string appId, string appSecret)
        {
            AppId = appId;
            AppSecret = appSecret;
            Url = "https://www.mifiel.com";
        }

        public HttpContent Get(string path)
        {
            return SendRequest(Rest.HttpMethod.GET, path, new StringContent(""));
        }

        public HttpContent Post(string path, HttpContent content)
        {
            return SendRequest(Rest.HttpMethod.POST, path, content);
        }

        public HttpContent Post(string path)
        {
            return SendRequest(Rest.HttpMethod.POST, path, null);
        }

        public HttpContent Post(string path, StringContent content)
        {
            return SendRequest(Rest.HttpMethod.POST, path, content);
        }

        public HttpContent Delete(string path)
        {
            return SendRequest(Rest.HttpMethod.DELETE, path, new StringContent(""));
        }

        public HttpContent Put(string path, HttpContent content)
        {
            return SendRequest(Rest.HttpMethod.PUT, path, content);
        }

        private HttpContent SendRequest(Rest.HttpMethod httpMethod, string path, HttpContent content)
        {
            string requestUri = url + _apiVersion + path;
            HttpRequestMessage requestMessage = null;
            HttpResponseMessage httpResponse = null;

            using (var client = new HttpClient())
            {
                using (content)
                {
                    switch (httpMethod)
                    {
                        case Rest.HttpMethod.POST:
                            requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, requestUri);
                            requestMessage.Content = content;
                            break;
                        case Rest.HttpMethod.GET:
                            requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, requestUri);
                            break;
                        case Rest.HttpMethod.PUT:
                            requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Put, requestUri);
                            requestMessage.Content = content;
                            break;
                        case Rest.HttpMethod.DELETE:
                            requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Delete, requestUri);
                            break;
                        default:
                            throw new MifielException("Unsupported HttpMethod: " + httpMethod.ToString());
                    }

                    SetAuthentication(httpMethod, path, requestMessage);

                    httpResponse = client.SendAsync(requestMessage).Result;

                    if (!httpResponse.IsSuccessStatusCode)
                        throw new MifielException("Status code error: " + httpResponse.StatusCode,
                                                    httpResponse.Content.ReadAsStringAsync().Result);

                    return httpResponse.Content;
                }
            }
        }

        private void SetAuthentication(Rest.HttpMethod httpMethod, string path, HttpRequestMessage requestMessage)
        {
            string contentType = requestMessage.Content == null ? "" : requestMessage.Content.Headers.ContentType.ToString();
            string date = DateTime.Now.ToUniversalTime().ToString("r", _usCulture);
            string contentMd5 = "";// MifielUtils.CalculateMD5(content);
            string signature = GetSignature(httpMethod, path, contentMd5, date, contentType);
            string authorizationHeader = string.Format("APIAuth {0}:{1}", AppId, signature);

            requestMessage.Headers.Add("Authorization", authorizationHeader);
            requestMessage.Headers.Add("Date", date);
        }

        private string GetSignature(Rest.HttpMethod httpMethod, string path, string contentMd5, string date, string contentType)
        {
            string canonicalString = string.Format("{0},{1},{2},{3},{4}",
                                            httpMethod.ToString(),
                                            contentType,
                                            contentMd5,
                                            _apiVersion + path,
                                            date);

            return MifielUtils.CalculateHMAC(AppSecret, canonicalString);
        }
    }
}
