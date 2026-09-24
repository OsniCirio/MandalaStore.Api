using MandalaStore.Infrastructure.Services;
using MandalaStore.Infrastructure.Settings;
using MandalaStore.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MandalaStore.Infrastructure.DTO.Request;
using MandalaStore.Infrastructure.DTO;
using MandalaStore.Application.DTO.Response;
using System.Net.Http.Json;



namespace MandalaStore.Infrastructure.Services;

public class MelhorEnvioService : IMelhorEnvioService
{
    private readonly HttpClient _httpClient;
    private readonly MelhorEnvioSettings _settings;

    public MelhorEnvioService(
        HttpClient httpClient,
        IOptions<MelhorEnvioSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;

        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.Token);

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_settings.UserAgent);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<ShippingOptionDto>> CalculateAsync(CalculateShippingRequest request)
    {
        var payload = new
        {
            from = new
            {
                postal_code = _settings.CepOrigem
            },
            to = new
            {
                postal_code = request.CepDestino
            },
            products = request.Products.Select(p => new
            {
                id = p.Id,
                width = p.Width,
                height = p.Height,
                length = 1,
                weight = p.Weight,
                insurance_value = p.InsuranceValue,
                quantity = p.Quantity
            }).ToList()
        };

        var json = JsonSerializer.Serialize(payload);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/v2/me/shipment/calculate", content);

        var responseJson = await response.Content.ReadAsStringAsync();

        //Console.WriteLine(responseJson);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao calcular frete Melhor Envio: {responseJson}");
        }

        var melhorEnvioResponse = JsonSerializer.Deserialize<List<MelhorEnvioShippingResponse>>(
            responseJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (melhorEnvioResponse == null)
            return new List<ShippingOptionDto>();

        return melhorEnvioResponse.Where(x => string.IsNullOrEmpty(x.Error))
            .Select(x => new ShippingOptionDto
            {
                Id = x.Id,
                Carrier = x.Company?.Name ?? "",
                Service = x.Name ?? "",
                Price = decimal.TryParse(x.Price.Replace(".", ","), out var price) ? price : 0,
                DeliveryDays = x.DeliveryTime ?? 0
            })
            .ToList();
    }

    public async Task<string> AdicionarAoCarrinhoAsync(
     MelhorEnvioCartRequest request)
    {
        request.From = CriarRemetente();

        var response = await _httpClient.PostAsJsonAsync(
            "/api/v2/me/cart",
            request);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Melhor Envio ({(int)response.StatusCode}): {json}");
        }

        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("id", out var idElement))
        {
            throw new Exception(
                $"Melhor Envio não retornou o ID do envio. Resposta: {json}");
        }

        var melhorEnvioId = idElement.GetString();

        if (string.IsNullOrWhiteSpace(melhorEnvioId))
        {
            throw new Exception(
                "Melhor Envio retornou um ID inválido.");
        }

        return melhorEnvioId;
    }
    public async Task GerarEtiquetaAsync(string melhorEnvioId)
    {
        var payload = new
        {
            orders = new[] { melhorEnvioId }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/api/v2/me/shipment/generate",
            payload);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"Erro ao gerar etiqueta Melhor Envio ({(int)response.StatusCode}): {json}");
    }
    public async Task CheckoutAsync(string melhorEnvioId)
    {
        var payload = new
        {
            orders = new[] { melhorEnvioId }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/api/v2/me/shipment/checkout",
            payload);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"Erro no checkout Melhor Envio ({(int)response.StatusCode}): {json}");
    }
    public async Task<string> ImprimirEtiquetaAsync(
    string melhorEnvioId)
    {
        var payload = new
        {
            mode = "public",
            orders = new[] { melhorEnvioId }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/api/v2/me/shipment/print",
            payload);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"Erro ao imprimir etiqueta Melhor Envio " +
                $"({(int)response.StatusCode}): {json}");

        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("url", out var urlElement))
            throw new Exception(
                $"Melhor Envio não retornou a URL da etiqueta. Resposta: {json}");

        var url = urlElement.GetString();

        if (string.IsNullOrWhiteSpace(url))
            throw new Exception(
                "Melhor Envio retornou uma URL de etiqueta inválida.");

        return url;
    }
    private  MelhorEnvioPessoa CriarRemetente()
    {
        return new MelhorEnvioPessoa
        {
            Name = _settings.Application,
            Document = _settings.Remetente.Documento,
            Phone = _settings.Remetente.Telefone,
            Email = _settings.Remetente.Email,

            PostalCode = _settings.CepOrigem,
            Address = _settings.Remetente.Endereco,
            Number = _settings.Remetente.Numero,
            Complement = _settings.Remetente.Complemento,
            District = _settings.Remetente.Bairro,
            City = _settings.Remetente.Cidade,
            StateAbbr = _settings.Remetente.Uf,
            CountryId = "BR"
        };
    }
}