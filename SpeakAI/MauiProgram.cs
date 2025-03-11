using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SpeakAI.Services;
using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Service;
using SpeakAI.ViewModels;
using SpeakAI.Views;
using System.Net;
using SpeakAI.Converters;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.Crashlytics;
using Plugin.Firebase.Bundled.Platforms.Android;
using CommunityToolkit.Maui.Core;
using UraniumUI;
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
                return new HttpClient { BaseAddress = new Uri("http://10.87.46.36:5232/") };
            });
            /* API Services */
            builder.Services.AddSingleton<HttpService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<ILoginService, LoginService>();
            builder.Services.AddSingleton<ICourseService, CourseService>();

            /* View Model Logic */
            builder.Services.AddSingleton<StudyViewModel>();
            builder.Services.AddSingleton<SignInViewModel>();
            builder.Services.AddSingleton<SignUpViewModel>();
            builder.Services.AddSingleton<CourseDetailViewModel>();
            builder.Services.AddSingleton<CourseViewModel>();
            builder.Services.AddSingleton<ExerciseViewModel>();
            builder.Services.AddSingleton<ExerciseDetailViewModel>();
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
            return builder.Build();
        }
        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if IOS
            events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => {
                CrossFirebase.Initialize(CreateCrossFirebaseSettings());
                return false;
            }));
#else
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                    CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings())));
                CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
#endif
            });

            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
            return builder;
        }

        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            return new CrossFirebaseSettings(isAuthEnabled: true,
            isCloudMessagingEnabled: true, isAnalyticsEnabled: true);
        }
    }
}
