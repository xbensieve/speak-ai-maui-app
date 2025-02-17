using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            _httpService = httpService;
        }
        public async Task<ResponseModel<LoginResultModel>> Login(LoginRequestModel loginRequestModel)
        {
            var result = await _httpService.PostAsync<LoginRequestModel, ResponseModel<LoginResultModel>>("api/auth/tokens", loginRequestModel);

            if (result != null && result.IsSuccess && result.Result != null)
            {
                await SecureStorage.SetAsync("AccessToken", result.Result.AccessToken);
                await SecureStorage.SetAsync("UserId", result.Result.User.UserId);
            }

            return result;
        }
    }
}
