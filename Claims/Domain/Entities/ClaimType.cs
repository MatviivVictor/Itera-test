using System.Runtime.Serialization;

namespace Claims.Domain.Entities;

public enum ClaimType
{
    [EnumMember(Value = "Collision")]
    Collision = 0,
    [EnumMember(Value = "Grounding")]
    Grounding = 1,
    [EnumMember(Value = "BadWeather")]
    BadWeather = 2,
    [EnumMember(Value = "Fire")]
    Fire = 3
}