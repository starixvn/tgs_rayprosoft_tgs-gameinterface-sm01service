using System.Text.Json.Serialization;

namespace Tgs.GameInterface.SM01Service.Core.Models;

public record GetTransationsHistoryResult
{
    [JsonPropertyName("bet_id")]
    public string BetId { get; set; } = default!;

    [JsonPropertyName("game_number")]
    public string GameNumber { get; set; } = default!;

    [JsonPropertyName("bet_side")]
    public string BetSide { get; set; } = default!;

    [JsonPropertyName("win_or_lost")]
    public string WinOrLost { get; set; } = default!;

    [JsonPropertyName("create_at")]
    public DateTime CreateAt { get; set; }

    [JsonPropertyName("winner")]
    public string Winner { get; set; } = default!;

    [JsonPropertyName("bet_amount")]
    public decimal BetAmount { get; set; }

    [JsonPropertyName("odds")]
    public string Odds { get; set; } = default!;

    [JsonPropertyName("customerName")]
    public string PlayerCode { get; set; } = default!;
}
