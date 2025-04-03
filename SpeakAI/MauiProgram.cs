using CommunityToolkit.Maui;
using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Service;
using SpeakAI.ViewModels;
using SpeakAI.Views;
using Plugin.Firebase.Bundled.Shared;
using CommunityToolkit.Maui.Core;
using UraniumUI;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui.Media;
namespace SpeakAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCore()
                .UseUraniumUIMaterial()
                .UseUraniumUI()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Inter-Regular.ttf", "Inter");
                    fonts.AddFont("Inter-Bold.ttf", "InterBold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                return new HttpClient { BaseAddress = new Uri("http://sai.runasp.net/") };
            });
            /* API Services */
            builder.Services.AddSingleton<HttpService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<ILoginService, LoginService>();
            builder.Services.AddSingleton<ICourseService, CourseService>();
            builder.Services.AddSingleton<IAIService, AIService>();
            builder.Services.AddSingleton<ISpeechToText>(_ => SpeechToText.Default);
            builder.Services.AddSingleton<NavigationDataService>();
            /* View Model Logic */
            builder.Services.AddSingleton<StudyViewModel>();
            builder.Services.AddSingleton<SignInViewModel>();
            builder.Services.AddSingleton<SignUpViewModel>();
            builder.Services.AddSingleton<CourseDetailViewModel>();
            builder.Services.AddSingleton<CourseViewModel>();
            builder.Services.AddSingleton<ExerciseViewModel>();
            builder.Services.AddSingleton<ExerciseDetailViewModel>();
            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddSingleton<ConfirmEmailViewModel>();
            builder.Services.AddSingleton<PaymentViewModel>();
            builder.Services.AddSingleton<StartAIViewModel>();
            /* View Page */
            builder.Services.AddTransient<StudyPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddTransient<CoursePage>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<CourseDetailPage>();
            builder.Services.AddTransient<ExercisePage>();
            builder.Services.AddTransient<ExerciseDetailPage>();
            builder.Services.AddTransient<AITutorPage>();
            builder.Services.AddTransient<ConfirmEmailPage>();
            builder.Services.AddTransient<PaymentPage>();
            builder.Services.AddTransient<PaymentWebViewPage>();
            builder.Services.AddTransient<StartAIPage>();
            return builder.Build();
        }
        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            return new CrossFirebaseSettings(isAuthEnabled: true,
            isCloudMessagingEnabled: true, isAnalyticsEnabled: true);
        }
    }
}
