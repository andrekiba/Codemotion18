using System;
using System.Collections.Generic;
using Realms;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.Models;
using TTask = System.Threading.Tasks.Task;

namespace Xamrealm.Behaviors
{
    public class TaskListColoringBehavior : Behavior<View>
    {
        private IDisposable notificationToken;
        private WeakReference<View> view;

        public static BindableProperty RealmCollectionProperty =
            BindableProperty.Create(nameof(RealmCollection), typeof(IList<TaskList>), typeof(TaskListColoringBehavior), propertyChanged: OnRealmCollectionChanged);

        public IList<TaskList> RealmCollection
        {
            get => (IList<TaskList>)GetValue(RealmCollectionProperty);
            set
            {
                SetValue(RealmCollectionProperty, value);
                CalculateColor();
            }
        }

        private static void OnRealmCollectionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (TaskListColoringBehavior)bindable;
            self.notificationToken?.Dispose();

            var newCollection = newValue as IRealmCollection<TaskList>;
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
            // HACK: yield control to avoid a race condition
            await TTask.Delay(1);
            View sameView = null;
            TaskList item;
            if (RealmCollection == null || view?.TryGetTarget(out sameView) != true || (item = sameView.BindingContext as TaskList) == null)
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
               
                    backgroundColor = colors[(count - index - 1) % colors.Count];
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
