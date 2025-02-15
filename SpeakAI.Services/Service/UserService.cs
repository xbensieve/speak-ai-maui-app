using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.Services.Service;

public class UserService : IUserService
{
    private readonly HttpService _httpService;
    public UserService(HttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<ResponseModel<object>> SignUpCustomer(UserModel userModel)
    {
        return await _httpService.PostAsync<UserModel, ResponseModel<object>>("api/auth/sign-up-customer", userModel);
    }
}
