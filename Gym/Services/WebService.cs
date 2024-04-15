using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gym.Model;


namespace Gym.Services
{
    public class WebService
    {
        HttpClient client = new HttpClient();
        private readonly TokenService tokenService;
        public WebService(TokenService tokenService)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            this.tokenService = tokenService;
        }
        public async Task LogIn(User user)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://localhost:7062/login", content);

            if (response.IsSuccessStatusCode)
            {
                _ = tokenService.SaveTokenAsync(await response.Content.ReadAsStringAsync());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await tokenService.GetTokenAsync());

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Invalid email or password");
            } 
            else if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Banned email");
            }
            else
            {
                throw new Exception("Something went wrong");
            }
           
        }
    }
}
