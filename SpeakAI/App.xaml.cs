using SpeakAI.Services.Interfaces;
using SpeakAI.Views;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace SpeakAI
{
    public partial class App : Application
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;

        public App(IUserService userService, ILoginService loginService)
        {
            InitializeComponent();
            _userService = userService;
            _loginService = loginService;

            MainPage = new NavigationPage(new LoadingPage(_userService, _loginService));
        }
    }
}
