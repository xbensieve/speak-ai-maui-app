using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Models
{
    public class ResponseTopicModel
    {
        [JsonProperty("currentTopic")]
        public int CurrentTopic { get; set; }
        [JsonProperty("botResponse")]
        public string BotResponse { get; set; }
        [JsonProperty("scenarioPrompt")]
        public string ScenarioPrompt { get; set; }
    }
}
