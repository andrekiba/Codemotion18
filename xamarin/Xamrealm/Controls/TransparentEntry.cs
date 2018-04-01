using Xamarin.Forms;

namespace Xamrealm.Controls
{
    public class TransparentEntry : Entry
    {
        public static readonly BindableProperty IsStrikeThroughProperty = BindableProperty.Create(nameof(IsStrikeThrough), typeof(bool), typeof(TransparentEntry), false);

        public bool IsStrikeThrough
        {
            get => (bool)GetValue(IsStrikeThroughProperty);
            set => SetValue(IsStrikeThroughProperty, value);
        }
    }
}
