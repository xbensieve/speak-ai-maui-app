using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Service
{
    public class AIService : IAIService
    {
        private readonly HttpService _httpService;
        public AIService(HttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseTopicModel> StartTopicAsync(TopicModel topic)
        {
            return await _httpService.PostAsync<TopicModel, ResponseTopicModel>("api/ai/start", topic);
        }
    }
}
