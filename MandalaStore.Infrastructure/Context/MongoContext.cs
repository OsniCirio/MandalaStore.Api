using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MandalaStore.Infrastructure.Context
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Produto> Produtos =>
            _database.GetCollection<Produto>("Produtos");

        public IMongoCollection<Pedido> Pedidos =>
            _database.GetCollection<Pedido>("Pedidos");

        public IMongoCollection<Categoria> Categorias =>
            _database.GetCollection<Categoria>("Categorias");

        public IMongoCollection<Tema> Temas =>
         _database.GetCollection<Tema>("Temas");
    }

}
