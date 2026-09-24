using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using MandalaStore.Application.DTO;
using MandalaStore.Infrastructure.Helpers;
using MongoDB.Bson;
using MandalaStore.API.Helpers;
using System.IO;
using MandalaStore.Application.Services;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MandalaStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoRepository _repo;
        private readonly ProdutoService _produtoService;

        private readonly IWebHostEnvironment _env;

        public ProdutoController(ProdutoRepository repo, 
                                 ProdutoService produtoService, 
                                 IWebHostEnvironment env)
        {
            _repo = repo;
            _produtoService = produtoService;
            _env = env;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> Get()
        {
           
            return Ok(await _repo.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return Ok(new Produto());
            }
            var produto =
                await _repo.GetByIdAsync(id);

            if (produto == null)
            {
                return Ok(new Produto());
            }
            return Ok(produto);

        }
        [HttpGet("detalhe/{id}")]
        public async Task<IActionResult> Getdetalhe(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return Ok(new Produto());
            }
            var produto =
                await _repo.ObterDetalhePorId(id);

            if (produto == null)
            {
                return Ok(new ProdutoDetalheDto());
            }
            return Ok(produto);

        }
        [HttpGet("destaques")]
        public async Task<IActionResult> Destaques()
        {
            var produtos =
                await _repo.ObterDestaquesAsync();

            return Ok(produtos);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProdutoCriacaoDto dto)
        {
            var imageUrl =
                await UploadHelper
                    .UploadImagemAsync(
                        dto.Imagem,
                        _env);


            var ImagemFisicaPath =
                    Path.Combine(
                        _env.WebRootPath ?? "wwwroot",
                        imageUrl.TrimStart('/'));

            var hash = ImageHashHelper.GenerateHash(ImagemFisicaPath);

            var product = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CategoriaId = dto.CategoriaId,
                Preco = decimal.Parse(dto.Preco),
                Estoque = dto.Estoque,
                Peso = decimal.Parse(dto.Peso),
                Ativo = dto.Ativo,
                Largura = decimal.Parse(dto.Largura.Replace(".", ",")),
                Altura = decimal.Parse(dto.Altura.Replace(".", ",")),
                ImagemUrl = imageUrl,
                ImageHash = hash
            };

            await _repo.CreateAsync(product);

            return Ok(new
            {
                success = true,
                Message = "Mandala cadastrada com sucesso",
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] ProdutoCriacaoDto dto )
        {
            try
            {
              

                var product =
                    await _repo.GetByIdAsync(id);

                if (product == null)
                    return NotFound();

                var produto = new Produto
                {
                    Nome = dto.Nome,
                    Descricao = dto.Descricao,
                    CategoriaId = dto.CategoriaId,
                    TemaId = dto.TemaId,
                    Preco = decimal.Parse(dto.Preco.Replace(".", ",")),
                    Estoque = dto.Estoque,
                    Peso = decimal.Parse(dto.Peso.Replace(".",",")),
                    Largura = decimal.Parse(dto.Largura.Replace(".", ",")),
                    Altura = decimal.Parse(dto.Altura.Replace(".", ",")),
                    ImagemUrl = product.ImagemUrl,
                    Id =id,
                    Ativo = dto.Ativo,
                };

                if (dto.Imagem != null)
                {
                    if (dto.Imagem != null)
                    {
                        var imageUrl =
                            await UploadHelper
                                .UploadImagemAsync(
                                    dto.Imagem,
                                    _env);

                        produto.ImagemUrl = imageUrl;

                        var ImagemFisicaPath =
                            Path.Combine(
                                _env.WebRootPath ?? "wwwroot",
                                imageUrl.TrimStart('/'));

                        var hash = ImageHashHelper.GenerateHash(ImagemFisicaPath);

                        await _produtoService.ValidarImagemAsync(hash);


                    }

                }
                await _repo.UpdateAsync(
                    id,
                    produto);

                return Ok(new
                {
                    success = true,
                    Message = "Produto atualizado com sucesso",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repo.DeleteAsync(id);
            return Ok();
        }
    }
}
