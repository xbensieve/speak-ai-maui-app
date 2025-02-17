using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpeakAI.Services.Models
{
    public class CourseModel
    {
        [JsonPropertyName("id")]
        public string CourseId { get; set; } = string.Empty;
        [JsonPropertyName("courseName")]
        public string CourseName { get; set; } = "Coming soon!";
        [JsonPropertyName("description")]
        public string Description { get; set; } = "Coming soon!";
        [JsonPropertyName("maxPoint")]
        public decimal MaxPoint { get; set; }
        [JsonPropertyName("isFree")]
        public bool IsFree { get; set; }
        [JsonPropertyName("isPremium")]
        public bool IsPremium { get; set; }
        [JsonPropertyName("levelId")]
        public int LevelId { get; set; }
    }
}
