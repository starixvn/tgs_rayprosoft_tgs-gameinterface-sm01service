using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.SM01Service.Core.ApplicationServices;
using Tgs.GameInterface.SM01Service.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Controllers;

[ApiController]
[Route("sm01")]
public class SM01Controller : ControllerBase
{
    [HttpPost("fund/in", Name = "FundIn")]
    public async ValueTask<FundResultViewModel> FundInAsync(
        [FromServices] IGetService<FundInQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundInQuery(
            ApiUrl: source.ApiUrl,
            MerchantId: source.MerchantId,
            MerchantKey: source.MerchantKey,
            CustomerName: source.CustomerName,
            RefTransactionCode: source.RefTransactionCode,
            Amount: source.Amount);

        var result = await service
                .GetAsync(
                    query: query,
                    cancellationToken: HttpContext.RequestAborted)
                .ConfigureAwait(false);

        return new FundResultViewModel(
            RefTransactionCode: result.RefTransactionCode,
            Balance: result.Balance);
    }

    [HttpPost("fund/out", Name = "FundOut")]
    public async ValueTask<FundResultViewModel> FundOutAsync(
        [FromServices] IGetService<FundOutQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundOutQuery(
            ApiUrl: source.ApiUrl,
            MerchantId: source.MerchantId,
            MerchantKey: source.MerchantKey,
            CustomerName: source.CustomerName,
            RefTransactionCode: source.RefTransactionCode,
            Amount: source.Amount);

        var result = await service
                .GetAsync(
                    query: query,
                    cancellationToken: HttpContext.RequestAborted)
                .ConfigureAwait(false);

        return new FundResultViewModel(
            RefTransactionCode: result.RefTransactionCode,
            Balance: result.Balance);
    }

    [HttpPost("fund/history", Name = "FundHistory")]
    public async IAsyncEnumerable<FundHistoryResultViewModel> FundHistoryAsync(
        [FromServices] IQueryService<FundHistoryQuery, FundHistoryResult> service,
        FundHistoryViewModel model)
    {
        var query = new FundHistoryQuery(
            ApiUrl: model.ApiUrl,
            model.MerchantId,
            model.MerchantKey,
            CustomerName: model.CustomerName,
            IsFundOut: model.IsFundOut,
            StartDate: model.StartDate,
            EndDate: model.EndDate);

        var transferRecords = await service
                .QueryAsync(
                    query: query,
                    cancellationToken: HttpContext.RequestAborted)
                .ToArrayAsync(
                    cancellationToken: HttpContext.RequestAborted)
                .ConfigureAwait(false);

        if (!transferRecords.Any())
        {
            yield break;
        }

        foreach (var transferRecord in transferRecords)
        {
            yield return new FundHistoryResultViewModel(
                TransactionId: transferRecord.TransactionId,
                Amount: transferRecord.Amount,
                CreatedTime: transferRecord.CreatedTime,
                PlayerCode: transferRecord.CustomerName);
        }
    }

    [HttpPost("player/create", Name = "PlayerCreate")]
    public ValueTask PlayerCreateAsync(
        [FromServices] ICommandService<PlayerCreateCommand> service,
        PlayerCreateViewModel source)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: source.ApiUrl,
            MerchantId: source.MerchantId,
            MerchantKey: source.MerchantKey,
            CustomerName: source.CustomerName);

        return service.ExecuteAsync(
                command: command,
                cancellationToken: HttpContext.RequestAborted);
    }

    [HttpPost("player/balance", Name = "PlayerBalance")]
    public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
        [FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
        PlayerBalanceViewModel source)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: source.ApiUrl,
            MerchantId: source.MerchantId,
            MerchantKey: source.MerchantKey,
            CustomerName: source.CustomerName);

        var result = await service
                .GetAsync(
                    query: query,
                    cancellationToken: HttpContext.RequestAborted)
                .ConfigureAwait(false);

        return new PlayerBalanceResultViewModel(
            result.IsPlayerExist,
            result.Balance);
    }

    [HttpPost("login")]
    public async ValueTask<string> PlatformLoginAsync(
        [FromServices] IGetService<PlatformLoginQuery, string> service,
        PlatformLoginViewModel source)
    {
        var query = new PlatformLoginQuery(
            source.ApiUrl,
            source.MerchantId,
            source.MerchantKey,
            source.CustomerName);
        return await service.GetAsync(query);
    }

    [HttpPost("game/history/settled", Name = "GameHistorySettled")]
    public async IAsyncEnumerable<GameHistorySettledResultViewModel> GameHistorySettledAsync(
        [FromServices] IQueryService<GameHistorySettledQuery, GameHistorySettledResult> service,
        GameHistorySettledViewModel model)
    {
        var query = new GameHistorySettledQuery(
            ApiUrl: model.ApiUrl,
            model.MerchantId,
            model.MerchantKey,
            StartDate: model.StartDate,
            EndDate: model.EndDate);

        var gameRecords = await service
                .QueryAsync(
                    query: query,
                    cancellationToken: HttpContext.RequestAborted)
                .ToArrayAsync(
                    cancellationToken: HttpContext.RequestAborted)
                .ConfigureAwait(false);

        if (!gameRecords.Any())
        {
            yield break;
        }

        foreach (var gameRecord in gameRecords)
        {
            yield return new GameHistorySettledResultViewModel(
                PlayerCode: gameRecord.PlayerCode,
                ExternalRoundId: gameRecord.ExternalRoundId,
                IsClosed: gameRecord.IsClosed,
                ExternalCreatedOn: gameRecord.ExternalCreatedOn,
                ExternalClosedOn: gameRecord.ExternalClosedOn,
                Bet: gameRecord.Bet,
                ValidBet: gameRecord.ValidBet,
                PlayerWinLoss: gameRecord.PlayerWinLoss,
                Turnover: gameRecord.Turnover,
                Settle: gameRecord.Settle,
                AdditionalData: gameRecord.AdditionalData);
        }
    }
}
