using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Context;
using MongoDB.Driver;


namespace MandalaStore.Infrastructure.Repositories
{

    public class ProdutoRepository
    {
        private readonly IMongoCollection<Produto> _collection;
        private readonly IMongoCollection<Categoria> _categorias;
        private readonly IMongoCollection<Tema> _temas;

        public ProdutoRepository(MongoContext context)
        {
            _collection = context.Produtos;
            _categorias = context.Categorias;
            _temas = context.Temas;
        }
        public async Task<List<Produto>> ObterDestaquesAsync()
        {
            return await _collection
                .Find(x => x.Ativo)
                .Limit(8)
                .ToListAsync();
        }
        public async Task<List<Produto>> GetAllAsync()
            => await _collection.Find(_ => true).ToListAsync();

        public async Task<Produto> GetByIdAsync(string id)
            => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Produto produto)
        {
            try
            {
                await _collection.InsertOneAsync(produto);
            }catch(Exception ex)
            {

            }
        }

        public async Task UpdateAsync(string id, Produto produto)
            => await _collection.ReplaceOneAsync(x => x.Id == id, produto);

        public async Task DeleteAsync(string id)
            => await _collection.DeleteOneAsync(x => x.Id == id);
        public async Task<List<Produto>> GetByIdsAsync(IEnumerable<string> ids)
        {
            var filter =
                Builders<Produto>.Filter.In(
                    x => x.Id,
                    ids);

            return await _collection
                .Find(filter)
                .ToListAsync();
        }
        public async Task<ProdutoDetalheDto?> ObterDetalhePorId(string produtoId)
        {
            var resultado = await _collection
                .Aggregate()

                // WHERE Produto.Id = produtoId
                .Match(p => p.Id == produtoId)

                // JOIN Categoria
                .Lookup<Produto, Categoria, ProdutoComRelacionamentos>(
                    _categorias,
                    p => p.CategoriaId,
                    c => c.Id,
                    x => x.Categorias)

                // JOIN Tema
                .Lookup<ProdutoComRelacionamentos, Tema, ProdutoComRelacionamentos>(
                    _temas,
                    p => p.TemaId,
                    t => t.Id,
                    x => x.Temas)

                .Project(x => new ProdutoDetalheDto
                {
                    Id = x.Id,
                    Nome = x.Nome,
                    Descricao = x.Descricao,
                    Preco = x.Preco,

                    CategoriaId = x.CategoriaId,

                    Categoria = x.Categorias
                        .Select(c => c.Categoria_Nome)
                        .FirstOrDefault(),

                    TemaId = x.TemaId,
                    Altura = x.Altura, 
                    Largura = x.Largura,
                    ImagemUrl = x.ImagemUrl ?? string.Empty,
                    Tema = x.Temas
                        .Select(t => t.Tema_Nome)
                        .FirstOrDefault()
                })

                .FirstOrDefaultAsync();

            return resultado;
        }

        
    }
}
