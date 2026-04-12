using System.Runtime.Serialization;

namespace Claims.Domain.Entities;

public enum CoverType
{
    [EnumMember(Value = "Yacht")]
    Yacht = 0,
    [EnumMember(Value = "PassengerShip")]
    PassengerShip = 1,
    [EnumMember(Value = "ContainerShip")]
    ContainerShip = 2,
    [EnumMember(Value = "BulkCarrier")]
    BulkCarrier = 3,
    [EnumMember(Value = "Tanker")]
    Tanker = 4
}