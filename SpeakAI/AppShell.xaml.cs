using SpeakAI.Views;

namespace SpeakAI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("coursedetail", typeof(CourseDetailPage));
            Routing.RegisterRoute("exercise", typeof(ExercisePage));
            Routing.RegisterRoute("exercisedetail", typeof(ExerciseDetailPage));
        }
    }
}
