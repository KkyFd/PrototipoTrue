using PrototipoTrue.Models;
using System.Collections.ObjectModel;

namespace PrototipoTrue.Views;

public partial class Autores : ContentPage
{
    private ObservableCollection<Autor> autores = new();

    public Autores()
    {
        InitializeComponent();
        Inicializar();
    }

    private async void Inicializar()
    {
        await CarregarAutores();
        ResetarVisibilidade();
    }

    private async Task CarregarAutores()
    {
        var lista = await App.Db.GetAll<Autor>();
        autores.Clear();
        foreach (var item in lista)
            autores.Add(item);
        autoresList.ItemsSource = autores;
    }

    private void MostrarSecao(string acao)
    {
        Pesquisar.IsVisible = acao == "Pesquisar";
        Adicionar.IsVisible = acao == "Adicionar";
        Remover.IsVisible = acao == "Remover";
        Atualizar.IsVisible = acao == "Atualizar";
        BackButton.IsVisible = true;

        Selecao.IsVisible = false;
        Opcoes.IsVisible = false;
    }

    private void ResetarVisibilidade()
    {
        Pesquisar.IsVisible = false;
        Adicionar.IsVisible = false;
        Remover.IsVisible = false;
        Atualizar.IsVisible = false;
        BackButton.IsVisible = false;
        PesquisaListagem.IsVisible = false;
        autoresSearchBar.IsVisible = false;

        Selecao.IsVisible = true;
        Opcoes.IsVisible = true;
    }

    public void OnClickedAction(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string acao)
            MostrarSecao(acao);
    }

    public async void OnClickedConfirmar(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string acao)
        {
            if (acao == "Pesquisar") await ConfirmarPesquisar();
            else if (acao == "Adicionar") await ConfirmarAdicionar();
            else if (acao == "Remover") await ConfirmarRemover();
            else if (acao == "Atualizar") await ConfirmarAtualizar();
            ResetarVisibilidade();
        }
    }

    private async Task ConfirmarPesquisar()
    {
        string termo = autoresSearchBar.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(termo))
        {
            await DisplayAlert("Erro", "Digite um texto para pesquisar.", "OK");
            return;
        }

        var lista = await App.Db.Search<Autor>("Nome", termo);
        if (lista.Any())
        {
            var msg = string.Join("\n", lista.Select(a => $"{a.ID} - {a.Nome}"));
            await DisplayAlert("Resultado", msg, "OK");
        }
        else
        {
            await DisplayAlert("Resultado", "Nenhum autor encontrado.", "OK");
        }

        autoresList.ItemsSource = null;
        PesquisaListagem.IsVisible = true;
    }

    private async Task ConfirmarAdicionar()
    {
        string nome = AdicionarNome.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(nome))
        {
            await DisplayAlert("Erro", "O nome do autor é obrigatório.", "OK");
            return;
        }

        string pseudonimo = AdicionarPseudonimo.Text?.Trim() ?? "";
        string descricao = AdicionarDescrição.Text?.Trim() ?? "";

        Autor autor = new()
        {
            Nome = nome,
            Pseudonimo = pseudonimo,
            Descricao = descricao
        };

        await App.Db.Insert(autor);
        await DisplayAlert("Sucesso", "Autor adicionado com sucesso.", "OK");

        AdicionarNome.Text = "";
        AdicionarPseudonimo.Text = "";
        AdicionarDescrição.Text = "";

        await CarregarAutores();
    }

    private async Task ConfirmarRemover()
    {
        if (!int.TryParse(RemoverEntry.Text?.Trim(), out int id))
        {
            await DisplayAlert("Erro", "Código inválido.", "OK");
            return;
        }

        var autor = (await App.Db.GetAll<Autor>()).FirstOrDefault(x => x.ID == id);
        if (autor == null)
        {
            await DisplayAlert("Erro", "Autor não encontrado.", "OK");
            return;
        }

        await App.Db.Delete<Autor>(autor.ID);
        await DisplayAlert("Sucesso", "Autor removido com sucesso.", "OK");

        RemoverEntry.Text = "";
        await CarregarAutores();
    }

    private async Task ConfirmarAtualizar()
    {
        if (!int.TryParse(CodeEntry.Text?.Trim(), out int id))
        {
            await DisplayAlert("Erro", "Código inválido.", "OK");
            return;
        }

        var autor = (await App.Db.GetAll<Autor>()).FirstOrDefault(x => x.ID == id);
        if (autor == null)
        {
            await DisplayAlert("Erro", "Autor não encontrado.", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(NewAutorNameEntry.Text))
            autor.Nome = NewAutorNameEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(NewAutorPseudonimoEntry.Text))
            autor.Pseudonimo = NewAutorPseudonimoEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(NewAutorDescriçãoEntry.Text))
            autor.Descricao = NewAutorDescriçãoEntry.Text.Trim();

        await App.Db.Update(autor);
        await DisplayAlert("Sucesso", "Autor atualizado com sucesso.", "OK");

        CodeEntry.Text = "";
        NewAutorNameEntry.Text = "";
        NewAutorPseudonimoEntry.Text = "";
        NewAutorDescriçãoEntry.Text = "";

        await CarregarAutores();
    }

    private void autoresSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textoBusca = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(textoBusca))
        {
            autoresList.ItemsSource = autores;
        }
        else
        {
            var filtrados = autores.Where(a =>
                (a.Nome?.ToLower().Contains(textoBusca) ?? false)
            ).ToList();

            autoresList.ItemsSource = filtrados;
        }

        PesquisaListagem.IsVisible = true;
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
