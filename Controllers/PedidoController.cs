using MandalaStore.Application.Services;
using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Repositories;
using MandalaStore.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MandalaStore.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly PedidoRepository _repository;
        private readonly PedidoService _pedidoService;


        public PedidoController(PedidoRepository repository, PedidoService pedidoService)
        {
            _repository = repository;
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var pedidos = await _repository.GetAllAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var pedido = await _repository.GetByIdAsync(id);

            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }
        [HttpPost("{id}/etiqueta")]
        public async Task<IActionResult> ValidarEtiqueta(string id)
        {
            try
            {
                var melhorEnvioId =
                    await _pedidoService.CriarEnvioAsync(id);

                
                return Ok(new
                {
                    melhorEnvioId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            if (pedido.Itens == null || !pedido.Itens.Any())
                return BadRequest("Pedido sem itens.");

            await _repository.CreateAsync(pedido);

            return CreatedAtAction(
                nameof(GetById),
                new { id = pedido.Id },
                pedido
            );
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            string id,
            [FromBody] string status)
        {
            await _repository.UpdateStatusAsync(id, status);
            return NoContent();
        }
        
    }
}

