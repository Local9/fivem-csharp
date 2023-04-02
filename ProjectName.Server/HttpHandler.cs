using Newtonsoft.Json;
using System.Net.Http;

namespace ProjectName.Server
{
    internal static class HttpHandler
    {
        private static readonly HttpClient HttpClient = new(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            UseCookies = false
        });


        internal static async Task<HttpResponseMessage> OnHttpResponseMessageAsync(HttpMethod httpMethod, string url, string endpoint = "", object data = null, Dictionary<string, string> cookies = null)
        {
            Main.Logger.Debug($"HttpHandler.OnHttpResponseMessageAsync() is attempting to send a {httpMethod} request to {endpoint}");

            return await Task.Run(async () =>
            {
                var baseAddress = new Uri(url);
                HttpClient.BaseAddress = baseAddress;

                using (var request = new HttpRequestMessage(httpMethod, endpoint))
                {
                    if (cookies is not null)
                    {
                        string cookieString = "";
                        foreach (var cookie in cookies)
                        {
                            cookieString += $"{cookie.Key}={cookie.Value};";
                        }
                        request.Headers.Add("Cookie", cookieString);
                    }

                    if (data is not null)
                    {
                        request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    }

                    HttpResponseMessage response = await HttpClient.SendAsync(request).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        Main.Logger.Debug($"HttpHandler.OnHttpResponseMessageAsync() successfully sent a {httpMethod} request to {endpoint}");
                    }
                    else
                    {
                        Main.Logger.Error($"HttpHandler.OnHttpResponseMessageAsync() was unable to send a {httpMethod} request to {endpoint}");
                        Main.Logger.Error($"{response.StatusCode}");
                        Main.Logger.Error($"{response.Content.ReadAsStringAsync().Result}");
                    }

                    return response;
                }
            });
        }

        internal static Dictionary<string, string> GetCookies(HttpResponseMessage response)
        {
            var cookieHeader = response.Headers.FirstOrDefault(h => h.Key == "Set-Cookie");
            if (cookieHeader.Value == null)
            {
                return new Dictionary<string, string>();
            }

            var cookies = cookieHeader.Value
                .Select(cookie => cookie.Split(';')[0].Split('='))
                .ToDictionary(keyValue => keyValue[0], keyValue => keyValue[1]);

            return cookies;
        }

        internal static async Task<T> GetObjectFromResponseContentAsync<T>(this HttpResponseMessage httpResponseMessage)
        {
            return await Task.Run(async () =>
            {
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            });
        }
    }
}
