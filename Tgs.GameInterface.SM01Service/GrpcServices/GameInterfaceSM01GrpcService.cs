using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.SM01Service.Core.ApplicationServices;
using Tgs.GameInterface.SM01Service.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.GrpcServices;

internal class GameInterfaceSM01GrpcService(
	ILogger<GameInterfaceSM01GrpcService> logger,
	IGetService<FundInQuery, FundResult> fundInService,
	IGetService<FundOutQuery, FundResult> fundOutService,
	IQueryService<FundHistoryQuery, FundHistoryResult> fundHistoryService,
	ICommandService<PlayerCreateCommand> playerCreateService,
	IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
	IQueryService<GameSettledHistoryQuery, GameSettledHistoryResult> gameSettledHistorySerivce,
	IQueryService<GameUnsettleHistoryQuery, GameUnsettleHistoryResult> gameUnsettleHistorySerivce,
	IGetService<PlatformLoginQuery, PlatformLoginResult> platformLoginService) : GameInterfaceSM01Service.GameInterfaceSM01ServiceBase
{
	public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
	{
		logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

		return Task.FromResult(new CheckingServiceReply
		{
			MachineName = Environment.MachineName
		});
	}

	public override async Task<PlatformLoginReply> PlatformLogin(PlatformLoginRequest request, ServerCallContext context)
	{
		var query = new PlatformLoginQuery(
			ApiUrl: new Uri(request.ApiUrl),
			MerchantId: request.MerchantId,
			MerchantKey: request.MerchantKey,
			CustomerName: request.CustomerName);

		var result = await platformLoginService
			.GetAsync(
				query: query,
				cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new PlatformLoginReply()
		{
			ApiUrl = result.ApiUrl,
			CustomerName = result.CustomerName,
			MerchantId = result.MerchantId,
			Sign = result.Sign,
			Timestamp = result.Timestamp
		};
	}

	public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
	{
		var query = new FundInQuery(
		   ApiUrl: new(request.ApiUrl),
		   MerchantId: request.MerchantId,
		   MerchantKey: request.MerchantKey,
		   CustomerName: request.CustomerName,
		   RefTransactionCode: request.RefTransactionCode,
		   Amount: request.Amount);

		var result = await fundInService
				.GetAsync(
					query: query,
					cancellationToken: context.CancellationToken)
				.ConfigureAwait(false);

		return new FundReply()
		{
			RefTransactionCode = result.RefTransactionCode,
			TransactionId = result.RefTransactionCode,
			Balance = result.Balance
		};
	}

	public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
	{
		var query = new FundOutQuery(
			ApiUrl: new(request.ApiUrl),
			MerchantId: request.MerchantId,
			MerchantKey: request.MerchantKey,
			CustomerName: request.CustomerName,
			RefTransactionCode: request.RefTransactionCode,
			Amount: request.Amount);

		var result = await fundOutService
				.GetAsync(
					query: query,
					cancellationToken: context.CancellationToken)
				.ConfigureAwait(false);

		return new FundReply()
		{
			RefTransactionCode = result.RefTransactionCode,
			TransactionId = result.RefTransactionCode,
			Balance = result.Balance
		};
	}

	public override async Task<FundHistoryReply> FundHistory(FundHistoryRequest request, ServerCallContext context)
	{
		var query = new FundHistoryQuery(
			ApiUrl: new(request.ApiUrl),
			MerchantId: request.MerchantId,
			MerchantKey: request.MerchantKey,
			CustomerName: request.CustomerName,
			IsFundOut: request.IsFundOut,
			StartDate: request.StartDate.ToDateTime(),
			EndDate: request.EndDate.ToDateTime());

		var fundHistorys = await fundHistoryService
			.QueryAsync(
				query: query,
				cancellationToken: context.CancellationToken)
			.ToArrayAsync(context.CancellationToken)
			.ConfigureAwait(false);

		var reply = new FundHistoryReply();
		if (fundHistorys.Length != 0)
		{
			foreach (var fundHistory in fundHistorys)
			{
				reply.Items.Add(new FundHistory()
				{
					PlayerCode = fundHistory.CustomerName,
					Amount = fundHistory.Amount,
					CreatedTime = fundHistory.CreatedTime.ToTimestamp(),
					TransactionId = fundHistory.TransactionId
				});
			}
		}

		return reply;
	}

	public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
	{
		var command = new PlayerCreateCommand(
		ApiUrl: new(request.ApiUrl),
		MerchantId: request.MerchantId,
		MerchantKey: request.MerchantKey,
		CustomerName: request.CustomerName);

		await playerCreateService
				.ExecuteAsync(
					command: command,
					cancellationToken: context.CancellationToken)
				.ConfigureAwait(false);

		return new BaseReply()
		{
			Status = true
		};
	}

	public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
	{
		var query = new PlayerBalanceQuery(
		ApiUrl: new(request.ApiUrl),
		MerchantId: request.MerchantId,
		MerchantKey: request.MerchantKey,
		CustomerName: request.CustomerName);

		var result = await playerBalanceService
				.GetAsync(
					query: query,
					cancellationToken: context.CancellationToken)
				.ConfigureAwait(false);

		return new PlayerBalanceReply()
		{
			IsPlayerExist = result.IsPlayerExist,
			Balance = result.Balance.GetValueOrDefault(),
		};
	}

	public override async Task<GameSettledHistoryReply> GameSettledHistory(GameSettledHistoryRequest request, ServerCallContext context)
	{
		var query = new GameSettledHistoryQuery(
			ApiUrl: new(request.ApiUrl),
			MerchantId: request.MerchantId,
			MerchantKey: request.MerchantKey,
			StartDate: request.StartDate.ToDateTime(),
			EndDate: request.EndDate.ToDateTime());

		var gameHistorySettleds = await gameSettledHistorySerivce
				.QueryAsync(
					query: query,
					cancellationToken: context.CancellationToken)
				.ToArrayAsync(context.CancellationToken)
				.ConfigureAwait(false);
		var reply = new GameSettledHistoryReply();

		if (gameHistorySettleds.Length != 0)
		{
			foreach (var gameHistorySettled in gameHistorySettleds)
			{
				reply.Items.Add(new GameSettledHistory()
				{
					PlayerCode = gameHistorySettled.PlayerCode,
					ExternalRoundId = gameHistorySettled.ExternalRoundId,
					ExternalCreatedOn = gameHistorySettled.ExternalCreatedOn.ToTimestamp(),
					ExternalClosedOn = gameHistorySettled.ExternalClosedOn.ToTimestamp(),
					Bet = gameHistorySettled.Bet,
					ValidBet = gameHistorySettled.ValidBet,
					PlayerWinLoss = gameHistorySettled.PlayerWinLoss,
					Turnover = gameHistorySettled.Turnover,
					Settle = gameHistorySettled.Settle,
					AdditionalData = gameHistorySettled.AdditionalData,
					IsCancelled = gameHistorySettled.IsCancelled,
				});
			}
		}

		return reply;
	}

	public override async Task<GameUnsettleHistoryReply> GameUnsettleHistory(GameUnsettleHistoryRequest request, ServerCallContext context)
	{
		var query = new GameUnsettleHistoryQuery(
			ApiUrl: new(request.ApiUrl),
			MerchantId: request.MerchantId,
			MerchantKey: request.MerchantKey,
			StartDate: request.StartDate.ToDateTime(),
			EndDate: request.EndDate.ToDateTime());

		var gameHistorySettleds = await gameUnsettleHistorySerivce
				.QueryAsync(
					query: query,
					cancellationToken: context.CancellationToken)
				.ToArrayAsync(context.CancellationToken)
				.ConfigureAwait(false);
		var reply = new GameUnsettleHistoryReply();

		if (gameHistorySettleds.Length != 0)
		{
			foreach (var gameHistorySettled in gameHistorySettleds)
			{
				reply.Items.Add(new GameUnsettleHistory()
				{
					PlayerCode = gameHistorySettled.PlayerCode,
					ExternalRoundId = gameHistorySettled.ExternalRoundId,
					ExternalCreatedOn = gameHistorySettled.ExternalCreatedOn.ToTimestamp(),
					Bet = gameHistorySettled.Bet,
					ValidBet = gameHistorySettled.ValidBet,
					AdditionalData = gameHistorySettled.AdditionalData
				});
			}
		}

		return reply;
	}
}
