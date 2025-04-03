using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.Services.Service;
using SpeakAI.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Maui.Controls;

namespace SpeakAI.Views;

public partial class CoursePage : ContentPage
{
    private readonly IUserService _userService;
    private ContentView _loadingOverlay;

    public CoursePage(ICourseService courseService, IUserService userService)
    {
        InitializeComponent();
        _userService = userService;
        BindingContext = new CourseViewModel(courseService);
        InitializeLoadingOverlay();
    }

    // Initialize the loading overlay programmatically
    private void InitializeLoadingOverlay()
    {
        _loadingOverlay = new ContentView
        {
            IsVisible = false,
            Content = new ActivityIndicator
            {
                Color = Color.FromHex("#3498DB"),
                WidthRequest = 40,
                HeightRequest = 40,
                IsRunning = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

        // Add the overlay to the page’s layout
        if (Content is Layout layout)
        {
            layout.Children.Add(_loadingOverlay);
        }
        else
        {
            // If Content isn’t a layout, wrap it
            var grid = new Grid { Children = { Content, _loadingOverlay } };
            Content = grid;
        }
    }

    private async Task<string> GetUserIdFromTokenAsync()
    {
        try
        {
            string token = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "Unknown";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Token parsing error: {ex.Message}");
        }
        return "Unknown";
    }

    private async void OnCourseTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is CourseModel selectedCourse)
        {
            // Show loading overlay on the main thread
            await MainThread.InvokeOnMainThreadAsync(() => _loadingOverlay.IsVisible = true);

            try
            {
                var userId = await GetUserIdFromTokenAsync();
                var user = await _userService.GetProfile(userId);
                var navigationParameter = new Dictionary<string, object>
                {
                    { "course", selectedCourse }
                };

                if (selectedCourse.IsPremium && !(user.Result?.IsPremium ?? false))
                {
                    // Redirect to payment page for premium courses
                    await Shell.Current.GoToAsync("payment", navigationParameter);
                }
                else
                {
                    // Redirect to course details for free courses
                    await Shell.Current.GoToAsync("coursedetail", navigationParameter);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
                });
            }
            finally
            {
                // Hide loading overlay on the main thread
                await MainThread.InvokeOnMainThreadAsync(() => _loadingOverlay.IsVisible = false);
            }
        }
    }
}