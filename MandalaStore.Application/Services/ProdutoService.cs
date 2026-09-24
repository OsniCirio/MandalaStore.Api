using MandalaStore.API.Helpers;
using MandalaStore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Application.Services
{

    public class ProdutoService
    {
        private readonly ProdutoRepository _repository;



        public ProdutoService(ProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidarImagemAsync(
        string hash)
        {
            var produtos =
                await _repository.GetAllAsync();

            foreach (var item in produtos)
            {
                if (string.IsNullOrEmpty(item.ImageHash))
                    continue;

                var distance =
                    ImageHashHelper.HammingDistance(
                        hash,
                        item.ImageHash);

                if (distance < 10)
                {
                    throw new Exception(
                        "Imagem muito parecida");
                }
            }
        }
    }
}
