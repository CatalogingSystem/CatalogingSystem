using System.Text.Json.Serialization;

namespace CatalogingSystem.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntryForm
{
    ByConfiscation,
    Purchase,
    Dation,
    Excavation,
    Bequest,
    Donation,
    OrderingAndReordering,
    Exchange,
    Awards,
    Usucapion,
    RegistrationByReintegration,
    ChangeOfAssignment,
    Offering,
    OwnProduction,
    Collection,
    ForStudy,
    Exhibition,
    Conservation,
    PublicOwnershipDeposit,
    ThirdPartyDeposit,
    JudicialDeposit,
    DepositPriorToAcquisition,
}
