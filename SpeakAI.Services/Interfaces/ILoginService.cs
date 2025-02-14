using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponseModel> Login(LoginRequestModel loginRequestModel);
    }
}
