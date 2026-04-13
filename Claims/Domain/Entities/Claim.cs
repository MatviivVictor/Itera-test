using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain.Entities
{
    public class Claim
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("coverId")]
        public string CoverId { get; set; }

        [BsonElement("created")]
        // [BsonDateTimeOptions(DateOnly = true)]
        /*
         * In the MongoDB.EntityFrameworkCore provider, the [BsonDateTimeOptions(DateOnly = true)] attribute is not currently supported and will throw a NotSupportedException if used on a property mapped through the EF Core provider.
         */
        public DateTime Created { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("claimType")]
        public ClaimType Type { get; set; }

        [BsonElement("damageCost")]
        public decimal DamageCost { get; set; }
    }
}
