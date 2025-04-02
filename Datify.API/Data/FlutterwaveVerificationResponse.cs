using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Datify.Shared.Models;

namespace Datify.API.Data;

public class FlutterWaveVerificationResponse : BaseProperties
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public Data? Data { get; set; }
}

public class Data
{
    //[Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //public int DatabaseId { get; set; }
    public int? Id { get; set; }
    //add json property
    [JsonPropertyName("tx_ref")]
    public string? TxRef { get; set; }
    //add json property
    [JsonPropertyName("flw_ref")]
    public string? FlwRef { get; set; }
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
    [JsonPropertyName("charged_amount")]
    public decimal? ChargedAmount { get; set; }
    [JsonPropertyName("app_fee")]
    public float? AppFee { get; set; }
    [JsonPropertyName("merchant_fee")]
    public decimal? MerchantFee { get; set; }
    [JsonPropertyName("processor_response")]
    public string? ProcessorResponse { get; set; }
    [JsonPropertyName("auth_model")]
    public string? AuthModel { get; set; }
    [JsonPropertyName("ip")]
    public string? IP { get; set; }
    [JsonPropertyName("narration")]
    public string? Narration { get; set; }
    [JsonPropertyName("status_code")]
    public string? StatusCode { get; set; }
    [JsonPropertyName("payment_type")]
    public string? PaymentType { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
    [JsonPropertyName("account_id")]
    public int? AccountId { get; set; }


    //public int id { get; set; }
    //public string tx_ref { get; set; }
    //public string flw_ref { get; set; }
    //public string device_fingerprint { get; set; }
    //public int amount { get; set; }
    //public string currency { get; set; }
    //public int charged_amount { get; set; }
    //public float app_fee { get; set; }
    //public int merchant_fee { get; set; }
    //public string processor_response { get; set; }
    //public string auth_model { get; set; }
    //public string ip { get; set; }
    //public string narration { get; set; }
    //public string status { get; set; }
    //public string payment_type { get; set; }
    //public DateTime created_at { get; set; }
    //public int account_id { get; set; }
    public Meta? Meta { get; set; }
    public float AmountSettled { get; set; }
    public Customer? Customer { get; set; }
}

public class Meta
{
    public int Id { get; set; }
    [JsonPropertyName("__checkout_init_address")]
    public string? __CheckoutInitAddress { get; set; }
    [JsonPropertyName("originator_account_number")]
    public string? OriginatorAccountNumber { get; set; }
    [JsonPropertyName("originator_name")]
    public string? OriginatorName { get; set; }
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }
    [JsonPropertyName("originator_amount")]
    public string? OriginatorAmount { get; set; }
}
[NotMapped]
public class Customer
{
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
}

