using MandalaStore.Application.DTO.Response;
using MandalaStore.Application.DTO;
using MandalaStore.Application.Interfaces;
using MandalaStore.Infrastructure.DTO.Request;
using MandalaStore.Infrastructure.Interfaces;
using MandalaStore.Infrastructure.Dtos;

public class FreteService : IFreteService
{
    private readonly IMelhorEnvioService _melhorEnvioService;

    public FreteService(IMelhorEnvioService melhorEnvioService)
    {
        _melhorEnvioService = melhorEnvioService;
    }

    public async Task<List<FreteResponseDto>> CalcularAsync(
        CalcularFreteDto dto)
    {
        var request = new CalculateShippingRequest
        {
            CepDestino = dto.CepDestino,

            Products = dto.Produtos.Select(p => new ShippingProductDto
            {
                Id = p.Id,
                Width = p.Largura,
                Height = p.Altura,
                Length = p.Comprimento,
                Weight = p.Peso,
                InsuranceValue = p.Valor,
                Quantity = p.Quantidade
            }).ToList()
        };

        var opcoes = await _melhorEnvioService.CalculateAsync(request);

        return opcoes.Select(x => new FreteResponseDto
        {
            Id = x.Id,
            Transportadora = x.Carrier,
            Servico = x.Service,
            Valor = x.Price,
            PrazoDias = x.DeliveryDays
        }).ToList();
    }
}