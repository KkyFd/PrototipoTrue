using PrototipoTrue.Models;
using System.Collections.ObjectModel;

namespace PrototipoTrue.Views;

public partial class Livros : ContentPage
{
    private ObservableCollection<Livro> livros = new();
    private List<Autor> autores = new();
    private List<Editora> editoras = new();

    public Livros()
    {
        InitializeComponent();
        InicializarDados();
    }

    private async void InicializarDados()
    {
        await CarregarAutoresEEditorasNosPickers();
        await CarregarLivros();
        ResetarVisibilidade();
    }

    private void ConfigurarPicker(Picker picker, List<object> itens)
    {
        picker.ItemsSource = itens;
        picker.ItemDisplayBinding = new Binding("Nome");
    }

    private async Task CarregarAutoresEEditorasNosPickers()
    {
        autores = (await App.Db.GetAll<Autor>()).ToList();
        editoras = (await App.Db.GetAll<Editora>()).ToList();

        ConfigurarPicker(AdicionarAutorPicker, autores.Cast<object>().ToList());
        ConfigurarPicker(AdicionarEditoraPicker, editoras.Cast<object>().ToList());
        ConfigurarPicker(AtualizarAutorPicker, autores.Cast<object>().ToList());
        ConfigurarPicker(AtualizarEditoraPicker, editoras.Cast<object>().ToList());
    }

    private async Task CarregarLivros()
    {
        var livrosDoBanco = await App.Db.GetAll<Livro>();
        livros.Clear();

        foreach (var livro in livrosDoBanco)
        {
            livro.AutorNome = autores.FirstOrDefault(a => a.ID == livro.AutorID)?.Nome ?? "Autor desconhecido";
            livro.EditoraNome = editoras.FirstOrDefault(e => e.ID == livro.EditoraID)?.Nome ?? "Editora desconhecida";
            livros.Add(livro);
        }

        livrosList.ItemsSource = livros;
    }

    private void MostrarSecao(string acao)
    {
        Pesquisar.IsVisible = acao == "Pesquisar";
        Adicionar.IsVisible = acao == "Adicionar";
        Remover.IsVisible = acao == "Remover";
        Atualizar.IsVisible = acao == "Atualizar";
        PesquisaListagem.IsVisible = acao == "Pesquisar";

        Selecao.IsVisible = false;
        Opcoes.IsVisible = false;
        BackButton.IsVisible = true;
    }


    private void ResetarVisibilidade()
    {
        Pesquisar.IsVisible = false;
        Adicionar.IsVisible = false;
        Remover.IsVisible = false;
        Atualizar.IsVisible = false;
        PesquisaListagem.IsVisible = false;

        Selecao.IsVisible = true;
        Opcoes.IsVisible = true;
        BackButton.IsVisible = false;
    }

    public void OnClickedAction(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string acao)
        {
            MostrarSecao(acao);
        }
    }

    public async void OnClickedConfirmar(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string acao)
        {
            switch (acao)
            {
                case "Pesquisar":
                    await ConfirmarPesquisar();
                    break;
                case "Adicionar":
                    await ConfirmarAdicionar();
                    break;
                case "Remover":
                    await ConfirmarRemover();
                    break;
                case "Atualizar":
                    await ConfirmarAtualizar();
                    break;
            }
        }
        ResetarVisibilidade();
    }

    private async Task ConfirmarPesquisar()
    {
        string texto = livrosSearchBar.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(texto))
        {
            await DisplayAlert("Erro", "Digite um texto para pesquisar.", "OK");
            return;
        }

        var encontrados = await App.Db.Search<Livro>("Nome", texto);
        if (encontrados.Any())
        {
            var lista = string.Join("\n", encontrados.Select(l => $"{l.ID} - {l.Nome}"));
            await DisplayAlert("Resultado da Pesquisa", lista, "OK");

            livrosList.ItemsSource = encontrados;
            PesquisaListagem.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Resultado da Pesquisa", "Nenhum livro encontrado.", "OK");
            livrosList.ItemsSource = null;
            PesquisaListagem.IsVisible = false;
        }
    }


    private async Task ConfirmarAdicionar()
    {
        string nome = AdicionarNome.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(nome))
        {
            await DisplayAlert("Erro", "O nome do livro é obrigatório.", "OK");
            return;
        }

        if (!int.TryParse(AdicionarAno.Text, out int ano))
        {
            await DisplayAlert("Erro", "Ano de publicação inválido.", "OK");
            return;
        }

        string isbn = AdicionarISBN.Text?.Trim() ?? "";
        string descricao = AdicionarDescricao.Text?.Trim() ?? "Nenhuma descrição adicionada";

        if (AdicionarAutorPicker.SelectedItem is not Autor autor || AdicionarEditoraPicker.SelectedItem is not Editora editora)
        {
            await DisplayAlert("Erro", "Selecione um autor e uma editora.", "OK");
            return;
        }

        Livro novoLivro = new()
        {
            Nome = nome,
            Ano = ano,
            ISBN = isbn,
            Descricao = descricao,
            AutorID = autor.ID,
            EditoraID = editora.ID
        };

        await App.Db.Insert(novoLivro);
        await DisplayAlert("Sucesso", $"Livro '{nome}' adicionado com sucesso.", "OK");

        AdicionarNome.Text = AdicionarAno.Text = AdicionarISBN.Text = AdicionarDescricao.Text = "";
        AdicionarAutorPicker.SelectedItem = AdicionarEditoraPicker.SelectedItem = null;

        await CarregarLivros();
    }

    private async Task ConfirmarRemover()
    {
        if (!int.TryParse(RemoverEntry.Text?.Trim(), out int codigo))
        {
            await DisplayAlert("Erro", "Código inválido para remoção.", "OK");
            return;
        }

        var livro = (await App.Db.GetAll<Livro>()).FirstOrDefault(l => l.ID == codigo);
        if (livro == null)
        {
            await DisplayAlert("Erro", "Livro não encontrado para remoção.", "OK");
            return;
        }

        await App.Db.Delete<Livro>(livro.ID);
        await DisplayAlert("Sucesso", $"Livro '{livro.Nome}' removido com sucesso.", "OK");

        RemoverEntry.Text = "";
        await CarregarLivros();
    }

    private async Task ConfirmarAtualizar()
    {
        if (!int.TryParse(CodeEntry.Text, out int codigo))
        {
            await DisplayAlert("Erro", "Código inválido para atualização.", "OK");
            return;
        }

        var livro = (await App.Db.GetAll<Livro>()).FirstOrDefault(l => l.ID == codigo);
        if (livro == null)
        {
            await DisplayAlert("Erro", "Livro não encontrado para atualização.", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(NewLivroNameEntry.Text)) livro.Nome = NewLivroNameEntry.Text.Trim();
        if (int.TryParse(NewLivroAnoEntry.Text, out int novoAno)) livro.Ano = novoAno;
        if (!string.IsNullOrWhiteSpace(NewLivroISBNEntry.Text)) livro.ISBN = NewLivroISBNEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(NewLivroDescricaoEntry.Text)) livro.Descricao = NewLivroDescricaoEntry.Text.Trim();

        if (AtualizarAutorPicker.SelectedItem is Autor autor) livro.AutorID = autor.ID;
        if (AtualizarEditoraPicker.SelectedItem is Editora editora) livro.EditoraID = editora.ID;

        await App.Db.Update(livro);
        await DisplayAlert("Sucesso", $"Livro de código {codigo} atualizado com sucesso.", "OK");

        CodeEntry.Text = NewLivroNameEntry.Text = NewLivroAnoEntry.Text = NewLivroISBNEntry.Text = NewLivroDescricaoEntry.Text = "";
        AtualizarAutorPicker.SelectedItem = AtualizarEditoraPicker.SelectedItem = null;

        await CarregarLivros();
    }

    private void livrosSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textoBusca = e.NewTextValue?.ToLower() ?? "";
        var filtrados = livros.Where(l =>
            l.Nome.ToLower().Contains(textoBusca) ||
            (l.AutorNome?.ToLower().Contains(textoBusca) ?? false) ||
            (l.EditoraNome?.ToLower().Contains(textoBusca) ?? false)
        ).ToList();

        livrosList.ItemsSource = filtrados;
        PesquisaListagem.IsVisible = true;
    }

    private void livrosList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Livro selecionado)
        {
            livrosSearchBar.Text = selecionado.Nome;
            livrosList.SelectedItem = null;
        }
    }

    private void OnClickedBack(object sender, EventArgs e)
    {
        ResetarVisibilidade();
    }

    public async void OnClickedVoltar(object sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
            await Navigation.PopAsync();
        else
            await Shell.Current.GoToAsync("//MainPage");
    }
}
