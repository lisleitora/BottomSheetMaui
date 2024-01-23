namespace BottomSheetMaui.Sample
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Popup_Clicked(object sender, EventArgs e)
        {
           pgNameLabel.Text = await Popup.Open(new PopExample());
            pgNameLabel.Text = "Your name is: "+pgNameLabel.Text;
        }
    }

}
