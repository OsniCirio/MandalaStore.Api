using MandalaStore.Infrastructure.DTO.Request;
using MandalaStore.Infrastructure.DTO;

namespace MandalaStore.Infrastructure.Interfaces;

public interface IMelhorEnvioService
{
    Task<string> AdicionarAoCarrinhoAsync(MelhorEnvioCartRequest request);
    Task<List<ShippingOptionDto>> CalculateAsync(CalculateShippingRequest request);
    Task CheckoutAsync(string melhorEnvioId);
    Task GerarEtiquetaAsync(string melhorEnvioId);
    Task<string> ImprimirEtiquetaAsync(string melhorEnvioId);
}