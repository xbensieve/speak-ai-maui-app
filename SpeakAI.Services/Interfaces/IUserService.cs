using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseModel<object>> SignUpCustomer(UserModel userModel);
        Task<ResponseModel<ProfileModel>> GetProfile(string userId);
        Task<ResponseModel<OTPModel>> ConfirmEmail(string userId);
        Task<ResponseModel<object>> VerifyOTP(string userId, string otp);
        Task<ResponseModel<string>> CreateOrder(string userId);
        Task<ResponseModel<string>> RequestPayment(OrderModel order);
        Task<ResponseModel<object>> ConfirmUpgrade(string orderId);
        Task<ResponseModel<object>> ResponsePayment(TransactionModel transactionModel);
    }
}
