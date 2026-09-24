using MandalaStore.Domain.Dtos;
using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Infrastructure.Repositories
{
    public class TemaRepository
    {
        private readonly IMongoCollection<Tema> _collection;
        private readonly IMongoCollection<Produto> _produtos;
        private readonly IMongoDatabase _database;



        public TemaRepository(MongoContext context)
        {
            _collection = context.Temas;
            _produtos = context.Produtos;
        }
        public async Task<List<Tema>> ObterNomeAsync(string nome)
        {
            return await _collection
                .Find(x => x.Tema_Nome == nome)
                .ToListAsync();
        }
        public async Task<List<Tema>> GetAllAsync()
             => await _collection.Find(_ => true).ToListAsync();

        public async Task<Tema> GetByIdAsync(string id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Tema tema)
        {
            try
            {
                await _collection.InsertOneAsync(tema);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task UpdateAsync(string id, Tema tema)
        {   
            tema.Id = id;
            await _collection.ReplaceOneAsync(x => x.Id == id, tema);
        }
        public async Task DeleteAsync(string id)
            => await _collection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<TemaListDto>> ObterListaAsync()
        {

            if (!_collection.Find(_ => true).Any()) return new List<TemaListDto>();


             var resultado = await _collection.Aggregate()
                .Lookup<Tema, Produto, TemaComProdutos>(
                    _produtos,
                    c => c.Id,
                    p => p.TemaId,
                    x => x.Produtos)
                .Project(x => new TemaListDto
                {
                    Id = x.Id,
                    Tema_Nome = x.Tema_Nome,
                    QuantidadeProdutos = x.Produtos.Count
                })
                .SortBy(x => x.Tema_Nome)
                .ToListAsync();

            return resultado;
        }
    }
}
