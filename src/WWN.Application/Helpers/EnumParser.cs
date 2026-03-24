namespace WWN.Application.Helpers;

public static class EnumParser
{
    public static T Parse<T>(string value, string paramName) where T : struct, Enum
    {
        if (!Enum.TryParse<T>(value, ignoreCase: true, out var result))
            throw new ArgumentException(
                $"'{value}' is not a valid {typeof(T).Name}. Expected one of: {string.Join(", ", Enum.GetNames<T>())}",
                paramName);

        return result;
    }

    public static T ParseOrDefault<T>(string? value, T defaultValue) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return defaultValue;
        return Enum.TryParse<T>(value, ignoreCase: true, out var result) ? result : defaultValue;
    }
}
