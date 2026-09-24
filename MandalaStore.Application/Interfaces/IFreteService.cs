using MandalaStore.Application.DTO;
using MandalaStore.Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Application.Interfaces
{
    public interface IFreteService
    {
        Task<List<FreteResponseDto>> CalcularAsync(
            CalcularFreteDto dto);
    }
}
