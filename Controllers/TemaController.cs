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
    public class TemaController : ControllerBase
    {
  
        private readonly TemaRepository _repo;
        private readonly IWebHostEnvironment _env;

        public TemaController(TemaRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }
        [HttpGet("TemaProduto")]
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
                return Ok(new Tema());
            }
            var tema =
                await _repo.GetByIdAsync(id);

            if (tema == null)
            {
                return Ok(new Produto());
            }
            return Ok(tema);

        }
        [HttpGet("nome")]
        public async Task<IActionResult> GetNome(string nome)
        {
            var temas =
                await _repo.ObterNomeAsync(nome);

            return Ok(temas);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Tema dto)
        {
            var tema = new Tema
            {
                Tema_Nome = dto.Tema_Nome,
                Ordem = dto.Ordem
            };

            await _repo.CreateAsync(tema);

            return Ok(new
            {
                success = true,
                Message = "Tema cadastrada com sucesso",
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] Tema dto)
        {
            try
            {


                var product =
                    await _repo.GetByIdAsync(id);

                if (product == null)
                    return NotFound();

                var categoria = new Tema
                {
                    Tema_Nome = dto.Tema_Nome,
                    Ordem = dto.Ordem
                };


                await _repo.UpdateAsync(
                    id,
                    categoria);

                return Ok(new
                {
                    success = true,
                    Message = "Tema atualizado com sucesso",
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

