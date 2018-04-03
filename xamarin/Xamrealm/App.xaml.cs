using FreshMvvm;
using Realms.Sync;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.ViewModels;
using Constants = Xamrealm.Base.Constants;

namespace Xamrealm
{
	public partial class App : Application
	{
	    public static App Instance { get; private set; }

        public App ()
		{
			InitializeComponent();

		    Instance = this;

            Resources = new ResourceDictionary
		    {
		        ["BoardColors"] = Constants.Colors.BoardColors,
		        ["CardColors"] = Constants.Colors.CardColors,
		        ["CompletedColor"] = Constants.Colors.CompletedColor
		    };

            SetupIoc();
                
		    SetStartPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

	    public void SetStartPage()
	    {
	        var taskListsPage = FreshPageModelResolver.ResolvePageModel<TaskListsViewModel>();
	        var taskListsContainer = new FreshNavigationContainer(taskListsPage, NavigationContainerNames.MainContainer);

	        var loginPage = FreshPageModelResolver.ResolvePageModel<LoginViewModel>();
	        var loginContainer = new FreshNavigationContainer(loginPage, NavigationContainerNames.LoginContainer);

            var realmUser = User.Current;
	        MainPage = realmUser == null ? loginContainer : taskListsContainer;
	    }

        private static void SetupIoc()
	    {
	        FreshTinyIOCBuiltIn.Current.Register<LoginViewModel>();
	        FreshTinyIOCBuiltIn.Current.Register<TasksViewModel>();
	        FreshTinyIOCBuiltIn.Current.Register<TaskListsViewModel>();
        }
	}
}
