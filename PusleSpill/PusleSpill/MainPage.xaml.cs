namespace PusleSpill
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnPuzzleSelected(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string puzzleName = button.Text;

            await Navigation.PushAsync(new GamePage(puzzleName));
        }
    }
}