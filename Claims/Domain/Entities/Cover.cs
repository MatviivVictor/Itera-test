using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain.Entities;

public class Cover
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("startDate")]
    // [BsonDateTimeOptions(DateOnly = true)]
    /*
     * In the MongoDB.EntityFrameworkCore provider, the [BsonDateTimeOptions(DateOnly = true)] attribute is not currently supported and will throw a NotSupportedException if used on a property mapped through the EF Core provider. 
    */
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    // [BsonDateTimeOptions(DateOnly = true)]
    /*
     * In the MongoDB.EntityFrameworkCore provider, the [BsonDateTimeOptions(DateOnly = true)] attribute is not currently supported and will throw a NotSupportedException if used on a property mapped through the EF Core provider.
     */
    public DateTime EndDate { get; set; }

    [BsonElement("claimType")]
    public CoverType Type { get; set; }

    [BsonElement("premium")]
    public decimal Premium { get; set; }
}