using SpeakAI.Views;

namespace SpeakAI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("coursedetail", typeof(CourseDetailPage));
            Routing.RegisterRoute("aitutor", typeof(AITutorPage));
            Routing.RegisterRoute("exercise", typeof(ExercisePage));
            Routing.RegisterRoute("exercisedetail", typeof(ExerciseDetailPage));
            Routing.RegisterRoute("study", typeof(StudyPage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(CoursePage), typeof(CoursePage));
            Routing.RegisterRoute(nameof(ConfirmEmailPage), typeof(ConfirmEmailPage));
            Routing.RegisterRoute("payment", typeof(PaymentPage));
        }
    }
}
