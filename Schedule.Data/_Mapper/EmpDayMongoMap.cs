using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Schedule.Data.BaseModels;
using Schedule.Data.Entities;

namespace Schedule.Data._Mapper;

public class EmpDayMongoMap : IDomainClassMap
{
    public EmpDayMongoMap()
    {
        BsonClassMap.RegisterClassMap<EmpDay>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.GetMemberMap(c => c.DayType).SetElementName("t");
            cm.GetMemberMap(c => c.Date).SetElementName("d");
            cm.GetMemberMap(c => c.Hours).SetElementName("hd");
            cm.GetMemberMap(c => c.EmployeeId).SetElementName("e");
            cm.GetMemberMap(c => c.OrganizationId).SetElementName("o");
            
            cm.GetMemberMap(c => c.Hours).SetSerializer(new HoursDetailSerializer());
        });
    }
}

public class HoursDetailSerializer : SerializerBase<HoursDetail>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, HoursDetail value)
    {
        var bsonWriter = context.Writer;

        bsonWriter.WriteStartDocument();
        bsonWriter.WriteDecimal128("s", value.Summary); // Мапінг поля Summary на s
        bsonWriter.WriteDecimal128("d", value.Day); // Мапінг поля Day на d
        bsonWriter.WriteDecimal128("n", value.Night); // Мапінг поля Night на n
        bsonWriter.WriteDecimal128("e", value.Evening); // Мапінг поля Evening на e
        bsonWriter.WriteBoolean("h", value.Holiday); // Мапінг поля Holiday на h
        bsonWriter.WriteEndDocument();
    }

    public override HoursDetail Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;

        bsonReader.ReadStartDocument();
        var summary = bsonReader.ReadDecimal128("s"); // Мапінг поля Summary на s
        var day = bsonReader.ReadDecimal128("d"); // Мапінг поля Day на d
        var night = bsonReader.ReadDecimal128("n"); // Мапінг поля Night на n
        var evening = bsonReader.ReadDecimal128("e"); // Мапінг поля Evening на e
        var holiday = bsonReader.ReadBoolean("h"); // Мапінг поля Holiday на h
        bsonReader.ReadEndDocument();

        return new HoursDetail
        {
            Summary = (decimal)summary,
            Day = (decimal)day,
            Night = (decimal)night,
            Evening = (decimal)evening,
            Holiday = holiday
        };
    }
}

/// <summary>
/// Базовий інтерфейс профілю доменної моделі
/// </summary>
public interface IDomainClassMap
{
}