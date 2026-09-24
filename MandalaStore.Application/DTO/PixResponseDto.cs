using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandalaStore.Application.DTO
{
    public class PixResponseDto
    {
        public long PaymentId { get; set; }

        public string QrCode { get; set; }

        public string QrCodeBase64 { get; set; }
    }
}
