namespace PrototipoTrue.Views;

public partial class Livros : ContentPage
{
    public Livros()
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
                DisplayAlert("Confirmar", $"Livro pesquisado: {pesquisarText}", "OK");
                Pesquisar.IsVisible = false;
                break;

            case "Adicionar":
                string nomeText = AdicionarNome.Text;
                string anoPublicacaoText = AdicionarAno.Text; // TODO: Converter pra int na implementa��o do banco
                string isbnText = AdicionarISBN.Text; // TODO: Converter pra decimal na implementa��o do banco
                string? descricaoText = AdicionarDescricao.Text?.Trim(); // Pra n�o crashar caso n�o tenha nada na entry
                string autorText = AdicionarAutor.Text; // TODO: Se decidir se isso vai ser c�digo ou nome
                string editoraText = AdicionarEditora.Text; // TODO: Se decidir se isso vai ser c�digo ou nome
                if (string.IsNullOrEmpty(descricaoText))
                {
                    descricaoText = "Nenhuma descri��o adicionada";
                }
                AdicionarNome.Text = "";
                AdicionarAno.Text = "";
                AdicionarISBN.Text = "";
                AdicionarDescricao.Text = "";
                AdicionarAutor.Text = "";
                AdicionarEditora.Text = "";
                DisplayAlert("Confirmar", $"Livro adicionado: {nomeText}, do autor {autorText} e editora {editoraText}\nPublicado em {anoPublicacaoText}\nISBN: {isbnText}\nDescri��o: {descricaoText}", "OK");
                Adicionar.IsVisible = false;
                break;

            case "Remover":
                string removerText = RemoverEntry.Text;
                RemoverEntry.Text = "";
                DisplayAlert("Confirmar", $"Livro removido: {removerText}", "OK");
                Remover.IsVisible = false;
                break;

            case "Atualizar":
                string newName = NewLivroNameEntry.Text;
                string newAno = NewLivroAnoEntry.Text;
                string newISBN = NewLivroISBNEntry.Text;
                string newDescricao = NewLivroDescricaoEntry.Text;
                string newEditora = EditorasidEntry.Text;
                string newAutor = AutoresidEntry.Text;
                NewLivroNameEntry.Text = "";
                NewLivroAnoEntry.Text = "";
                NewLivroISBNEntry.Text = "";
                NewLivroDescricaoEntry.Text = "";
                CodeEntry.Text = "";
                EditorasidEntry.Text = "";
                AutoresidEntry.Text = "";
                DisplayAlert("Atualizar", $"Livro de c�digo {code} atualizado\n\nNovo Nome: {newName}\nNovo Ano: {newAno}\nNovo ISBN: {newISBN}\nNova Descri��o: {newDescricao}\nNova Editora: {newEditora}\nNovo Autor: {newAutor}", "OK"); //TODO (pra todos os 3 eu acho): ADICIONAR CHECK SE O INPUT � IGUAL AO QUE J� EST� INSERIDO, SE INPUT EST� VAZIO, ETC
                Atualizar.IsVisible = false;
                break;
        }
        Selecao.IsVisible = true;
        Opcoes.IsVisible = true;
        BackButton.IsVisible = false;
    }

    // Esse volta pras op��es existentes da sele��o
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

    // Esse volta pra p�gina principal de sele��es
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
