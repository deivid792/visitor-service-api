using System.ComponentModel;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using ModelContextProtocol.Server;

internal class ManagerVisitsTools
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ManagerVisitsTools(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    [McpServerTool]
    [Description("Lista todas as visitas pendentes para aprovação.")]
    public async Task<string> ListarVisitasPendentes()
    {
        var authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorization))
        {
            return "Erro: Token de autorização não encontrado no repasse.";
        }

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", authorization);

            var response = await client.GetAsync("http://localhost:5057/api/Visitor/gestor");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return json; 
            }

            var erroMsg = await response.Content.ReadAsStringAsync();
            return $"A API de negócio retornou erro: {response.StatusCode} - {erroMsg}";
        }
        catch (Exception ex)
        {
            return $"Falha na listagem das visitas: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Aprova ou rejeita uma visita. Requer o visitId e o status (Approved ou Rejected).")]
    public async Task<string> AtualizarStatusVisita(string visitId, string status)
    {
        var authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorization))
        {
            return "Erro: Token de autorização não encontrado no repasse.";
        }

        var statusFinal = status.Trim().ToLower().Contains("aprov") ? "Approved" : 
        status.Trim().ToLower().Contains("rejeit") ? "Rejected" : status;
        try
        {
            var updateData = new { 
                visitId = Guid.Parse(visitId), 
                status = statusFinal 
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", authorization);

            var response = await client.PutAsJsonAsync("http://localhost:5057/api/Visitor/status", updateData);

            if (response.IsSuccessStatusCode)
            {
                return $"STATUS: SUCESSO. A visita {visitId} foi atualizada para {status}.";
            }

            var erroMsg = await response.Content.ReadAsStringAsync();
            return $"A API de negócio retornou erro: {response.StatusCode} - {erroMsg}";
        }
        catch (Exception ex)
        {
            return $"Falha ao atualizar status: {ex.Message}";
        }
    }
}