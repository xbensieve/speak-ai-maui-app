using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Models
{
    public class UserModel
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string confirmedPassword { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public string birthday { get; set; }
        public string gender { get; set; }
    }
}
