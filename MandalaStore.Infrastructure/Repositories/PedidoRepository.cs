using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Infrastructure.Repositories
{
    using MandalaStore.Domain.Entities;
    using MandalaStore.Infrastructure.Context;
    using MongoDB.Driver;

    public class PedidoRepository
    {
        private readonly IMongoCollection<Pedido> _collection;

        public PedidoRepository(MongoContext context)
        {
            _collection = context.Pedidos;
        }

        public async Task<List<Pedido>> GetAllAsync()
        {
            return await _collection
                .Find(_ => true)
                .SortByDescending(x => x.DataPedido)
                .ToListAsync();
        }
        public async Task UpdateMelhorEnvioIdAsync(
           string pedidoId,
           string melhorEnvioId)
        {
            var update = Builders<Pedido>
                .Update
                .Set(x => x.MelhorEnvioId, melhorEnvioId);
           

            await _collection.UpdateOneAsync(
                x => x.Id == pedidoId,
                update);
        }
        public async Task UpdateFreteCompradoAsync(string pedidoId)
{
    var update = Builders<Pedido>.Update
        .Set(x => x.FreteCompradoEm, DateTime.UtcNow);

    await _collection.UpdateOneAsync(
        x => x.Id == pedidoId,
        update);
}
        public async Task<Pedido?> GetByIdAsync(string id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
        public async Task UpdateEtiquetaAsync(string pedidoId,string url)
        {
            var update = Builders<Pedido>.Update
                .Set(x => x.EtiquetaUrl, url)
                .Set(x => x.EtiquetaGeradaEm, DateTime.UtcNow)
                .Set(x => x.Status, "EtiquetaGerada");

            await _collection.UpdateOneAsync(
                x => x.Id == pedidoId,
                update);
        }
        public async Task CreateAsync(Pedido pedido)
        {
            pedido.DataPedido = DateTime.UtcNow;
            pedido.Status = string.IsNullOrWhiteSpace(pedido.Status)
                ? "Pendente"
                : pedido.Status;

            await _collection.InsertOneAsync(pedido);
        }
        public async Task UpdatePaymentIdAsync(
              string pedidoId,
              long paymentId)
        {
            var update = Builders<Pedido>
                .Update
                .Set(x => x.PaymentId, paymentId);

            await _collection.UpdateOneAsync(
                x => x.Id == pedidoId,
                update);
        }
        public async Task UpdateStatusAsync(string id,   string status)
        {
            var update =
                Builders<Pedido>.Update
                    .Set(x => x.Status, status);

            await _collection.UpdateOneAsync(
                x => x.Id == id,
                update);
        }
     
    }
}
