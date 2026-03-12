using System.Data;
using System.Text.Json;
using Dapper;

namespace RecadastramentoApi.Database;

public sealed class JsonDictionaryTypeHandler : SqlMapper.TypeHandler<Dictionary<string, string>?>
{
    public override void SetValue(IDbDataParameter parameter, Dictionary<string, string>? value)
    {
        parameter.Value = value is null ? DBNull.Value : JsonSerializer.Serialize(value);
    }

    public override Dictionary<string, string>? Parse(object value)
    {
        if (value is DBNull)
        {
            return null;
        }

        if (value is string json)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        if (value is IDictionary<string, string> dictionary)
        {
            return new Dictionary<string, string>(dictionary);
        }

        throw new DataException($"Unable to parse hstore payload of type {value.GetType().Name}.");
    }
}
