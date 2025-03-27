using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
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

    public async Task<ResponseModel<OTPModel>> ConfirmEmail(string userId)
    {
        return await _httpService.PostAsync<string, ResponseModel<OTPModel>>($"api/emails/verify?userID={userId}", userId);
    }

    public async Task<ResponseModel<object>> ConfirmUpgrade(string orderId)
    {
        return await _httpService.PostAsync<string, ResponseModel<object>>($"api/premium/confirm-upgrade/{orderId}", orderId);
    }

    public async Task<ResponseModel<string>> CreateOrder(string userId)
    {
        return await _httpService.PostAsync<string, ResponseModel<string>>($"api/premium/upgrade/{userId}", userId);
    }
    public async Task<ResponseModel<ProfileModel>> GetProfile(string userId)
    {
        return await _httpService.GetAsync<ResponseModel<ProfileModel>>($"api/users/{userId}");
    }

    public async Task<ResponseModel<string>> RequestPayment(OrderModel order)
    {
        return await _httpService.PostAsync<OrderModel, ResponseModel<string>>("api/payments/requests", order);
    }

    public async Task<ResponseModel<object>> ResponsePayment(TransactionModel transactionModel)
    {
        return await _httpService.PostAsync<TransactionModel, ResponseModel<object>>("api/payments/handle-response", transactionModel);
    }

    public async Task<ResponseModel<object>> SignUpCustomer(UserModel userModel)
    {
        return await _httpService.PostAsync<UserModel, ResponseModel<object>>("api/auth/register/customer", userModel);
    }

    public async Task<ResponseModel<object>> VerifyOTP(string userId, string otp)
    {
        return await _httpService.PostAsync<string, ResponseModel<object>>($"api/auth/verify/otp?userId={userId}&otpCode={otp}", userId);
    }
}
