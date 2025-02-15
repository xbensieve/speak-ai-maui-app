using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpeakAI.Services.Models
{
    public class EnrolledCourseResult
    {
        [JsonPropertyName("enrolledCourseId")]
        public string EnrolledCourseId { get; set; }
    }
}
