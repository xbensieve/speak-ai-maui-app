using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SpeakAI.Services.Service
{
    public class LoginService : ILoginService
    {
        private readonly HttpService _httpService;

        public LoginService(HttpService httpService)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        }

        public async Task<ResponseModel<LoginResultModel>> Login(LoginRequestModel loginRequestModel)
        {
            var result = await _httpService.PostAsync<LoginRequestModel, ResponseModel<LoginResultModel>>("api/auth/login", loginRequestModel);

            if (result == null)
            {
                return new ResponseModel<LoginResultModel> { IsSuccess = false, Message = "Null response from server" };
            }

            if (result.IsSuccess && result.Result != null)
            {
                string accessToken = result.Result.AccessToken;

                // Store AccessToken securely
                await SecureStorage.SetAsync("AccessToken", accessToken);

                // Decode JWT and extract the "Id"
                string userId = DecodeJwtAndGetUserId(accessToken);
                if (!string.IsNullOrEmpty(userId))
                {
                    await SecureStorage.SetAsync("UserId", userId);
                }
            }

            return result;
        }

        private string DecodeJwtAndGetUserId(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                return userId ?? "Unknown";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding JWT: {ex.Message}");
                return "Unknown";
            }
        }
    }
}
