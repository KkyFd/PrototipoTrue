namespace PrototipoTrue.Views;

public partial class Selecoes : ContentPage
{
    public Selecoes()
    {
        InitializeComponent();
    }

    private async void OnClickedSelecaoFrame(object sender, EventArgs e)
    {
        string? tappedParameter = (sender as Frame)?.GestureRecognizers.OfType<TapGestureRecognizer>()
                                      .FirstOrDefault()?.CommandParameter as string;

        await Navegar(tappedParameter);
    }


    private async void OnClickedSelecaoBotao(object sender, EventArgs e)
    {
        string? selectedOption = (sender as ImageButton)?.CommandParameter as string;

        await Navegar(selectedOption);
    }

    private async Task Navegar(string? option)
    {
        switch (option)
        {
            case "Autores":
                await Navigation.PushAsync(new Autores());
                break;
            case "Livros":
                await Navigation.PushAsync(new Livros());
                break;
            case "Editoras":
                await Navigation.PushAsync(new Editoras());
                break;
            default:
                await DisplayAlert("Erro", $"Opção não encontrada: {option}", "OK");
                break;
        }
    }


    // Como o flyout tem prioridade de click em qualquer lugar da tela, não consegui achar uma maneira de fazer ele fechar, 
    // e ao mesmo tempo voltar ao menu principal, então vai ficar assim mesmo 
    private async void OnClickedVoltar(object sender, EventArgs e)
    {
        Shell.Current.FlyoutIsPresented = false;
        await Navigation.PopToRootAsync();
    }
}