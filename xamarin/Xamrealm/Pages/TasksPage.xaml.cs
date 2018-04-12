using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamrealm.Models;

namespace Xamrealm.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : ContentPage
    {
        public TasksPage()
        {
            InitializeComponent();

            //TasksListControl.ItemSelected += (sender, e) => {
            //    ((ListView)sender).SelectedItem = null;
            //};
        }

        private void TasksListControlOnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is Task task))
                return;
            task.Display = !task.Display;
        }
    }
}