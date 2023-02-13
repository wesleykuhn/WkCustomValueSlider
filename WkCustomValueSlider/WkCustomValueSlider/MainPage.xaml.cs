using Xamarin.Forms;

namespace WkCustomValueSlider
{
    public partial class MainPage : ContentPage
    {
        public readonly static BindableProperty ValueProperty =
                BindableProperty.Create(nameof(Value), typeof(int), typeof(MainPage), 0, BindingMode.TwoWay);
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
