using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace MandalaStore.Domain.Entities
{

    public class Produto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Nome { get; set; } = null!;

        public string Descricao { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoriaId { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string TemaId { get; set; } = null!;
        public decimal Preco { get; set; }

        public int Estoque { get; set; }

        public decimal Peso { get; set; }

        public bool Ativo { get; set; }

        public bool Destaque { get; set; }


        public decimal Altura { get; set; }

        public decimal Largura { get; set; }

        public decimal Comprimento { get; set; }

        public string? ImagemUrl { get; set; }
        public string? ImageHash { get; set; }

        public DateTime DataCriacao { get; set; }
            = DateTime.UtcNow;
    }
    public class ProdutoComRelacionamentos : Produto
    {
        public List<Categoria> Categorias { get; set; } = [];

        public List<Tema> Temas { get; set; } = [];
    }
}
