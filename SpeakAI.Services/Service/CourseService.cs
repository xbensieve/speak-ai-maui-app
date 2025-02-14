using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Service
{
    public class CourseService : ICourseService
    {
        private readonly HttpService _httpService;
        public CourseService(HttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<List<CourseModel>> GetAllCourses()
        {
            if (_httpService == null)
            {
                Console.Error.WriteLine("Error: _httpService is not initialized.");
                return new List<CourseModel>();
            }

            try
            {
                var response = await _httpService.GetAsync<CourseResponseModel>("api/Course/get-all");

                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return response.Result;
                }
                else
                {
                    Console.Error.WriteLine($"Error: {response?.Message ?? "Unknown error"}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.Error.WriteLine($"HTTP Error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected Error: {ex.Message}");
            }

            return new List<CourseModel>();
        }

    }
}
