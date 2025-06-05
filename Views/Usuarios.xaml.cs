using Microsoft.UI.Xaml.Controls.Primitives;
using PrototipoTrue.Models;
using System.Collections.ObjectModel;

namespace PrototipoTrue.Views;

public partial class Usuarios : ContentPage
{
    private ObservableCollection<Usuario> usuarios = new();

    public Usuarios()
    {
        InitializeComponent();
        Inicializar();
    }

    private async void Inicializar()
    {
        await CarregarUsuarios();
        ResetarVisibilidade();
    }

    private async Task CarregarUsuarios()
    {
        var lista = await App.Db.GetAll<Usuario>();
        usuarios.Clear();
        foreach (var item in lista)
            usuarios.Add(item);
        usuariosList.ItemsSource = usuarios;
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
                usuariosList.ItemsSource = usuarios;
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

    private async Task ConfirmarPesquisar()
    {
        string termo = usuariosSearchBar.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(termo))
        {
            await DisplayAlert("Erro", "Digite um texto para pesquisar.", "OK");
            return;
        }

        var lista = await App.Db.Search<Usuario>("Nome", termo);
        if (lista.Any())
        {
            var msg = string.Join("\n", lista.Select(u => $"{u.Codigo} - {u.Nome}"));
            await DisplayAlert("Resultado", msg, "OK");
        }
        else
        {
            await DisplayAlert("Resultado", "Nenhum usuário encontrado.", "OK");
        }

        usuariosList.ItemsSource = null;
        PesquisaListagem.IsVisible = true;
    }

    private async Task ConfirmarAdicionar()
    {
        string nome = AdicionarNome.Text?.Trim() ?? "";
        string senha = AdicionarSenha.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(senha))
        {
            await DisplayAlert("Erro", "O nome e a senha são obrigatórios.", "OK");
            return;
        }

        Usuario usuario = new()
        {
            Nome = nome,
            Senha = senha
        };

        await App.Db.Insert(usuario);
        await DisplayAlert("Sucesso", "Usuário adicionado com sucesso.", "OK");

        AdicionarNome.Text = "";
        AdicionarSenha.Text = "";

        await CarregarUsuarios();
    }

    private async Task ConfirmarRemover()
    {
        if (!int.TryParse(RemoverEntry.Text?.Trim(), out int id))
        {
            await DisplayAlert("Erro", "Código inválido.", "OK");
            return;
        }

        var usuario = (await App.Db.GetAll<Usuario>()).FirstOrDefault(x => x.Codigo == id);
        if (usuario == null)
        {
            await DisplayAlert("Erro", "Usuário não encontrado.", "OK");
            return;
        }

        await App.Db.Delete<Usuario>(usuario.Codigo);
        await DisplayAlert("Sucesso", "Usuário removido com sucesso.", "OK");

        RemoverEntry.Text = "";
        await CarregarUsuarios();
    }

    private async void usuariosList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Usuario usuario)
        {
            await DisplayAlert("Detalhes do Usuário",
                $"Código: {usuario.Codigo}\nNome: {usuario.Nome}\nSenha: {usuario.Senha}",
                "OK");
        }
    }

    private async Task ConfirmarAtualizar()
    {
        if (!int.TryParse(CodeEntry.Text?.Trim(), out int id))
        {
            await DisplayAlert("Erro", "Código inválido.", "OK");
            return;
        }

        var usuario = (await App.Db.GetAll<Usuario>()).FirstOrDefault(x => x.Codigo == id);
        if (usuario == null)
        {
            await DisplayAlert("Erro", "Usuário não encontrado.", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(NewUsuarioNameEntry.Text))
            usuario.Nome = NewUsuarioNameEntry.Text.Trim();
        if (!string.IsNullOrWhiteSpace(NewUsuarioSenhaEntry.Text))
            usuario.Senha = NewUsuarioSenhaEntry.Text.Trim();

        await App.Db.Update(usuario);
        await DisplayAlert("Sucesso", "Usuário atualizado com sucesso.", "OK");

        CodeEntry.Text = "";
        NewUsuarioNameEntry.Text = "";
        NewUsuarioSenhaEntry.Text = "";

        await CarregarUsuarios();
    }

    private void usuariosSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textoBusca = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(textoBusca))
        {
            usuariosList.ItemsSource = usuarios;
        }
        else
        {
            var filtrados = usuarios.Where(u =>
                (u.Nome?.ToLower().Contains(textoBusca) ?? false)
            ).ToList();

            usuariosList.ItemsSource = filtrados;
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
