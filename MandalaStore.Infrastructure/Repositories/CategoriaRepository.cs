using MandalaStore.Domain.Dtos;
using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Context;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Infrastructure.Repositories
{
    public class CategoriaRepository
    {
        private readonly IMongoCollection<Categoria> _collection;
        private readonly IMongoCollection<Produto> _produtos;
        public CategoriaRepository(MongoContext context)
        {
            _collection = context.Categorias;
            _produtos = context.Produtos;
        }
        public async Task<List<Categoria>> ObterNomeAsync(string nome)
        {
            return await _collection
                .Find(x => x.Categoria_Nome == nome)
                .ToListAsync();
        }
        public async Task<List<Categoria>> GetAllAsync()
             => await _collection.Find(_ => true).ToListAsync();

        public async Task<Categoria> GetByIdAsync(string id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Categoria categoria)
        {
            try
            {
                await _collection.InsertOneAsync(categoria);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task UpdateAsync(string id, Categoria categoria)
        {   
            categoria.Id = id;
            await _collection.ReplaceOneAsync(x => x.Id == id, categoria);
        }
        public async Task DeleteAsync(string id)
            => await _collection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<CategoriaListDto>> ObterListaAsync()
        {
            var resultado = await _collection.Aggregate()
                .Lookup<Categoria, Produto, CategoriaComProdutos>(
                    _produtos,
                    c => c.Id,
                    p => p.CategoriaId,
                    x => x.Produtos)
                .Project(x => new CategoriaListDto
                {
                    Id = x.Id,
                    Categoria_Nome = x.Categoria_Nome,
                    QuantidadeProdutos = x.Produtos.Count
                })
                .SortBy(x => x.Categoria_Nome)
                .ToListAsync();

            return resultado;
        }
    }
}
