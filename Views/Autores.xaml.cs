namespace PrototipoTrue.Views;

public partial class Autores : ContentPage
{
    public Autores()
    {
        InitializeComponent();
    }
    public void OnClickedAction(object sender, EventArgs e)
    {
        var button = sender as Button;
        string? action = button?.CommandParameter?.ToString();

        switch (action)
        {
            case "Pesquisar":
                Pesquisar.IsVisible = true;
                Selecao.IsVisible = false;
                Adicionar.IsVisible = false;
                Remover.IsVisible = false;
                Atualizar.IsVisible = false;
                break;

            case "Adicionar":
                Pesquisar.IsVisible = false;
                Selecao.IsVisible = false;
                Adicionar.IsVisible = true;
                Remover.IsVisible = false;
                Atualizar.IsVisible = false;
                break;

            case "Remover":
                Pesquisar.IsVisible = false;
                Selecao.IsVisible = false;
                Adicionar.IsVisible = false;
                Remover.IsVisible = true;
                Atualizar.IsVisible = false;
                break;
            case "Atualizar":
                Pesquisar.IsVisible = false;
                Selecao.IsVisible = false;
                Adicionar.IsVisible = false;
                Remover.IsVisible = false;
                Atualizar.IsVisible = true;
                break;

        }
        Opcoes.IsVisible = false;
        BackButton.IsVisible = true;
    }

    public void OnClickedConfirmar(object sender, EventArgs e)
    {
        var button = sender as Button;
        string? action = button?.CommandParameter?.ToString();

        string code = CodeEntry.Text;

        switch (action)
        {
            case "Pesquisar":
                string pesquisarText = PesquisarEntry.Text;
                PesquisarEntry.Text = "";
                DisplayAlert("Confirmar", $"Autor pesquisado: {pesquisarText}", "OK");
                Pesquisar.IsVisible = false;
                break;

            case "Adicionar":
                string nomeText = AdicionarNome.Text;
                string pseudonimoText = AdicionarPseudonimo.Text;
                string? descricaoText = AdicionarDescrição.Text?.Trim(); // Pra não crashar caso não tenha nada na entry
                AdicionarNome.Text = "";
                AdicionarPseudonimo.Text = "";
                AdicionarDescrição.Text = "";
                if (string.IsNullOrEmpty(descricaoText))
                {
                    descricaoText = "Nenhuma descrição adicionada";
                }
                DisplayAlert("Confirmar", $"Autor adicionado: {nomeText}, com o pseudonimo: {pseudonimoText}.\nDescrição: {descricaoText}", "OK");
                Adicionar.IsVisible = false;
                break;

            case "Remover":
                string removerText = RemoverEntry.Text;
                RemoverEntry.Text = "";
                DisplayAlert("Confirmar", $"Autor removido: {removerText}", "OK");
                Remover.IsVisible = false;
                break;

            case "Atualizar":
                string newName = NewAutorNameEntry.Text;
                string newPseudonimo = NewAutorPseudonimoEntry.Text;
                string newDescricao = NewAutorDescriçãoEntry.Text;
                NewAutorNameEntry.Text = "";
                NewAutorPseudonimoEntry.Text = "";
                NewAutorDescriçãoEntry.Text = "";
                CodeEntry.Text = "";
                DisplayAlert("Atualizar", $"Autor de código {code} atualizado:\nNome para: {newName}\nPseudonimo para: {newPseudonimo}\nDescrição para: {newDescricao}", "OK");
                Atualizar.IsVisible = false;
                break;
        }
        Selecao.IsVisible = true;
        Opcoes.IsVisible = true;
        BackButton.IsVisible = false;
    }

    // Esse volta pras opções existentes da seleção
    public void OnClickedBack(object sender, EventArgs e)
    {
        Opcoes.IsVisible = true;
        Selecao.IsVisible = true;
        Pesquisar.IsVisible = false;
        Adicionar.IsVisible = false;
        Atualizar.IsVisible = false;
        Remover.IsVisible = false;
        BackButton.IsVisible = false;
    }

    // Esse volta pra página principal de seleções
    public async void OnClickedVoltar(object sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
        else
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

}
