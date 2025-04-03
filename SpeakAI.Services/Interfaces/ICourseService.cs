using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseModel>> GetAllCourses();
        Task<ResponseModel<EnrolledCourseResult>> CheckEnrolledCourse(string courseId);
        Task<ResponseModel<object>> EnrollCourse(string courseId);
        Task<ResponseModel<List<EnrolledCourseModel>>> GetEnrolledCourses();
        Task<ResponseModel<CourseDetailModel>> GetCourseDetails(string courseId);
        Task<ResponseModel<EnrolledCourseProgressModel>> GetCourseProgress(string enrolledCourseId);
        Task<ResponseModel<object>> SubmitExerciseResult(string exerciseId, decimal earnedPoints);
    }
}
