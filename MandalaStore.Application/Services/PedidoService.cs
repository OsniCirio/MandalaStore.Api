using MandalaStore.Domain.Entities;
using MandalaStore.Infrastructure.Interfaces;
using MandalaStore.Infrastructure.Repositories;
using MandalaStore.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Routing;
using MongoDB.Driver;
using OpenCvSharp;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MandalaStore.Application.Services;
public class PedidoService
{
    private readonly PedidoRepository _pedidoRepository;
    private readonly ProdutoRepository _produtoRepository;
    private readonly IMelhorEnvioService _melhorEnvioService;

    public PedidoService(
        PedidoRepository pedidoRepository,
        ProdutoRepository produtoRepository,
        IMelhorEnvioService melhorEnvioService)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _melhorEnvioService = melhorEnvioService;
    }
    public async Task ProcessarEtiquetaAsync(string pedidoId)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(pedidoId)
            ?? throw new Exception("Pedido não encontrado.");

        if (pedido.Status != "Pago")
            throw new Exception(
                "A etiqueta só pode ser gerada para um pedido pago.");

        // 1. Cria o envio no Melhor Envio
        if (string.IsNullOrWhiteSpace(pedido.MelhorEnvioId))
        {
            await CriarEnvioAsync(pedidoId);

            // CriarEnvioAsync já grava MelhorEnvioId
            pedido = await _pedidoRepository.GetByIdAsync(pedidoId)
                ?? throw new Exception("Pedido não encontrado.");
        }

        // 2. Compra o frete
        await ComprarEnvioAsync(pedidoId);

        // 3. Solicita geração da etiqueta
        await GerarEtiquetaAsync(pedidoId);

        // 4. Atualiza nosso pedido
        await _pedidoRepository.UpdateStatusAsync(
            pedidoId,
            "EtiquetaGerada");
    }
    public async Task<string> CriarEnvioAsync(string pedidoId)
    {
        var pedido =
            await _pedidoRepository.GetByIdAsync(pedidoId)
            ?? throw new Exception("Pedido não encontrado.");

      
        if (!string.IsNullOrWhiteSpace(pedido.MelhorEnvioId))
            return pedido.MelhorEnvioId;

        if (pedido.FreteServicoId is null)
            throw new Exception(
                "Pedido não possui serviço de frete.");

        var produtoIds = pedido.Itens
            .Select(x => x.Id)
            .Distinct()
            .ToList();

        var produtos =
            await _produtoRepository.GetByIdsAsync(produtoIds);

        if (produtos.Count != produtoIds.Count)
            throw new Exception(
                "Um ou mais produtos não foram encontrados.");

        // ============================
        // DESTINATÁRIO
        // ============================

        var destinatario = new MelhorEnvioPessoa
        {
            Name = pedido.NomeCliente,
            Document = pedido.Documento,
            Phone = pedido.Telefone,
            Email = pedido.Email,

            PostalCode = pedido.Cep,
            Address = pedido.Endereco,
            Number = pedido.Numero,
            Complement = pedido.Complemento,
            District = pedido.Bairro,
            City = pedido.Cidade,
            StateAbbr = pedido.Uf,
            CountryId = "BR"
        };

        // ============================
        // PRODUTOS / DECLARAÇÃO
        // ============================

        var itens = pedido.Itens
            .Select(item => new MelhorEnvioProduto
            {
                Name = item.Nome,
                Quantity = item.Quantidade,
                UnitaryValue = item.Valor
            })
            .ToList();

        // ============================
        // VOLUMES
        // ============================

        var volumes = pedido.Itens
            .Select(item =>
            {
                var produto = produtos
                    .First(p => p.Id == item.Id);

                return new MelhorEnvioVolume
                {
                    Height = produto.Altura,
                    Width = produto.Largura,
                    Length = 1,

                    // Peso total desse item
                    Weight = produto.Peso * item.Quantidade
                };
            })
            .ToList();

        // ============================
        // PAYLOAD
        // ============================

        var request = new MelhorEnvioCartRequest
        {
            Service = pedido.FreteServicoId.Value,

            To = destinatario,

            Products = itens,

            Volumes = volumes,

            Options = new MelhorEnvioOptions
            {
                InsuranceValue = pedido.Subtotal,
                Receipt = false,
                OwnHand = false,
                Reverse = false,
                NonCommercial = true
            }
        };

        var melhorEnvioId =
            await _melhorEnvioService
                .AdicionarAoCarrinhoAsync(request);

        await _pedidoRepository.UpdateMelhorEnvioIdAsync(
            pedido.Id!,
            melhorEnvioId);

        

        return melhorEnvioId;

    
    }
   
    public async Task ComprarEnvioAsync(string pedidoId)
    {
        var pedido =
            await _pedidoRepository.GetByIdAsync(pedidoId)
            ?? throw new Exception("Pedido não encontrado.");

        if (string.IsNullOrWhiteSpace(pedido.MelhorEnvioId))
            throw new Exception(
                "O pedido ainda não possui envio no Melhor Envio.");

        // Já foi comprado
        if (pedido.FreteCompradoEm.HasValue)
            return;

        await _melhorEnvioService.CheckoutAsync(
            pedido.MelhorEnvioId);

        await _pedidoRepository.UpdateFreteCompradoAsync(
            pedidoId);
    }
    

    public async Task GerarEtiquetaAsync(string pedidoId)
    {
        var pedido =
            await _pedidoRepository.GetByIdAsync(pedidoId)
            ?? throw new Exception("Pedido não encontrado.");

        if (string.IsNullOrWhiteSpace(pedido.MelhorEnvioId))
            throw new Exception(
                "Pedido não possui ID do Melhor Envio.");

        await _melhorEnvioService.GerarEtiquetaAsync(
            pedido.MelhorEnvioId);

    }
    public async Task GerarEtiquetaCompletaAsync(string pedidoId)
    {
        var pedido =
            await _pedidoRepository.GetByIdAsync(pedidoId)
            ?? throw new Exception("Pedido não encontrado.");

        // Só processamos expedição depois do pagamento
        if (pedido.Status != "Pago")
            throw new Exception(
                "A etiqueta só pode ser gerada para um pedido pago.");

        // ==========================================
        // 1 - CRIAR ENVIO NO MELHOR ENVIO
        // ==========================================

        await CriarEnvioAsync(pedidoId);


        // ==========================================
        // 2 - COMPRAR FRETE
        // ==========================================

        await ComprarEnvioAsync(pedidoId);


        // ==========================================
        // 3 - GERAR ETIQUETA
        // ==========================================

        await GerarEtiquetaAsync(pedidoId);


        // ==========================================
        // 4 - ATUALIZAR NOSSO PEDIDO
        // ==========================================

        await _pedidoRepository.UpdateStatusAsync(
            pedidoId,
            "EtiquetaGerada");
    }
    public async Task<string> ImprimirEtiquetaAsync(string pedidoId)
    {
        var pedido =
            await _pedidoRepository.GetByIdAsync(pedidoId)
            ?? throw new Exception("Pedido não encontrado.");

        if (string.IsNullOrWhiteSpace(pedido.MelhorEnvioId))
            throw new Exception(
                "Pedido não possui ID do Melhor Envio.");

        var url =
            await _melhorEnvioService.ImprimirEtiquetaAsync(
                pedido.MelhorEnvioId);

        await _pedidoRepository.UpdateEtiquetaAsync(
            pedido.Id!,
            url);

        return url;
    }
}
