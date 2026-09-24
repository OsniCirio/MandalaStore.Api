using MandalaStore.API.Helpers;
using MandalaStore.Application.DTO;
using MandalaStore.Application.Services;
using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Helpers;
using MandalaStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace MandalaStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
  
        private readonly CategoriaRepository _repo;
        private readonly IWebHostEnvironment _env;

        public CategoriaController(CategoriaRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }
        [HttpGet("CategoriaProduto")]
        public async Task<IActionResult> GetAgreegate()
        {

            return Ok(await _repo.ObterListaAsync());
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
                return Ok(new Categoria());
            }
            var categoria =
                await _repo.GetByIdAsync(id);

            if (categoria == null)
            {
                return Ok(new Produto());
            }
            return Ok(categoria);

        }
        [HttpGet("nome")]
        public async Task<IActionResult> GetNome(string nome)
        {
            var categorias =
                await _repo.ObterNomeAsync(nome);

            return Ok(categorias);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Categoria dto)
        {
            var categoria = new Categoria
            {
                Categoria_Nome = dto.Categoria_Nome,
            };

            await _repo.CreateAsync(categoria);

            return Ok(new
            {
                success = true,
                Message = "Categoria cadastrada com sucesso",
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] Categoria dto)
        {
            try
            {


                var product =
                    await _repo.GetByIdAsync(id);

                if (product == null)
                    return NotFound();

                var categoria = new Categoria
                {
                    Categoria_Nome = dto.Categoria_Nome,
                };


                await _repo.UpdateAsync(
                    id,
                    categoria);

                return Ok(new
                {
                    success = true,
                    Message = "Categoria atualizado com sucesso",
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

