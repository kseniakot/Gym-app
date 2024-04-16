using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gym.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;


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

        // LOG USER IN
        public async Task LogIn(User user)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://localhost:7062/login", content);

            if (response.IsSuccessStatusCode)
            {
                await tokenService.SaveTokenAsync((await response.Content.ReadAsStringAsync()).Trim('"'));
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


        //DECRYPT TOKEN AND GET USER INFO
        public async Task<User> GetUserFromToken()
        {
          
            var stream = await tokenService.GetTokenAsync();
           
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
           

            var tokenS = jsonToken as JwtSecurityToken;

            var user = new User()
            {
                Email = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Email).Value,
                Id = int.Parse(tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value)
            };

            return user;
        }

        //REMOVE USER 

        public async Task RemoveUser(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7062/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

        //ADD USER
        public async Task AddUser(User user)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://localhost:7062/users", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

        //BAN USER
        public async Task BanUser(int id)
        {
            HttpResponseMessage response = await client.PutAsync($"https://localhost:7062/users/ban/{id}", null);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

        //GET ALL USERS
        public async Task<List<User>> GetUsers()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:7062/users");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<User>>(content, options);

            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }

        //GET BANNED USERS
        public async Task<List<User>> GetBannedUsers()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:7062/users/banned");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<User>>(content, options);

            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }

        //GET UNBANNED USERS
        public async Task<List<User>> GetUnbannedUsers()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:7062/users/unbanned");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
              
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                 return JsonSerializer.Deserialize<List<User>>(content, options);
                
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }
    }
}
