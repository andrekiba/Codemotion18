using FreshMvvm;
using Xamrealm.ViewModels;

namespace Xamrealm.Base
{
    public static class DesignViewModelLocator
    {
        private static LoginViewModel loginViewModel;
        public static LoginViewModel LoginViewModel => loginViewModel ?? (loginViewModel = FreshIOC.Container.Resolve<LoginViewModel>());

        private static TaskListsViewModel taskListsViewModel;
        public static TaskListsViewModel TaskListsViewModel => taskListsViewModel ?? (taskListsViewModel = FreshIOC.Container.Resolve<TaskListsViewModel>());

        private static TasksViewModel tasksViewModel;
        public static TasksViewModel TasksViewModel => tasksViewModel ?? (tasksViewModel = FreshIOC.Container.Resolve<TasksViewModel>());
    }
}
