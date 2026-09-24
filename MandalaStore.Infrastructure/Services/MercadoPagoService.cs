using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.Extensions.Configuration;


namespace MandalaStore.Infrastructure.Services
{
    
    public class MercadoPagoService
    {
        public MercadoPagoService(
            IConfiguration configuration)
        {
            MercadoPagoConfig.AccessToken =
                configuration["MercadoPago:AccessToken"];
        }

        public async Task<Payment> CriarPix(decimal valor,
                                            string email,
                                            string nome)
            
        {
            var client =
          new PaymentClient();

            var request =
                new PaymentCreateRequest
                {
                    TransactionAmount = valor,

                    Description =
                        "Pedido Mandala Store",

                    PaymentMethodId =
                        "pix",

                    Payer =
                        new PaymentPayerRequest
                        {
                            Email = email,

                            FirstName = nome
                        }
                };

            var requestOptions =
                new RequestOptions
                {
                    AccessToken = MercadoPagoConfig.AccessToken
                };

            return await client.CreateAsync(
                request,
                requestOptions
            );
        }
    }
}
