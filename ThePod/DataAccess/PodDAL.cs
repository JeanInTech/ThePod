using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using ThePod.Models;


namespace ThePod.DataAccess
{
    public class PodDAL
    {
        private readonly HttpClient _client;
        public PodDAL(HttpClient client)
        {
            _client = client;
        }
        private static async Task<AccessToken> GetToken()
        {
            string clientId = Secret.ClientId;
            string clientSecret = Secret.ClientSecret;
            string credentials = String.Format("{0}:{1}", clientId, clientSecret);

            using (var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = await client.PostAsync("https://accounts.spotify.com/api/token", requestBody);
                var response = await request.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<AccessToken>(response);
            }
        }
        public async Task<RootobjectEpisodes> SearchEpisodeNameAsync(string query) // returns object that is model type: "Episdodes"
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();
            var encodedQuery = Uri.EscapeDataString(query);

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"search?q={encodedQuery}&type=episode&market=US");
            var jsonData = await response.Content.ReadAsStringAsync();

            RootobjectEpisodes ro = JsonSerializer.Deserialize<RootobjectEpisodes>(jsonData);

            return ro;
        }

        public async Task<RootobjectEpisodes> MoreSearchEpisodeAsync(string query, int offset)
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();
            var encodedQuery = Uri.EscapeDataString(query);

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"search?q={encodedQuery}&type=episode&market=US&offset={offset}&limit=20");
            var jsonData = await response.Content.ReadAsStringAsync();

            RootobjectEpisodes ro = JsonSerializer.Deserialize<RootobjectEpisodes>(jsonData);

            return ro;
        }
        public async Task<RootEpisodes> SearchEpisodeIdAsync(string epId) //returns object that is model type: "Rootepisode"
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"episodes?ids={epId}&market=US");
            var jsonData = await response.Content.ReadAsStringAsync();

            RootEpisodes re = JsonSerializer.Deserialize<RootEpisodes>(jsonData);

            return re;
        }
        public async Task<Rootobject> SearchShowNameAsync(string query) //returns object that is model type: "shows"
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();
            var encodedQuery = Uri.EscapeDataString(query);

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"search?q={encodedQuery}&type=show&market=US");
            var jsonData = await response.Content.ReadAsStringAsync();

            Rootobject ro = JsonSerializer.Deserialize<Rootobject>(jsonData);

            return ro;
        }
        public async Task<Rootobject> MoreSearchShowAsync(string query, int offset)
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();
            var encodedQuery = Uri.EscapeDataString(query);

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"search?q={encodedQuery}&type=show&market=US&offset={offset}&limit=20");
            var jsonData = await response.Content.ReadAsStringAsync();

            Rootobject ro = JsonSerializer.Deserialize<Rootobject>(jsonData);

            return ro;
        }
        public async Task<RootShows> SearchShowIdAsync(string shoId) // returns object that is model type: "Rootshows"
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"shows?ids={shoId}&market=US");
            var jsonData = await response.Content.ReadAsStringAsync();

            RootShows re = JsonSerializer.Deserialize<RootShows>(jsonData);

            return re;
        }
        public async Task<EpisodesByPodId> SearchEpbyPodIdAsync(string shoId) // returns object that is model type: "EpisodesByPodId"
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"shows/{shoId}/episodes?market=US");
            var jsonData = await response.Content.ReadAsStringAsync();

            EpisodesByPodId ebp = JsonSerializer.Deserialize<EpisodesByPodId>(jsonData);

            return ebp;
        }
        public async Task<EpisodesByPodId> MoreEpbyPodIdAsync(string shoId, int offset)
        {
            var token = await GetToken();
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://api.spotify.com/v1/");
            client.DefaultRequestHeaders.Add("Authorization", $"{token.token_type} {token.access_token}");
            var response = await client.GetAsync($"shows/{shoId}/episodes?&market=US&offset={offset}&limit=20");
            var jsonData = await response.Content.ReadAsStringAsync();

            EpisodesByPodId ro = JsonSerializer.Deserialize<EpisodesByPodId>(jsonData);

            return ro;
        }
    }
}