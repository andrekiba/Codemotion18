using System;
using System.Collections.Generic;
using Realms;
using Xamarin.Forms;
using Xamrealm.Base;
using TTask = System.Threading.Tasks.Task;
using Task = Xamrealm.Models.Task;

namespace Xamrealm.Behaviors
{
    public class TaskColoringBehavior : Behavior<View>
    { 
        private IDisposable notificationToken;
        private WeakReference<View> view;

        public static BindableProperty RealmCollectionProperty =
            BindableProperty.Create(nameof(RealmCollection), typeof(IList<Task>), typeof(TaskColoringBehavior), propertyChanged: OnRealmCollectionChanged);

        public IList<Task> RealmCollection
        {
            get => (IList<Task>)GetValue(RealmCollectionProperty);
            set
            {
                SetValue(RealmCollectionProperty, value);
                CalculateColor();
            }
        }

        public static BindableProperty TaskListIndexProperty = BindableProperty.Create(nameof(TaskListIndex), typeof(int), typeof(TaskColoringBehavior), default(int));

        public int TaskListIndex
        {
            get => (int)GetValue(TaskListIndexProperty);
            set => SetValue(TaskListIndexProperty, value);
        }

        public static BindableProperty TaskListCountProperty = BindableProperty.Create(nameof(TaskListCount), typeof(int), typeof(TaskColoringBehavior), default(int));

        public int TaskListCount
        {
            get => (int)GetValue(TaskListCountProperty);
            set => SetValue(TaskListCountProperty, value);
        }

        private static void OnRealmCollectionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (TaskColoringBehavior)bindable;
            self.notificationToken?.Dispose();

            var newCollection = newValue as IRealmCollection<Task>;
            self.notificationToken = newCollection?.SubscribeForNotifications((sender, changes, error) => { self.CalculateColor(); });
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            view = new WeakReference<View>(bindable);
            CalculateColor();
        }

        protected override void OnDetachingFrom(View bindable)
        {
            base.OnDetachingFrom(bindable);

            view = null;
            notificationToken?.Dispose();
        }

        private async void CalculateColor()
        {
            //yield control to avoid a race condition
            await TTask.Delay(1);
            View sameView = null;
            Task item;
            if (RealmCollection == null || view?.TryGetTarget(out sameView) != true || (item = sameView.BindingContext as Task) == null)
                return;
            try
            {
                Color backgroundColor;
                if (item.IsCompleted)
                    backgroundColor = Constants.Colors.CompletedColor;
                else
                {
                    var index = RealmCollection.IndexOf(item);
                    var count = RealmCollection.Count;
                    var colors = Constants.Colors.TaskListColors;
                      
                    var shades = Constants.Colors.TaskColors[(TaskListCount - TaskListIndex - 1) % colors.Count];
                    backgroundColor = shades[(count - index - 1) % shades.Count];                
                }

                sameView.BackgroundColor = backgroundColor;
            }
            catch
            {
                // Let's not crash because of a coloring fail :)
                Console.WriteLine("Problem on calculating color!");
            }
        }
    }
}
