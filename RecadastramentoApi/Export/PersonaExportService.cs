using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using RecadastramentoApi.DTO;

namespace RecadastramentoApi.Export;

public sealed class PersonaExportService : IPersonaExportService
{
    private static readonly PropertyInfo[] PersonaProperties = typeof(PersonaResponseDto)
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .OrderBy(property => property.MetadataToken)
        .ToArray();

    public byte[] ExportCsv(IEnumerable<PersonaResponseDto> personas)
    {
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|",
            HasHeaderRecord = true
        };

        using var stringWriter = new StringWriter();
        using var csvWriter = new CsvWriter(stringWriter, configuration);

        foreach (var property in PersonaProperties)
        {
            csvWriter.WriteField(ToSnakeCase(property.Name));
        }

        csvWriter.NextRecord();

        foreach (var persona in personas)
        {
            foreach (var property in PersonaProperties)
            {
                var value = property.GetValue(persona);
                csvWriter.WriteField(FormatValue(value));
            }

            csvWriter.NextRecord();
        }

        return Encoding.UTF8.GetBytes(stringWriter.ToString());
    }

    public byte[] ExportXlsx(IEnumerable<PersonaResponseDto> personas)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("personas");

        for (var column = 0; column < PersonaProperties.Length; column++)
        {
            worksheet.Cell(1, column + 1).Value = ToSnakeCase(PersonaProperties[column].Name);
        }

        var row = 2;
        foreach (var persona in personas)
        {
            for (var column = 0; column < PersonaProperties.Length; column++)
            {
                var value = PersonaProperties[column].GetValue(persona);
                worksheet.Cell(row, column + 1).Value = FormatValue(value);
            }

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    private static string? FormatValue(object? value)
    {
        return value switch
        {
            null => null,
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture),
            Dictionary<string, string> dictionary => JsonSerializer.Serialize(dictionary),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }

    private static string ToSnakeCase(string value)
    {
        var chars = new List<char>(value.Length + 8);

        for (var i = 0; i < value.Length; i++)
        {
            var currentChar = value[i];
            var isUpperCase = char.IsUpper(currentChar);

            if (i > 0 && isUpperCase)
            {
                var previousChar = value[i - 1];
                var nextCharIsLower = i + 1 < value.Length && char.IsLower(value[i + 1]);

                if (char.IsLower(previousChar) || nextCharIsLower)
                {
                    chars.Add('_');
                }
            }

            chars.Add(char.ToLowerInvariant(currentChar));
        }

        return new string(chars.ToArray());
    }
}
