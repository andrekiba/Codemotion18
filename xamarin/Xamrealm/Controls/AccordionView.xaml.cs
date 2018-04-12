using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamrealm.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccordionView : ContentView
    {
        #region Bindable Properties

        public static BindableProperty headerBackgroundColorProperty =
            BindableProperty.Create(nameof(HeaderBackgroundColor),
                typeof(Color),
                typeof(AccordionView),
                defaultValue: Color.Transparent,
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((AccordionView)bindable).UpdateHeaderBackgroundColor();
                });

        public static BindableProperty headerOpenedBackgroundColorProperty =
            BindableProperty.Create(nameof(HeaderOpenedBackgroundColor),
                typeof(Color),
                typeof(AccordionView),
                defaultValue: Color.Transparent,
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((AccordionView)bindable).UpdateHeaderBackgroundColor();
                });

        public static BindableProperty headerTextColorProperty =
            BindableProperty.Create(nameof(HeaderTextColor),
                typeof(Color),
                typeof(AccordionView),
                defaultValue: Color.Black,
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((AccordionView)bindable).UpdateHeaderTextColor((Color)newVal);
                });

        public static BindableProperty headerTextProperty =
            BindableProperty.Create(nameof(headerTextProperty),
                typeof(string),
                typeof(AccordionView),
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((AccordionView)bindable).UpdateHeaderText((string)newVal);
                });

        public static BindableProperty bodyTextColorProperty =
            BindableProperty.Create(nameof(BodyTextColor),
                typeof(Color),
                typeof(AccordionView),
                defaultValue: Color.Black,
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((AccordionView) bindable).UpdateBodyTextColor((Color) newVal);
                });

        public static BindableProperty bodyTextProperty =
            BindableProperty.Create(nameof(BodyText),
                typeof(string),
                typeof(AccordionView),
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((AccordionView)bindable).UpdateBodyText((string)newVal);
                });


        public static BindableProperty isBodyVisibleProperty =
            BindableProperty.Create(nameof(IsBodyVisible),
                typeof(bool),
                typeof(AccordionView),
                defaultValue: true,
                propertyChanged: (bindable, oldVal, newVal) =>
                {
                    ((AccordionView)bindable).UpdateBodyVisibility((bool)newVal);
                });
        #endregion

        #region Public Properties
        public Color HeaderBackgroundColor
        {
            get => (Color)GetValue(headerBackgroundColorProperty);
            set => SetValue(headerBackgroundColorProperty, value);
        }
        public Color HeaderOpenedBackgroundColor
        {
            get => (Color)GetValue(headerOpenedBackgroundColorProperty);
            set => SetValue(headerOpenedBackgroundColorProperty, value);
        }
        public Color HeaderTextColor
        {
            get => (Color)GetValue(headerTextColorProperty);
            set => SetValue(headerTextColorProperty, value);
        }
        public string HeaderText
        {
            get => (string)GetValue(headerTextProperty);
            set => SetValue(headerTextProperty, value);
        }
        public Color BodyTextColor
        {
            get => (Color)GetValue(bodyTextColorProperty);
            set => SetValue(bodyTextColorProperty, value);
        }
        public string BodyText
        {
            get => (string)GetValue(bodyTextProperty);
            set => SetValue(bodyTextProperty, value);
        }

        public bool IsBodyVisible
        {
            get => (bool)GetValue(isBodyVisibleProperty);
            set => SetValue(isBodyVisibleProperty, value);
        }

        #endregion

        public AccordionView()
        {
            InitializeComponent();
            IsBodyVisible = false;
        }

        public void UpdateHeaderBackgroundColor(Color color) => HeaderView.BackgroundColor = color;

        public void UpdateHeaderBackgroundColor()
        {
            if (IsBodyVisible)
            {
                HeaderView.BackgroundColor = HeaderOpenedBackgroundColor;
                BodyView.BackgroundColor = HeaderOpenedBackgroundColor;
            }
            else
            {
                HeaderView.BackgroundColor = HeaderBackgroundColor;
            }
        }

        public void UpdateHeaderTextColor(Color color) => HeaderLabel.TextColor = color;

        public void UpdateBodyTextColor(Color color) => BodyLabel.TextColor = color;

        public void UpdateHeaderText(string text) => HeaderLabel.Text = text;

        public void UpdateBodyText(string text) => BodyLabel.Text = text;

        public void UpdateBodyVisibility(bool isVisible)
        {
            BodyView.IsVisible = isVisible;
            IndicatorLabel.Text = "+";
            IndicatorLabel.RotateTo(isVisible ? 45 : 0, 100);
        }

        private void HandleTapped(object sender, System.EventArgs e)
        {
            IsBodyVisible = !IsBodyVisible;
            UpdateHeaderBackgroundColor();
        }
    }
}