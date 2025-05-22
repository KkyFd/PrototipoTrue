using PrototipoTrue.Models;
using System.Collections.ObjectModel;

namespace PrototipoTrue.Views;

public partial class Editoras : ContentPage
{
    private ObservableCollection<Editora> editoras = new();

    public Editoras()
    {
        InitializeComponent();
        Inicializar();
    }

    private async void Inicializar()
    {
        await CarregarEditoras();
        ResetarVisibilidade();
    }

    private async Task CarregarEditoras()
    {
        var lista = await App.Db.GetAll<Editora>();
        editoras.Clear();
        foreach (var item in lista)
            editoras.Add(item);
        editorasList.ItemsSource = editoras;
    }

    private void MostrarSecao(string acao)
    {
        Pesquisar.IsVisible = false;
        Adicionar.IsVisible = false;
        Remover.IsVisible = false;
        Atualizar.IsVisible = false;

        switch (acao)
        {
            case "Pesquisar":
                Pesquisar.IsVisible = true;
                PesquisaListagem.IsVisible = true;
                editorasList.ItemsSource = editoras;
                break;
            case "Adicionar":
                Adicionar.IsVisible = true;
                break;
            case "Remover":
                Remover.IsVisible = true;
                break;
            case "Atualizar":
                Atualizar.IsVisible = true;
                break;
        }

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
        BackButton.IsVisible = false;
        PesquisaListagem.IsVisible = false;

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

    private async void editorasList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Editora editora)
        {
            await DisplayAlert("Detalhes da Editora",
                $"ID: {editora.ID}\nNome: {editora.Nome}\nSigla: {editora.Sigla}\nDescrição: {editora.Descricao}",
                "OK");

        }
    }


    private async Task ConfirmarPesquisar()
    {
        string termo = editorasSearchBar.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(termo))
        {
            await DisplayAlert("Erro", "Digite um texto para pesquisar.", "OK");
            return;
        }

        var lista = await App.Db.Search<Editora>("Nome", termo);
        if (lista.Any())
        {
            var msg = string.Join("\n", lista.Select(e => $"{e.ID} - {e.Nome} ({e.Sigla})"));
            await DisplayAlert("Resultado", msg, "OK");
        }
        else
        {
            await DisplayAlert("Resultado", "Nenhuma editora encontrada.", "OK");
        }

        editorasList.ItemsSource = null;
        PesquisaListagem.IsVisible = true;
    }

    private async Task ConfirmarAdicionar()
    {
        string nome = AdicionarNome.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(nome))
        {
            await DisplayAlert("Erro", "O nome da editora é obrigatório.", "OK");
            return;
        }

        string sigla = AdicionarSigla.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(sigla))
        {
            await DisplayAlert("Erro", "A sigla da editora é obrigatória.", "OK");
            return;
        }

        string descricao = AdicionarDescrição.Text?.Trim() ?? "";

        Editora editora = new()
        {
            Nome = nome,
            Sigla = sigla,
            Descricao = descricao
        };

        await App.Db.Insert(editora);
        await DisplayAlert("Sucesso", "Editora adicionada com sucesso.", "OK");

        AdicionarNome.Text = "";
        AdicionarSigla.Text = "";
        AdicionarDescrição.Text = "";

        await CarregarEditoras();
    }

    private async Task ConfirmarRemover()
    {
        if (!int.TryParse(RemoverEntry.Text?.Trim(), out int id))
        {
            await DisplayAlert("Erro", "Código inválido.", "OK");
            return;
        }

        var editora = (await App.Db.GetAll<Editora>()).FirstOrDefault(x => x.ID == id);
        if (editora == null)
        {
            await DisplayAlert("Erro", "Editora não encontrada.", "OK");
            return;
        }

        await App.Db.Delete<Editora>(editora.ID);
        await DisplayAlert("Sucesso", "Editora removida com sucesso.", "OK");

        RemoverEntry.Text = "";
        await CarregarEditoras();
    }

    private async Task ConfirmarAtualizar()
    {
        if (!int.TryParse(CodeEntry.Text?.Trim(), out int id))
        {
            await DisplayAlert("Erro", "Código inválido.", "OK");
            return;
        }

        var editora = (await App.Db.GetAll<Editora>()).FirstOrDefault(x => x.ID == id);
        if (editora == null)
        {
            await DisplayAlert("Erro", "Editora não encontrada.", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(NewEditoraNameEntry.Text))
            editora.Nome = NewEditoraNameEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(NewEditoraSiglaEntry.Text))
            editora.Sigla = NewEditoraSiglaEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(NewDescricaoEditoraEntry.Text))
            editora.Descricao = NewDescricaoEditoraEntry.Text.Trim();

        await App.Db.Update(editora);
        await DisplayAlert("Sucesso", "Editora atualizada com sucesso.", "OK");

        CodeEntry.Text = "";
        NewEditoraNameEntry.Text = "";
        NewEditoraSiglaEntry.Text = "";
        NewDescricaoEditoraEntry.Text = "";

        await CarregarEditoras();
    }

    private void editorasSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textoBusca = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(textoBusca))
        {
            editorasList.ItemsSource = editoras;
        }
        else
        {
            var filtrados = editoras.Where(e =>
                (e.Nome?.ToLower().Contains(textoBusca) ?? false) ||
                (e.Sigla?.ToLower().Contains(textoBusca) ?? false) ||
                (e.Descricao?.ToLower().Contains(textoBusca) ?? false)
            ).ToList();


            editorasList.ItemsSource = filtrados;
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
