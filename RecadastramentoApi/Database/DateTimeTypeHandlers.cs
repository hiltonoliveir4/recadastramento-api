using System.Data;
using Dapper;

namespace RecadastramentoApi.Database;

public sealed class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value;
    }

    public override DateTime Parse(object value)
    {
        return ConvertToDateTime(value);
    }

    internal static DateTime ConvertToDateTime(object value)
    {
        return value switch
        {
            DateTime dateTime => dateTime,
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            _ => Convert.ToDateTime(value)
        };
    }
}

public sealed class NullableDateTimeTypeHandler : SqlMapper.TypeHandler<DateTime?>
{
    public override void SetValue(IDbDataParameter parameter, DateTime? value)
    {
        parameter.Value = value.HasValue ? value.Value : DBNull.Value;
    }

    public override DateTime? Parse(object value)
    {
        if (value is DBNull)
        {
            return null;
        }

        return DateTimeTypeHandler.ConvertToDateTime(value);
    }
}
