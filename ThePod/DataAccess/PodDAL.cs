using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

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
        public async Task<Shows> FindShowsAsync(string query)
        {
            var response = await _client.GetAsync($"search?id={query}");
            var jsonData = await response.Content.ReadAsStringAsync();

            Shows s = JsonSerializer.Deserialize<Shows>(jsonData);

            return s;
        }


        public async Task<Show> GetEpisodeAsync(string id)
        {
            var response = await _client.GetAsync($"search?id={id}");
            var jsonData = await response.Content.ReadAsStringAsync();
            Show shows = JsonSerializer.Deserialize<Show>(jsonData);


            
            return shows;
        }
    }
}
