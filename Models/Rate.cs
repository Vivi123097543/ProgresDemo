using System;
using System.Text.Json.Serialization;

namespace ProgresDemo.Models;

public class Rate
{
    [JsonPropertyName("Cur_ID")]
    public int Cur_ID { get; set; }

    [JsonPropertyName("Date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("Cur_Abbreviation")]
    public string Cur_Abbreviation { get; set; } = string.Empty;

    [JsonPropertyName("Cur_Scale")]
    public int Cur_Scale { get; set; }

    [JsonPropertyName("Cur_Name")]
    public string Cur_Name { get; set; } = string.Empty;

    [JsonPropertyName("Cur_OfficialRate")]
    public decimal Cur_OfficialRate { get; set; }
}
