namespace PrototipoTrue.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    private async void onFlyoutIconClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Selecoes());
        Shell.Current.FlyoutIsPresented = true;

    }
}
