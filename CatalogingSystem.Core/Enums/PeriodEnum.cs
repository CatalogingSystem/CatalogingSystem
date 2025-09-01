using System.Text.Json.Serialization;

namespace CatalogingSystem.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PeriodEnum
{
    BeginningOfCentury,
    MidCentury,
    EndOfCentury,
    FirstHalfOfCentury,
    SecondHalfOfCentury,
    FirstThirdOfCentury,
    SecondThirdOfCentury,
    LastThirdOfCentury,
    FirstQuarterOfCentury,
    SecondQuarterOfCentury,
    ThirdQuarterOfCentury,
    LastQuarterOfCentury,
}
