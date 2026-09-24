using MandalaStore.Application.DTO;
using MandalaStore.Application.Interfaces;
using MandalaStore.Application.Services;
using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MandalaStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreteController : ControllerBase
    {
        private readonly IFreteService _freteService;
        private readonly PedidoService _pedidoService;
        private readonly PedidoRepository _pedidoRepository;

        public FreteController(IFreteService freteService, PedidoService pedidoService, PedidoRepository pedidoRepository)
        {
            _freteService = freteService;
            _pedidoService = pedidoService;
            _pedidoRepository = pedidoRepository;
        }

        [HttpPost("calcular")]
        public async Task<IActionResult>
            Calcular(
                
            CalcularFreteDto dto)
        {

            var resultado =
                await _freteService
                    .CalcularAsync(dto);

            return Ok(resultado);
        }
        [HttpPost("{id}/checkout-frete")]
        public async Task<IActionResult> CheckoutFrete(string id)
        {
            try
            {
                await _pedidoService.ComprarEnvioAsync(id);


                return Ok(new
                {
                    mensagem = "Frete comprado com sucesso."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }
        [HttpPost("{id}/gerar-etiqueta")]
        public async Task<IActionResult> GerarEtiqueta(string id)
        {
            try
            {
                await _pedidoService.GerarEtiquetaCompletaAsync(id);

                return Ok(new
                {
                    mensagem = "Etiqueta enviada e gerada."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }
        [HttpPost("{id}/imprimir-etiqueta")]
        public async Task<IActionResult> ImprimirEtiqueta(string id)
        {
            try
            {
                var url =
                    await _pedidoService.ImprimirEtiquetaAsync(id);

                return Ok(new
                {
                    mensagem = "Etiqueta pronta para impressão.",
                    url
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = ex.Message
                });
            }
        }
    }
}
