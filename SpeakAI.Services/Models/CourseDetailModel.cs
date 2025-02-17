using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpeakAI.Services.Models
{
    public class CourseDetailModel
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
        [JsonPropertyName("topics")]
        public List<Topic> Topics { get; set; } = new();
    }
    public class Topic
    {
        [JsonPropertyName("id")]
        public string TopicId { get; set; } = string.Empty;
        [JsonPropertyName("topicName")]
        public string TopicName { get; set; } = string.Empty;
        [JsonPropertyName("maxPoint")]
        public decimal MaxPoint { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("exercises")]
        public List<Exercise> Exercises { get; set; } = new();
        [JsonIgnore]
        public decimal ProgressPoints { get; set; }
    }
    public class Exercise
    {
        [JsonPropertyName("id")]
        public string ContentId { get; set; } = string.Empty;
        [JsonPropertyName("content")]
        public string ContentRaw { get; set; }
        [JsonPropertyName("maxPoint")]
        public decimal MaxPoint { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonIgnore]
        public ExerciseContent ContentExercises { get; set; }
    }
    public class ExerciseContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("question")]
        public string Question { get; set; }

        [JsonPropertyName("options")]
        public List<string> Options { get; set; }

        [JsonPropertyName("answer")]
        public string Answer { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; }
    }
}
