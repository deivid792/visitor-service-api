using System.ComponentModel;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;

internal class CreateVisitsTools
{

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateVisitsTools(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    [McpServerTool]
    [Description ("Método para criar uma visita no sistema. Informe a data em formato brasileiro (DD/MM/AAAA) e hora (HH:mm).")]
    public async Task<string> CriarVisita(string data, string hora, string motivo, string categoria )
    {
        var authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorization))
        {
            return "Erro: Token de autorização não encontrado no repasse.";
        }

        if (DateTime.TryParse(data, new System.Globalization.CultureInfo("pt-BR"), out DateTime parsedDate))
        {
            data = parsedDate.ToString("yyyy-MM-dd");
        }

        try{
            var userData = new {
                date = data,
                time = hora,
                reason = motivo,
                category = categoria};

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization",authorization );

            var response = await client.PostAsJsonAsync("http://localhost:5057/api/Visitor", userData );

            if (response.IsSuccessStatusCode)
            {
                return $"STATUS: SUCESSO. A visita foi registrada no banco de dados para o dia {data}. Não chame esta ferramenta novamente para este pedido.";
            }

            var erroMsg = await response.Content.ReadAsStringAsync();
        return $"A API de negócio retornou erro: {response.StatusCode} - {erroMsg}";}
        
        catch (Exception ex)
        {
            return $"Falha no Registro da visita {ex.Message}";
        }
    }
}