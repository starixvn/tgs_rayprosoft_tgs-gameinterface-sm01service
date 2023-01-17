using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.SM01Service.Core.ApplicationServices;
using Tgs.GameInterface.SM01Service.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.Controllers;

[ApiController]
[Route("sm01")]
public class SM01Controller : ControllerBase
{
    /// <summary>
    /// Funds the in asynchronous.(GP -> Tgs)
    /// </summary>
    /// <param name="service"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "customerName": "4ucmqplxb029",
    ///             "refTransactionCode": "8a1ff03c-8101-0000-0001-00009da1fea9",
    ///             "amount": 10
    ///         }
    /// </remarks>
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

    /// <summary>
    /// Funds the out asynchronous.(Tgs -> GP)
    /// </summary>
    /// <param name="service"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "customerName": "4ucmqplxb029",
    ///             "refTransactionCode": "8a1ff03c-8101-0000-0001-00009da1fea9",
    ///             "amount": 10
    ///         }
    /// </remarks>
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

    /// <summary>
    /// 獲取Fund In或Fund Out歷史記錄
    /// </summary>
    /// <param name="service"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    /// isFundOut:true (Tgs -> GP)<br />
    /// isFundOut:false(GP ->Tgs)<br />
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "customerName": "4ucmqplxb029",
    ///             "isFundOut": true,
    ///             "startDate": "2022-12-01T03:15:43.434Z",
    ///             "endDate": "2022-12-13T03:15:43.434Z"
    ///         }
    /// </remarks>
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

    /// <summary>
    /// 檢查玩家，玩家存在則返回玩家訊息，不存在則創建玩家
    /// </summary>
    /// <param name="service"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "customerName": "4ucmqplxb029"
    ///         }
    /// </remarks>

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

    /// <summary>
    /// 取玩家餘額
    /// </summary>
    /// <param name="service"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "customerName": "4ucmqplxb029"
    ///         }
    /// </remarks>
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

    /// <summary>
    /// 取得玩家登入需要的資訊
    /// </summary>
    /// <param name="service"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "customerName": "4ucmqplxb029"
    ///         }
    /// </remarks>

    [HttpPost("login")]
    public async ValueTask<PlatformLoginResultViewModel> PlatformLoginAsync(
        [FromServices] IGetService<PlatformLoginQuery, PlatformLoginResult> service,
        PlatformLoginViewModel source)
    {
        var query = new PlatformLoginQuery(
            source.ApiUrl,
            source.MerchantId,
            source.MerchantKey,
            source.CustomerName);
        var result = await service.GetAsync(query);

        return new PlatformLoginResultViewModel(
            result.MerchantId,
            result.CustomerName,
            result.Timestamp,
            result.Sign);
    }

    /// <summary>
    /// 獲取遊戲結算記錄
    /// </summary>
    /// <param name="service"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "startDate": "2022-12-12T11:30:58.148Z",
    ///             "endDate": "2022-12-12T12:00:58.148Z"
    ///         }
    /// </remarks>
    [HttpPost("game/history/settled", Name = "GameSettledHistory")]
    public async IAsyncEnumerable<GameSettledHistoryResultViewModel> GameSettledHistoryAsync(
        [FromServices] IQueryService<GameSettledHistoryQuery, GameSettledHistoryResult> service,
        GameSettledHistoryViewModel model)
    {
        var query = new GameSettledHistoryQuery(
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
            yield return new GameSettledHistoryResultViewModel(
                PlayerCode: gameRecord.PlayerCode,
                ExternalRoundId: gameRecord.ExternalRoundId,
                ExternalCreatedOn: gameRecord.ExternalCreatedOn,
                ExternalClosedOn: gameRecord.ExternalClosedOn,
                Bet: gameRecord.Bet,
                ValidBet: gameRecord.ValidBet,
                PlayerWinLoss: gameRecord.PlayerWinLoss,
                Turnover: gameRecord.Turnover,
                Settle: gameRecord.Settle,
                AdditionalData: gameRecord.AdditionalData,
                IsCancelled: gameRecord.IsCancelled);
        }
    }

    /// <summary>
    /// Games the unsettle history asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://cn.ucfclive.com",
    ///             "merchantId": "SQjCxm2cX5bahBta",
    ///             "merchantKey": "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
    ///             "startDate": "2022-12-12T11:30:58.148Z",
    ///             "endDate": "2022-12-12T12:00:58.148Z"
    ///         }
    /// </remarks>
    [HttpPost("game/history/unsettle", Name = "GameUnsettleHistory")]
    public async IAsyncEnumerable<GameUnsettleHistoryResultViewModel> GameUnsettleHistoryAsync(
        [FromServices] IQueryService<GameUnsettleHistoryQuery, GameUnsettleHistoryResult> service,
        GameUnsettleHistoryViewModel model)
    {
        var query = new GameUnsettleHistoryQuery(
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
            yield return new GameUnsettleHistoryResultViewModel(
                PlayerCode: gameRecord.PlayerCode,
                ExternalRoundId: gameRecord.ExternalRoundId,
                ExternalCreatedOn: gameRecord.ExternalCreatedOn,
                Bet: gameRecord.Bet,
                ValidBet: gameRecord.ValidBet,
                AdditionalData: gameRecord.AdditionalData);
        }
    }
}
