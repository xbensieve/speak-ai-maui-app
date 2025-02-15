using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpeakAI.ViewModels;

[QueryProperty(nameof(Course), "course")]
public class CourseDetailViewModel : INotifyPropertyChanged
{
    private readonly ICourseService _courseService;
    private CourseModel _course;
    private bool _isEnrolled;
    private bool _isNew;
    public bool IsEnrolled
    {
        get => _isEnrolled;
        set { _isEnrolled = value; OnPropertyChanged(nameof(IsEnrolled)); }
    }
    public bool IsNew
    {
        get => _isNew;
        set { _isNew = value; OnPropertyChanged(nameof(IsNew)); }
    }
    
    public CourseModel Course
    {
        get => _course;
        set
        {
            if (_course != value)
            {
                _course = value;
                OnPropertyChanged();
                CheckEnrollmentAsync();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public CourseDetailViewModel(ICourseService courseService)
    {
        _courseService = courseService;
    }
    private async void CheckEnrollmentAsync()
    {
        if (Course == null || string.IsNullOrEmpty(Course.CourseId))
        {
            return;
        }

        try
        {
            var response = await _courseService.CheckEnrolledCourse(Course.CourseId);
            if (response.IsSuccess && response.Result.EnrolledCourseId != null)
            {
                IsEnrolled = true;
                IsNew = false;
            }
            else
            {
                IsEnrolled = false;
                IsNew = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            IsEnrolled = false;
            IsNew = true;
        }
    }   
}
