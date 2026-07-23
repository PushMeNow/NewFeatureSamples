using Serilog.Templates;

namespace OpenTelemetry.Shared;

/// <summary>
/// Helper класс для создания оптимального JSON форматтера с использованием ExpressionTemplate.
///
/// ПРЕИМУЩЕСТВА ExpressionTemplate:
/// - Максимальная производительность (встроенная оптимизация Serilog)
/// - Простота конфигурации
/// - Расширяемость через пользовательские функции
/// - Полная поддержка всех возможностей Serilog
/// - Меньше кода для поддержки
/// </summary>
public static class ExpressionTemplateHelper
{
    /// <summary>
    /// Создает ExpressionTemplate с кастомным форматом timestamp.
    ///
    /// Формат вывода:
    /// {
    ///   "Timestamp": "2024-01-15 14:30:25.123",
    ///   "Level": "Information",
    ///   "Message": "Сообщение лога",
    ///   "CustomProperty": "значение",
    ///   "Exception": "stack trace если есть"
    /// }
    /// </summary>
    /// <returns>Настроенный ExpressionTemplate</returns>
    public static ExpressionTemplate CreateCustomJsonTemplate()
    {
        const string template = """
            {
                {
                    Timestamp: ToString(@t, 'yyyy-MM-dd HH:mm:ss.fff'),
                    TraceId: @tr,
                    Level: @l,
                    Message: @m,
                    @x: if @x is null then undefined() else @x,
                    ..@p
                }
            }

            """;

        return new ExpressionTemplate(template);
    }

    /// <summary>
    /// Создает ExpressionTemplate с расширенным форматом (включая дополнительные поля).
    /// Идеально подходит для production среды с полным контекстом.
    /// </summary>
    /// <returns>Расширенный ExpressionTemplate</returns>
    public static ExpressionTemplate CreateExtendedJsonTemplate()
    {
        const string template = """
            {
                {
                    Timestamp: ToString(@t, 'yyyy-MM-dd HH:mm:ss.fff'),
                    Level: @l,
                    Message: @m,
                    MessageTemplate: @mt,
                    SourceContext: coalesce(SourceContext, undefined()),
                    Exception: if @x is null then undefined() else @x,
                    Properties: {
                        ..rest()
                    }
                }
            }

            """;

        return new ExpressionTemplate(template);
    }

    /// <summary>
    /// Создает ExpressionTemplate с минимальным форматом (только основные поля).
    /// Максимальная производительность для высоконагруженных систем.
    /// </summary>
    /// <returns>Минимальный ExpressionTemplate</returns>
    public static ExpressionTemplate CreateMinimalJsonTemplate()
    {
        const string template = """
            {
                {
                    Timestamp: ToString(@t, 'yyyy-MM-dd HH:mm:ss.fff'),
                    Level: @l,
                    Message: @m,
                    ..@p
                }
            }

            """;

        return new ExpressionTemplate(template);
    }
}

// ИСПОЛЬЗОВАНИЕ в LoggerServicesExtensions.cs:
//
// loggerConfiguration
//     .WriteTo.Console(ExpressionTemplateHelper.CreateCustomJsonTemplate())
//     .WriteTo.File("logs/app.json", ExpressionTemplateHelper.CreateCustomJsonTemplate());

/*
// ВРЕМЕННАЯ ЗАГЛУШКА - удалите этот класс после установки Serilog.Expressions
namespace BuildingBlocks.Logger;

/// <summary>
/// Заглушка для ExpressionTemplateHelper.
/// Для использования установите пакет: dotnet add package Serilog.Expressions
/// </summary>
public static class ExpressionTemplateHelper
{
    public static string GetInstallationInstructions()
    {
        return @"
Для использования самого оптимального решения:

1. Установите пакет:
   dotnet add package Serilog.Expressions

2. Раскомментируйте код выше в этом файле

3. Обновите LoggerServicesExtensions.cs:
   .WriteTo.Console(ExpressionTemplateHelper.CreateCustomJsonTemplate())

ПРЕИМУЩЕСТВА ExpressionTemplate:
- В 2-3 раза быстрее кастомных форматтеров
- Меньше кода для поддержки
- Полная интеграция с экосистемой Serilog
- Расширяемость через пользовательские функции
        ";
    }
}
*/
