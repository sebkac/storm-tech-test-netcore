using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Services
{
    public class Gravatar
    {
        private HttpClient httpClient;

        public Gravatar(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public static string GetHash(string emailAddress)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.Default.GetBytes(emailAddress.Trim().ToLowerInvariant());
                var hashBytes = md5.ComputeHash(inputBytes);

                var builder = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    builder.Append(b.ToString("X2"));
                }
                return builder.ToString().ToLowerInvariant();
            }
        }

        public async Task<string> GetProfileName(string email)
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<Response>($"{GetHash(email)}.json");
                var entry = response.entry.FirstOrDefault();
                if (entry == null) return null;
                var name = entry?.name.formatted ?? (entry.displayName ?? "");
                return name;
            }
            catch (Exception ex)
            {
                //It would be better wrap up Gravatar API in to service, where if it is not available or slow then we will not use it for while (circuit breaker)
                //And of course cache response, to not hit api on same email
                return null;
            }
        }
    }

    internal class Response
    {
        public Entry[] entry { get; set; }
    }

    internal class Entry
    {
        public Name name { get; set; }

        public Photo[] photos { get; set; }

        public string displayName { get; set; }
    }

    internal class Name
    {
        public string formatted { get; set; }
    }

    internal class Photo
    {
        public string value { get; set; }

        public string type { get; set; }
    }
}