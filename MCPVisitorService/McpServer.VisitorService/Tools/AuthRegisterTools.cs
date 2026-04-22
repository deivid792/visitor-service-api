using System.ComponentModel;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using ModelContextProtocol.Server;

internal class AuthRegisterTools
{
    [McpServerTool]
    [Description("Registra um novo usuário. Envie os dados exatamente como fornecidos pelo usuário. Não valide a senha localmente; a API retornará erros específicos caso a validação falhe. Parammetros obrigatórios: nome, email, senha. Parammetros Opcionais: telefone, empresa, cnpj. Todos os parametros são strings")]
    public async Task<string> RegistrarUsuario(string nome, string email, string senha, string? telefone = null, string? empresa = null, string? cnpj = null)
    {
        Console.WriteLine($"[TOOL] Tentando registrar: {email}"); // LOG 1
        try
        {
            var userData = new {name = nome, email, password = senha, phone = telefone, company = empresa, cnpj};

            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync("http://localhost:5057/api/auth/register", userData);

            Console.WriteLine($"[TOOL] API respondeu com: {response.StatusCode}"); // LOG 2

            if(response.IsSuccessStatusCode)
            {
                return $"Usuario registrado com sucesso";
            }

            else
            {
                var erroDetalhado = await response.Content.ReadAsStringAsync();
                return $"A API retornou erro: {erroDetalhado}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return  $"Erro {ex.Message}";
        }
    }
}