using FreshMvvm;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.Converters;
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
		        ["ListColors"] = Constants.Colors.ListColors,
		        ["TaskColors"] = Constants.Colors.TaskColors,
		        ["CompletedColor"] = Constants.Colors.CompletedColor,
		        ["InverseBoolConverter"] = new InverseBoolConverter(),
		        ["TaskListToAlphaConverter"] = new TaskListToAlphaConverter()
		    };

            SetupIoc();
                
		    SetMainPage();
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

	    public void SetMainPage()
	    {
	        var taskListsPage = FreshPageModelResolver.ResolvePageModel<TaskListsViewModel>();
	        var taskListsContainer = new FreshNavigationContainer(taskListsPage, NavigationContainerNames.MainContainer);

	        MainPage = taskListsContainer;
	    }

        private static void SetupIoc()
	    {
	        FreshTinyIOCBuiltIn.Current.Register<LoginViewModel>();
	        FreshTinyIOCBuiltIn.Current.Register<TasksViewModel>();
	        FreshTinyIOCBuiltIn.Current.Register<TaskListsViewModel>();
        }
	}
}
