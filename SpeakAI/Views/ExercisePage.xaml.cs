using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.Services.Service;
using SpeakAI.ViewModels;
using System.Diagnostics;

namespace SpeakAI.Views;

public partial class ExercisePage : ContentPage
{
    private readonly ICourseService _courseService;
    private readonly NavigationDataService _navigationDataService;
    public ExercisePage(ICourseService courseService, NavigationDataService navigationDataService)
    {
        Debug.WriteLine("[DOTNET] ExercisePage constructor called");
        InitializeComponent();
        _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        _navigationDataService = navigationDataService ?? throw new ArgumentNullException(nameof(navigationDataService));
        BindingContext = new ExerciseViewModel(_courseService);
        Debug.WriteLine("[DOTNET] ExercisePage initialized");
    }
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        Debug.WriteLine("[DOTNET] OnExerciseTapped started");
        try
        {
            if (Shell.Current == null)
            {
                Debug.WriteLine("[DOTNET] Shell.Current is null");
                await DisplayAlert("Error", "Shell is not initialized", "OK");
                return;
            }

            if (sender is Frame frame && frame.BindingContext is Topic selectedTopic)
            {
                Debug.WriteLine($"[DOTNET] Selected Topic: {selectedTopic?.TopicName ?? "null"}");
                _navigationDataService.SelectedTopic = selectedTopic;
                Debug.WriteLine("[DOTNET] Navigating to exercisedetail");
                await Shell.Current.GoToAsync("exercisedetail");
                Debug.WriteLine("[DOTNET] Navigation completed");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[DOTNET] Navigation error: {ex.Message}\n{ex.StackTrace}");
            await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
        }
    }
}
