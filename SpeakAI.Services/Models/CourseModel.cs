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
        public string CourseId { get; set; }
        [JsonPropertyName("courseName")]
        public string CourseName { get; set; } = "Comming soon!";
        [JsonPropertyName("description")]
        public string Description { get; set; } = "Comming soon!";
        [JsonPropertyName("maxPoint")]
        public decimal MaxPoint { get; set; }
    }
}
