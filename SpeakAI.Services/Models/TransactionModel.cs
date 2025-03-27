using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Models
{
    public class TransactionModel
    {
        public string userId { get; set; }
        public string transactionInfo { get; set; }
        public string transactionNumber { get; set; }
        public bool isSuccess { get; set; }
    }
}
