using System.Globalization;
using System;
using System.Text.Json.Serialization;

namespace Tgs.GameInterface.SM01Service.Core.Models;

public record GetTransferHistoryResult
{
    [JsonPropertyName("merchant_transaction_id")]
    public string TransactionId { get; set; } = default!;

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = default!;

    [JsonPropertyName("created_time")]
    public string _createdTime { get; init; } = default!;

    [JsonIgnore]
    public DateTime CreatedTime => DateTime.ParseExact(_createdTime, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);    
}
