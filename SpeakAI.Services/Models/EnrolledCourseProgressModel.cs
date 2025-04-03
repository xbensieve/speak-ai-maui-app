using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpeakAI.Services.Models
{
    public class EnrolledCourseProgressModel
    {
        [JsonPropertyName("course")]
        public CourseModel Course { get; set; }
        [JsonPropertyName("progress")]
        public decimal ProgressPoints { get; set; }
        [JsonPropertyName("topics")]
        public List<TopicProgress> TopicProgresses { get; set; }
    }
    public class TopicProgress
    {
        [JsonPropertyName("id")]
        public string TopicId { get; set; }
        [JsonPropertyName("progress")]
        public decimal ProgressPoints { get; set; }
    }
}
