﻿using System;
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
using Microsoft.Maui.Controls;
using System.Xml;


namespace Gym.Services
{
    public class WebService
    {
        HttpClient client;
        private readonly TokenService tokenService;
        // string socket = "http://192.168.56.1:5119";
        // string socket = "http://localhost:5119";
        string socket = "https://stirred-lightly-cattle.ngrok-free.app";

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
             HttpResponseMessage response = await client.PostAsync($"{socket}/login", content);

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
                throw new Exception(response.StatusCode.ToString());
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
                //Password = tokenS.Claims.First(claim => claim.Type == "Password").Value,
                Name = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Name).Value,
                PhoneNumber = tokenS.Claims.First(claim => claim.Type == ClaimTypes.MobilePhone).Value,
                IsBanned = bool.Parse(tokenS.Claims.First(claim => claim.Type == "IsBanned").Value),
                Orders = new List<Order>()
               
            };
            //Debug.WriteLine("User from token: " + user.Email);

            return user;
        }

        //REMOVE USER 

        public async Task RemoveUser(int id)
        {
            // HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7062/users/{id}");
            HttpResponseMessage response = await client.DeleteAsync($"{socket}/users/{id}");
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
            HttpResponseMessage response = await client.PostAsync($"{socket}/users", content);
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
            HttpResponseMessage response = await client.PutAsync($"{socket}/users/ban/{id}", null);
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
            HttpResponseMessage response = await client.GetAsync($"{socket}/users");
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
            HttpResponseMessage response = await client.GetAsync($"{socket}/users/banned");
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
            HttpResponseMessage response = await client.GetAsync($"{socket}/users/unbanned");
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
            HttpResponseMessage response = await client.GetAsync($"{socket}/users/exist/{email}");
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
                throw new Exception((response.StatusCode.ToString()));
            }
        } 
        

        //GET ALL MEMBERSHIPS
        public async Task<List<Membership>> GetAllMemberships()
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/memberships");
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

        // GET ALL TRENERS
        public async Task<List<Trener>> GetAllTreners()
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/treners");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Trener>>(content, options);

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

        //DELETE TRENER
        public async Task DeleteTrener(Trener trener)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{socket}/treners/{trener.Id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

        //ADD TRENER
        public async Task AddTrener(Trener trener)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(trener), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{socket}/treners", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }


        //GET ACTIVE MEMBERSHIPS BY USER ID
        public async Task<List<MembershipInstance>> GetActiveMembershipsByUserId(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/memberships/active/{id}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<MembershipInstance>>(content, options);

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

        //GET INACTIVE MEMBERSHIPS BY USER ID
        public async Task<List<MembershipInstance>> GetNotActiveMembershipsByUserId(int id)
        {
            {
                HttpResponseMessage response = await client.GetAsync($"{socket}/memberships/notactive/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<MembershipInstance>>(content, options);

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

        // GET FROZEN MEMBERSHIPS BY USER ID
        public async Task<List<MembershipInstance>> GetFrozenMembershipsByUserId(int id)
        {
            {
                HttpResponseMessage response = await client.GetAsync($"{socket}/memberships/frozen/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<MembershipInstance>>(content, options);

                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new SessionExpiredException();
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString());
                }
            }
        }

        // DOES ACTIVE MEMBERSHIP EXIST BY USER ID
        public async Task<bool> DoesActiveMembershipExist(int userId)
        {
           
                
                HttpResponseMessage response = await client.GetAsync($"{socket}/user/memberships/exist/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return bool.Parse(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new Exception($"Failed to check if active membership exists. Status code: {response.StatusCode}");
                }
            
        }

        // ACTIVATE MEMBERSHIP
        public async Task<MembershipInstance> ActivateMembershipInstance(int id)
        {
            HttpResponseMessage response = await client.PutAsync($"{socket}/membershipinstances/activate/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<MembershipInstance>(content, options);
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
            HttpResponseMessage response = await client.DeleteAsync($"{socket}/memberships/{membership.Id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong");
            }
        }

        // DELETE FREEZE
        public async Task DeleteFreeze(Freeze freeze)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{socket}/freezes/{freeze.Id}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }



        //GET MEMBERSHIP BY ID
        public async Task<Membership> GetMembershipById(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/memberships/{id}");
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


        //CHECK IF USER IS TRENER BY EMAIL
       public async Task<bool> IsTrenerAsync(string email)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/treners/exist/{email}");
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
                throw new Exception((response.StatusCode.ToString()));
            }
        }

      

        //GET FREEZE BY ID
        public async Task<Freeze> GetFreezeById(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/freezes/{id}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Freeze>(content, options);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //GET MEMBERSHIP INSTANCE BY ID

        public async Task<MembershipInstance> GetMembershipInstanceById(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/membershipinstances/{id}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<MembershipInstance>(content, options);
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
            HttpResponseMessage response = await client.PostAsync($"{socket}/memberships/exist", content);

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

        //DOES FREEZE EXIST
        public async Task<bool> DoesFreezeExistAsync(Freeze freeze)
        {

            HttpContent content = new StringContent(JsonSerializer.Serialize(freeze), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{socket}/freezes/exist", content);

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
            HttpResponseMessage response = await client.PutAsync($"{socket}/memberships", content);
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

        // EDIT FREEZE
        public async Task EditFreeze(Freeze freeze)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(freeze), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"{socket}/freezes", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Freeze not found");
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

        }

        //ADD MEMBERSHIP
        public async Task AddMembership(Membership membership)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(membership), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{socket}/memberships", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        // ADD FREEZE
        public async Task AddFreeze(Freeze freeze)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(freeze), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{socket}/freezes", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        // FREEEZE MEMBERSHIP
        public async Task FreezeMembership(MembershipInstance membership)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(membership), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{socket}/memberships/freeze", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        // CANCEL FREEZE
        public async Task CancelFreeze(int membershipId)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{socket}/memberships/cancel/freeze/{membershipId}");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }



        //WORK WITH PAYMENTS
        public async Task<string> MakePayment(int userId, Membership membership)
        {
            var order = new Order
            {
                UserId = userId,
                MembershipId = membership.Id,
                Amount = new Amount
                {
                    Currency = "RUB",
                    Value = membership.Price.ToString()
                },
                Confirmation = new Redirection
                {
                    Type = "redirect",
                    Return_url = "https://www.google.com"
                },
                Capture = true,

            };
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            HttpContent content = new StringContent(JsonSerializer.Serialize(order, options), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{socket}/users/payment?userId={userId}&membershipId={membership.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                var content_with_payment = await response.Content.ReadAsStringAsync();
                Payment payment = JsonSerializer.Deserialize<Payment>(content_with_payment, options);
                Debug.WriteLine(payment.Confirmation.Confirmation_url);
                return payment.Confirmation.Confirmation_url;


            }
            else
            {
                Debug.WriteLine(response.StatusCode.ToString());
                throw new Exception(response.StatusCode.ToString());
            }

        }



        // CHECK STATUS OF PAYMENT
        //public async Task CheckPaymentStatus(int userId, int membershipId)
        //{
        //    HttpResponseMessage response = await client.GetAsync($"{socket}/payment/notification?userId={userId}");
        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    Debug.WriteLine(await response.Content.ReadAsStringAsync());
        //    User user = JsonSerializer.Deserialize<User>((await response.Content.ReadAsStringAsync()), options);

        //    if (user.Payment.Paid)
        //    {
               
        //        var membershipInstance = new MembershipInstance
        //        {
        //            MembershipId = membershipId,
        //            UserId = user.Id,
        //        };

        //        user?.UserMemberships?.Add(membershipInstance);

        //        HttpContent content2 = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
        //        HttpResponseMessage response2 = await client.PutAsync($"{socket}/memberships/buy", content2);
                
        //    }
        //    else
        //    {
        //        throw new Exception("Payment not completed");
        //    }
        //}

     


        //GET ALL FREEZES
        public async Task<List<Freeze>> GetAllFreezes()
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/freezes");
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

        //RESET PASSWORD
        public async Task ResetPassword(string email)
        {
            if(!(await IsUserExistAsync(email)))
            {
                throw new Exception("User with this email does not exist");
            }
            HttpContent content = new StringContent(JsonSerializer.Serialize(email), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{socket}/users/resetpassword", content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }


        //CHECK USER STATUS
        public async Task<bool> CheckUserOrMember(string email)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/users/status/{email}");
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
                throw new Exception((response.StatusCode.ToString()));
            }
        }


        //GET TRENER BY ID
        public async Task<Trener> GetTrenerById(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/treners/{id}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Trener>(content, options);
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

      

        //ADD WORKHOUR TO TRENER'S WORKDAY BY DATE AND ID
        public async Task AddWorkHour(int trenerId, DateTime date, TimeSpan time)
        {
            date = date.Date.Add(time);
            string dateString = date.ToString("yyyy.MM.dd HH:mm"); ;
            // HttpContent content = new StringContent(JsonSerializer.Serialize(dateString), Encoding.UTF8, "application/json");
            // Debug.WriteLine(JsonSerializer.Serialize(date));
            Debug.WriteLine(dateString);
            HttpResponseMessage response = await client.PostAsync($"{socket}/treners/{trenerId}/workday?dateString={dateString}", null);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {

                throw new Exception("This hour has already been set");
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }


        //GET TRENER WOKHOURS BY ID AND DATE
        public async Task<List<WorkHour>> GetTrenerWorkHours(int trenerId, DateTime date)
        {
            string dateString = date.ToString("yyyy.MM.dd HH:mm"); ;
            HttpResponseMessage response = await client.GetAsync($"{socket}/treners/workhours/{trenerId}?dateString={dateString}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var workouts = JsonSerializer.Deserialize<List<WorkHour>>(content, options);
                foreach (var workout in workouts)
                {
                    Debug.WriteLine(workout.Start);
                }
                return JsonSerializer.Deserialize<List<WorkHour>>(content, options);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
               // Debug.WriteLine(response.StatusCode.ToString());
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //GET AVAILABLE WORKHOURS BY DATE BY TRENER ID
        public async Task<List<WorkHour>> GetAvailableWorkHoursByDateByTrenerId(int trenerId, DateTime date)
        {
            string dateString = date.ToString("yyyy.MM.dd HH:mm"); ;
            HttpResponseMessage response = await client.GetAsync($"{socket}/treners/workhours/available/{trenerId}?dateString={dateString}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<WorkHour>>(content, options);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //REMOVE WORKHOUR FROM WORKDAY
        public async Task RemoveWorkHour(int trenerId, DateTime date)
        {
            
            string dateString = date.ToString("yyyy.MM.dd HH:mm"); ;
            // HttpContent content = new StringContent(JsonSerializer.Serialize(dateString), Encoding.UTF8, "application/json");
            // Debug.WriteLine(JsonSerializer.Serialize(date));
            Debug.WriteLine(dateString);
            HttpResponseMessage response = await client.DeleteAsync($"{socket}/treners/{trenerId}/workday?dateString={dateString}");
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw new Exception("You have clients here");
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //APPLY FOR WORKOUT
        public async Task ApplyWorkout(int trenerId, int memberId, DateTime date)
        {
            
            string dateString = date.ToString("yyyy.MM.dd HH:mm"); ;

            HttpResponseMessage response = await client.PostAsync($"{socket}/treners/{trenerId}/workday/{memberId}?dateString={dateString}", null);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {

                throw new Exception("You already have a workout for today");
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //APPLY FOR WORKOUT BY WEEKDAY
        public async Task<string> ApplyWorkoutByWeekday(int trenerId, int memberId, DateTime date)
        {

            string dateString = date.ToString("yyyy.MM.dd HH:mm"); ;

            HttpResponseMessage response = await client.PostAsync($"{socket}/treners/{trenerId}/workday/{memberId}/weekday?dateStringFrom={dateString}", null);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                
                return result;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }


        //GET USER WORKOUTS

        public async Task<List<WorkHour>> GetUserWorkouts(int id, DateTime date)
        {
            string dateString = date.ToString("yyyy.MM.dd HH:mm"); 
            HttpResponseMessage response = await client.GetAsync($"{socket}/member/{id}/workouts?dateString={dateString}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<WorkHour> workouts = JsonSerializer.Deserialize<List<WorkHour>>(content, options);
                foreach(var workout in workouts)
                {
                    Debug.WriteLine(workout.Start);
                }
                return workouts;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //COPY WORKDAY HOURS

        public async Task CopyWorkDay(int trenerId, DateTime dateFrom, DateTime dateTo)
        {

            string dateStringFrom = dateFrom.ToString("s");
            string dateStringTo = dateTo.ToString("s");

            HttpResponseMessage response = await client.PostAsync($"{socket}/trener/{trenerId}/workdays/copy?dateStringFrom={dateStringFrom}&dateStringTo={dateStringTo}", null);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {

                throw new Exception("Date is not empty");
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }


        //COPY TO A PARTICULAR DAY
        public async Task CopyByWeekDays(int trenerId, DateTime dateFrom)
        {

            string dateStringFrom = dateFrom.ToString("s");
           

            HttpResponseMessage response = await client.PostAsync($"{socket}/trener/{trenerId}/workdays/copy/weekday?dateStringFrom={dateStringFrom}", null);
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }


        //GET WORKHOUR CLIENTS BY  WORKHOUR ID

        public async Task<List<User>> GetWorkHourClients(int workHourId)
        {
            HttpResponseMessage response = await client.GetAsync($"{socket}/workhour/{workHourId}/clients");
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
                throw new Exception(response.StatusCode.ToString());
            }
        }

        //REMOVE WORKHOUR CLIENT BY WORKHOUR ID AND USER ID
        public async Task RemoveWorkHourClient(int workHourId, int userId)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{socket}/workhour/{workHourId}/client/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception("Cannot cancel a workout that has already started");
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

    }
}
