using System.Text.Json.Serialization;

namespace Tgs.GameInterface.SM01Service.Core.Models;

public record GetTransationsHistoryResult
{
    [JsonPropertyName("bet_id")]
    public string BetId { get; init; } = default!;

    [JsonPropertyName("game_number")]
    public string GameNumber { get; init; } = default!;

    [JsonPropertyName("bet_side")]
    public string BetSide { get; init; } = default!;

    [JsonPropertyName("win_or_lost")]
    public int WinOrLost { get; init; }

    [JsonPropertyName("create_at")]
    public DateTime CreateAt { get; init; }

    [JsonPropertyName("winner")]
    public string Winner { get; init; } = default!;

    [JsonPropertyName("bet_amount")]
    public decimal BetAmount { get; init; }

    [JsonPropertyName("odds")]
    public string Odds { get; init; } = default!;

    [JsonPropertyName("customerName")]
    public string PlayerCode { get; init; } = default!;
}
