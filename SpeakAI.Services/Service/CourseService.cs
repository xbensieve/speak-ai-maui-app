using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SpeakAI.Services.Service
{
    public class CourseService : ICourseService
    {
        private readonly HttpService _httpService;
        public CourseService(HttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseModel<EnrolledCourseResult>> CheckEnrolledCourse(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                throw new ArgumentException("Course ID cannot be empty.");
            }

            try
            {
                string userId = await SecureStorage.GetAsync("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Cannot retrieve user ID.");
                }

                string url = $"api/courses/users/{userId}/courses/{courseId}/enrollment-status";

                var response = await _httpService.GetAsync<ResponseModel<EnrolledCourseResult>>(url);

                // Check if response exists and is successful
                return response ?? new ResponseModel<EnrolledCourseResult> { IsSuccess = false };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                Console.WriteLine($"Enrollment check failed: {ex.Message}");
                return new ResponseModel<EnrolledCourseResult> { IsSuccess = false };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new ResponseModel<EnrolledCourseResult> { IsSuccess = false };
            }
        }

        public async Task<ResponseModel<object>> EnrollCourse(string courseId)
        {
            if (string.IsNullOrEmpty(courseId))
            {
                throw new ArgumentException("Course ID cannot be empty.");
            }
            try
            {
                string userId = await SecureStorage.GetAsync("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Cannot retrieve user ID.");
                }
                string url = $"api/courses/{courseId}/enrollments";
                var response = await _httpService.PostAsync<string, ResponseModel<object>>(url, userId);
                return response ?? new ResponseModel<object> { IsSuccess = false };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                Console.WriteLine($"Enrollment check failed: {ex.Message}");
                return new ResponseModel<object> { IsSuccess = false };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new ResponseModel<object> { IsSuccess = false };
            }
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
                var response = await _httpService.GetAsync<CourseResponseModel>("api/courses");
                Console.Error.WriteLine(response.Result);
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

        public async Task<ResponseModel<CourseDetailModel>> GetCourseDetails(string courseId)
        {
            try
            {
                string url = $"api/courses/{courseId}/details";
                var response = await _httpService.GetAsync<ResponseModel<CourseDetailModel>>(url);
                foreach (var topic in response.Result.Topics)
                {
                    foreach (var exercise in topic.Exercises)
                    {
                        try
                        {
                            exercise.ContentExercises = JsonSerializer.Deserialize<ExerciseContent>(exercise.ContentRaw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        catch (JsonException)
                        {
                            exercise.ContentExercises = null;
                        }
                    }
                }
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return response;
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

            return new ResponseModel<CourseDetailModel> { IsSuccess = false };
        }

        public async Task<ResponseModel<EnrolledCourseProgressModel>> GetCourseProgress(string enrolledCourseId)
        {
            if (_httpService == null)
            {
                Console.Error.WriteLine("Error: _httpService is not initialized.");
                return new ResponseModel<EnrolledCourseProgressModel> { IsSuccess = false };
            }

            try
            {
                string url = $"api/courses/enrollments/{enrolledCourseId}";
                var response = await _httpService.GetAsync<ResponseModel<EnrolledCourseProgressModel>>(url);
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return response;
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

            return new ResponseModel<EnrolledCourseProgressModel> { IsSuccess = false };
        }

        public async Task<ResponseModel<List<EnrolledCourseModel>>> GetEnrolledCourses()
        {
            if (_httpService == null)
            {
                Console.Error.WriteLine("Error: _httpService is not initialized.");
                return new ResponseModel<List<EnrolledCourseModel>> { IsSuccess = false };
            }
            try
            {
                string userId = await SecureStorage.GetAsync("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Cannot retrieve user ID.");
                }
                string url = $"api/courses/user/{userId}/enrolled-courses";
                var response = await _httpService.GetAsync<ResponseModel<List<EnrolledCourseModel>>>(url);
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return response;
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

            return new ResponseModel<List<EnrolledCourseModel>> { IsSuccess = false };
        }

        public async Task<ResponseModel<object>> SubmitExerciseResult(string exerciseId, decimal earnedPoints)
        {
            if (string.IsNullOrEmpty(exerciseId))
            {
                throw new ArgumentException("Exercise Id cannot be empty.");
            }
            try
            {
                string userId = await SecureStorage.GetAsync("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Cannot retrieve user ID.");
                }
                var exerciseResult = new ExerciseResultModel
                {
                    userId = userId,
                    earnedPoints = earnedPoints
                };
                string url = $"api/courses/exercises/{exerciseId}/submissions";
                var response = await _httpService.PostAsync<ExerciseResultModel, ResponseModel<object>>(url, exerciseResult);
                return response ?? new ResponseModel<object> { IsSuccess = false };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                Console.WriteLine($"Enrollment check failed: {ex.Message}");
                return new ResponseModel<object> { IsSuccess = false };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new ResponseModel<object> { IsSuccess = false };
            }
        }
    }
}
