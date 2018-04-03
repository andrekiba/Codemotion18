using System;
using System.Collections.Generic;
using System.Text;
using Realms;
using Xamarin.Forms;
using Xamrealm.Base;
using Xamrealm.Models;
using Task = System.Threading.Tasks.Task;

namespace Xamrealm.Behaviors
{
    public class BoardColoringBehavior<T> : Behavior<View> where T : RealmObject, ICompletable
    {
        private IDisposable notificationToken;
        private WeakReference<View> view;

        public static BindableProperty RealmCollectionProperty =
            BindableProperty.Create(nameof(RealmCollection), typeof(IList<T>), typeof(BoardColoringBehavior<T>), propertyChanged: OnRealmCollectionChanged);

        public IList<T> RealmCollection
        {
            get => (IList<T>)GetValue(RealmCollectionProperty);
            set
            {
                SetValue(RealmCollectionProperty, value);
                CalculateColor();
            }
        }

        public Color[] BoardColors { get; set; }

        public Color CompletedColor { get; set; }

        private static void OnRealmCollectionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var self = (BoardColoringBehavior<T>)bindable;
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
            T board;
            if (RealmCollection == null || view?.TryGetTarget(out sameView) != true || (board = sameView.BindingContext as T) == null)
                return;
            try
            {
                Color backgroundColor;
                if (board.Done)
                    backgroundColor = CompletedColor;
                else
                {
                    var index = RealmCollection.IndexOf(board);
                    var boardCount = RealmCollection.Count;
                    
                    backgroundColor = BoardColors[(boardCount - index - 1) % BoardColors.Length];
                }

                sameView.BackgroundColor = backgroundColor;
            }
            catch
            {
                // Let's not crash because of a coloring fail :)
            }
        }

        private Color GradientColorAtFraction(double fraction)
        {
            // Ensure offset is normalized to 1
            var normalizedOffset = Math.Max(Math.Min(fraction, 1), 0);

            // Work out the 'size' that each color stop spans
            var colorStopRange = 1.0 / (BoardColors.Length - 1);

            // Determine the base stop our offset is within
            var colorRangeIndex = (int)Math.Floor(normalizedOffset / colorStopRange);

            // Get the initial color which will serve as the origin
            var topColor = BoardColors[colorRangeIndex];
            var fromColors = new[] { topColor.R, topColor.G, topColor.B };

            // Get the destination color we will lerp to
            var bottomColor = BoardColors[colorRangeIndex + 1];
            var toColors = new[] { bottomColor.R, bottomColor.G, bottomColor.B };

            // Work out the actual percentage we need to lerp, inside just that stop range
            var stopOffset = (normalizedOffset - colorRangeIndex * colorStopRange) / colorStopRange;

            return new Color(
                fromColors[0] + stopOffset * (toColors[0] - fromColors[0]),
                fromColors[1] + stopOffset * (toColors[1] - fromColors[1]),
                fromColors[2] + stopOffset * (toColors[2] - fromColors[2]));
        }
    }
}
