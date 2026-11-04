using MandalaStore.Application.DTO;
using MandalaStore.Application.Services;
using MandalaStore.Infrastructure.Repositories;
using MandalaStore.Infrastructure.Services;
using MercadoPago.Client.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MandalaStore.Api.Controllers
{

     
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentoController : ControllerBase
    {
        protected readonly MercadoPagoService _service;
        protected readonly PedidoRepository  _pedidoRepository;
        protected readonly PedidoService _pedidoService;


        public PagamentoController(MercadoPagoService service, PedidoRepository pedidoRepository, PedidoService pedidoService)
        {
            _service = service;
            _pedidoRepository = pedidoRepository;
            _pedidoService = pedidoService;
        }

        [HttpPost("pix/{id}")]
        public async Task<IActionResult> Pix(string id)
        {
        
                var pedido =
                    await _pedidoRepository.GetByIdAsync(id);

                if (pedido == null)
                    return NotFound("Pedido não encontrado.");

                var pagamento =
                    await _service.CriarPix(
                        pedido.Total,
                        pedido.Email,
                        pedido.NomeCliente
                    );

                await _pedidoRepository.UpdatePaymentIdAsync(
                    id!,
                    pagamento.Id!.Value
                );

                return Ok(new
                {
                    qrCode =
                        pagamento.PointOfInteraction
                            .TransactionData
                            .QrCode,

                    qrCodeBase64 =
                        pagamento.PointOfInteraction
                            .TransactionData
                            .QrCodeBase64,

                    paymentId = pagamento.Id
                });
            }
        
        [HttpGet("status/{pedidoId}/{paymentId}")]
        public async Task<IActionResult> Status(string pedidoId,long paymentId)
        {
            var pedido =
                await _pedidoRepository.GetByIdAsync(pedidoId);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            // Segurança: o pagamento consultado precisa
            // pertencer ao pedido informado.
            if (pedido.PaymentId != paymentId)
                return BadRequest(
                    "O pagamento informado não pertence ao pedido.");

            var client = new PaymentClient();

            var payment =
                await client.GetAsync(paymentId);

            if (payment.Status == "approved" &&
                pedido.Status != "Pago")
            {
                await _pedidoRepository.UpdateStatusAsync(
                    pedidoId,
                    "Pago"
                );

                await _pedidoService.EnviarEmailAcompanhamentoAsync(pedido.Id!);
            }

            return Ok(new
            {
                status = payment.Status
            });
        }
    }
}
