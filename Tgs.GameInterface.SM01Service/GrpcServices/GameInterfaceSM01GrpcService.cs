using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.SM01Service.Core.ApplicationServices;
using Tgs.GameInterface.SM01Service.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SM01Service.GrpcServices
{
    internal class GameInterfaceSM01GrpcService : GameInterfaceSM01Service.GameInterfaceSM01ServiceBase
    {
        private readonly IGetService<FundInQuery, FundResult> _fundInService;
        private readonly IGetService<FundOutQuery, FundResult> _fundOutService;
        private readonly IQueryService<FundHistoryQuery, FundHistoryResult> _fundHistoryService;
        private readonly ICommandService<PlayerCreateCommand> _playerCreateService;
        private readonly IGetService<PlayerBalanceQuery, PlayerBalanceResult> _playerBalanceService;
        private readonly IQueryService<GameHistorySettledQuery, GameHistorySettledResult> _gameHistorySettledService;
        private readonly IGetService<PlatformLoginQuery, PlatformLoginResult> _platformLoginService;

        public GameInterfaceSM01GrpcService(
            IGetService<FundInQuery, FundResult> fundInService,
            IGetService<FundOutQuery, FundResult> fundOutService,
            IQueryService<FundHistoryQuery, FundHistoryResult> fundHistoryService,
            ICommandService<PlayerCreateCommand> playerCreateService,
            IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
            IQueryService<GameHistorySettledQuery, GameHistorySettledResult> gameHistorySettledService,
            IGetService<PlatformLoginQuery, PlatformLoginResult> platformLoginService)
        {
            _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
            _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
            _fundHistoryService = fundHistoryService ?? throw new ArgumentNullException(nameof(fundHistoryService));
            _playerCreateService = playerCreateService ?? throw new ArgumentNullException(nameof(playerCreateService));
            _playerBalanceService = playerBalanceService ?? throw new ArgumentNullException(nameof(playerBalanceService));
            _gameHistorySettledService = gameHistorySettledService ?? throw new ArgumentNullException(nameof(gameHistorySettledService));
            _platformLoginService = platformLoginService ?? throw new ArgumentNullException(nameof(platformLoginService));
        }

        public override async Task<PlatformLoginReply> PlatformLogin(PlatformLoginRequest request, ServerCallContext context)
        {
            var query = new PlatformLoginQuery(
                ApiUrl: new Uri(request.ApiUrl),
                MerchantId: request.MerchantId,
                MerchantKey: request.MerchantKey,
                CustomerName: request.CustomerName);

            var result = await _platformLoginService
                .GetAsync(
                    query: query,
                    cancellationToken: context.CancellationToken)
                .ConfigureAwait(false);

            return new PlatformLoginReply()
            {
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

            var result = await _fundInService
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

            var result = await _fundOutService
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

            var fundHistorys = await _fundHistoryService
                .QueryAsync(
                    query: query,
                    cancellationToken: context.CancellationToken)
                .ToArrayAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var reply = new FundHistoryReply();
            if (fundHistorys.Any())
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

            await _playerCreateService
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

            var result = await _playerBalanceService
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

        public override async Task<GameHistorySettledReply> GameHistorySettled(GameHistorySettledRequest request, ServerCallContext context)
        {
            var query = new GameHistorySettledQuery(
                ApiUrl: new(request.ApiUrl),
                MerchantId: request.MerchantId,
                MerchantKey: request.MerchantKey,
                StartDate: request.StartDate.ToDateTime(),
                EndDate: request.EndDate.ToDateTime());

            var gameHistorySettleds = await _gameHistorySettledService
                    .QueryAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ToArrayAsync(context.CancellationToken)
                    .ConfigureAwait(false);
            var reply = new GameHistorySettledReply();

            if (gameHistorySettleds.Any())
            {
                foreach (var gameHistorySettled in gameHistorySettleds)
                {
                    reply.Items.Add(new GameHistorySettled()
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
                        AdditionalData = gameHistorySettled.AdditionalData
                    });
                }
            }

            return reply;
        }
    }
}
