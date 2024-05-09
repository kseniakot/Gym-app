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
using Gym.Exceptions;
using System.Net;

namespace Gym.Services
{
    public class WebService
    {
        HttpClient client;
        private readonly TokenService tokenService;
      
        public WebService(TokenService tokenService, HttpClient client)
        {

            this.client = client;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            this.client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            this.tokenService = tokenService;
        }

        // LOG USER IN
        public async Task LogIn(User user)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
           // HttpResponseMessage response = await client.PostAsync("https://localhost:7062/login", content);
             HttpResponseMessage response = await client.PostAsync("https://192.168.56.1:7062/login", content);

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
                Id = int.Parse(tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value),
                Password = tokenS.Claims.First(claim => claim.Type == "Password").Value,
                Name = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Name).Value,
                PhoneNumber = tokenS.Claims.First(claim => claim.Type == ClaimTypes.MobilePhone).Value,
                IsBanned = bool.Parse(tokenS.Claims.First(claim => claim.Type == "IsBanned").Value),
                UserMemberships = new List<MembershipInstance>(),
                UserFreezes = new List<FreezeInstance>()
               
            };
            Debug.WriteLine("User from token: " + user.Email);

            return user;
        }

        //REMOVE USER 

        public async Task RemoveUser(int id)
        {
            // HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7062/users/{id}");
            HttpResponseMessage response = await client.DeleteAsync($"https://192.168.56.1:7062/users/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

        //ADD USER
        public async Task AddUserAsync(User user)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://192.168.56.1:7062/users", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //BAN USER
        public async Task BanUser(int id)
        {
            HttpResponseMessage response = await client.PutAsync($"https://192.168.56.1:7062/users/ban/{id}", null);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

        //GET ALL USERS
        public async Task<List<User>> GetUsers()
        {
            HttpResponseMessage response = await client.GetAsync("https://192.168.56.1:7062/users");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<User>>(content, options);

            } 
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }

        //GET BANNED USERS
        public async Task<List<User>> GetBannedUsers()
        {
            HttpResponseMessage response = await client.GetAsync("https://192.168.56.1:7062/users/banned");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<User>>(content, options);

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }

        //GET UNBANNED USERS
        public async Task<List<User>> GetUnbannedUsers()
        {
            HttpResponseMessage response = await client.GetAsync("https://192.168.56.1:7062/users/unbanned");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
              
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                 return JsonSerializer.Deserialize<List<User>>(content, options);
                
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }

        //ISUSEREXIST
        public async Task<bool> IsUserExistAsync(string email)
        {
            HttpResponseMessage response = await client.GetAsync($"https://192.168.56.1:7062/users/exist/{email}");
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        } 
        

        //GET ALL MEMBERSHIPS
        public async Task<List<Membership>> GetAllMemberships()
        {
            HttpResponseMessage response = await client.GetAsync("https://192.168.56.1:7062/memberships");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Membership>>(content, options);

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }


        //DELETE MEMBERSHIP
        public async Task DeleteMembership(Membership membership)
        {
            HttpResponseMessage response = await client.DeleteAsync($"https://192.168.56.1:7062/memberships/{membership.Id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

  

        //GET MEMBERSHIP BY ID
        public async Task<Membership> GetMembershipById(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"https://192.168.56.1:7062/memberships/{id}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Membership>(content, options);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }

        //DOES MEMBERSHIP EXIST
        public async Task<bool> DoesMembershipExistAsync(Membership membership)
        {

            HttpContent content = new StringContent(JsonSerializer.Serialize(membership), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"https://192.168.56.1:7062/memberships/exist", content);

            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }

        //EDIT MEMBERSHIP
        public async Task EditMembership(Membership membership)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(membership), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"https://192.168.56.1:7062/memberships", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Membership not found");
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
           
        }

        //ADD MEMBERSHIP
        public async Task AddMembership(Membership membership)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(membership), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://192.168.56.1:7062/memberships", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //BUY MEMBERSHIP
        public async Task BuyMembership(Membership membership)
        {
           // Debug.WriteLine("Buying membership");
            var user = await GetUserFromToken();



            var membershipInstance = new MembershipInstance
            {
                MembershipId = membership.Id,
                UserId = user.Id,
            };

            user?.UserMemberships?.Add(membershipInstance);

            HttpContent content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("https://192.168.56.1:7062/memberships/buy", content);


            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response.StatusCode);
                throw new Exception("Failed to buy membership");
            }
        }

        //GET ALL FREEZES
        public async Task<List<Freeze>> GetAllFreezes()
        {
            HttpResponseMessage response = await client.GetAsync("https://192.168.56.1:7062/freezes");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Freeze>>(content, options);

            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception("Something went wrong");
            }
        }


    }
}
