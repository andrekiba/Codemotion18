using System;
using System.Collections.Generic;
using Realms;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.Models;
using Task = System.Threading.Tasks.Task;

namespace Xamrealm.Behaviors
{
    public class TaskListColoringBehavior<T> : Behavior<View> where T : RealmObject, ICompletable
    {
        private IDisposable notificationToken;
        private WeakReference<View> view;

        public static BindableProperty RealmCollectionProperty =
            BindableProperty.Create(nameof(RealmCollection), typeof(IList<T>), typeof(TaskListColoringBehavior<T>), propertyChanged: OnRealmCollectionChanged);

        public IList<T> RealmCollection
        {
            get => (IList<T>)GetValue(RealmCollectionProperty);
            set
            {
                SetValue(RealmCollectionProperty, value);
                CalculateColor();
            }
        }

        public static BindableProperty TaskListColorProperty =
            BindableProperty.Create(nameof(TaskListColor), typeof(string), typeof(TaskListColoringBehavior<T>));

        public string TaskListColor
        {
            get => (string) GetValue(TaskListColorProperty);
            set => SetValue(TaskListColorProperty, value);
        }

        private static void OnRealmCollectionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (TaskListColoringBehavior<T>)bindable;
            self.notificationToken?.Dispose();

            var newCollection = newValue as IRealmCollection<T>;
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
            // HACK: yield control to avoid a race condition where things might not be initialized yet, resulting in no color being applied
            await Task.Delay(1);
            View sameView = null;
            T item;
            if (RealmCollection == null || view?.TryGetTarget(out sameView) != true || (item = sameView.BindingContext as T) == null)
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

                    if (item is TaskList)
                    {
                        var colors = Constants.Colors.TaskListColors;
                        backgroundColor = colors[(count - index - 1) % colors.Count];
                    }
                    else
                    {
                        var shades = Constants.Colors.TaskColorsByTaskList["#FF673AB7"];                       
                        backgroundColor = shades[(count - index - 1) % shades.Count];
                    }

                    //item.Realm.Write(() => { item.Color = backgroundColor.ToHexString(); });
                }

                sameView.BackgroundColor = backgroundColor;
            }
            catch
            {
                // Let's not crash because of a coloring fail :)
                Console.WriteLine("Problem on saving color!");
            }
        }
    }
}
