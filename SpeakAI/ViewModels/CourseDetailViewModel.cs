using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotLoading));
        }
    }
    public bool IsNotLoading => !IsLoading;

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
    public ICommand EnrollCommand { get; }
    public ICommand StudyCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public CourseDetailViewModel(ICourseService courseService)
    {
        _courseService = courseService;
        EnrollCommand = new Command<string>(async (courseId) => await EnrollCourse(courseId));
        StudyCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("study");
        });
    }
    public async Task EnrollCourse(string courseId)
    {
        if (IsLoading) return;

        IsLoading = true;

        try
        {
            var result = await _courseService.EnrollCourse(courseId);

            if (result?.IsSuccess == true)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    IsEnrolled = true;
                    IsNew = false;
                });
            }
            else
            {
                Console.WriteLine("Enrollment failed: Server response was unsuccessful.");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Enrollment failed. Please try again.", "OK");
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Enrollment failed: {ex.Message}");
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred. Please try again later.", "OK");
            });
        }
        finally
        {
            IsLoading = false;
        }
    }


    private async void CheckEnrollmentAsync()
    {
        if (Course == null || string.IsNullOrEmpty(Course.CourseId))
        {
            return;
        }

        try
        {
            var response = await Task.Run(async () => await _courseService.CheckEnrolledCourse(Course.CourseId));

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
