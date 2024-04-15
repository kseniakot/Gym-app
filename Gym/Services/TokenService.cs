using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Services
{
    public class TokenService
    {
       

      

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync("token", token);
        }

        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync("token");
        }

        public void RemoveToken()
        {
            SecureStorage.Remove("token");
        }
    }
}
