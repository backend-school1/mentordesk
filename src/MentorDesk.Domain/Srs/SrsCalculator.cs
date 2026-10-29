namespace MentorDesk.Domain.Srs;

// Базовые интервалы: 1 -> 10 минут, 2 -> 1 день, 3 -> 3 дня, 4 -> 7 дней, 5 -> 14 дней.
// Для 4 и 5 интервал растёт: max(база, предыдущий * 2) и max(база, предыдущий * 2.5) соответственно.
// Для 1-3 всегда возвращается базовый интервал.
public static class SrsCalculator
{
    private static readonly TimeSpan BaseInterval1 = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan BaseInterval2 = TimeSpan.FromDays(1);
    private static readonly TimeSpan BaseInterval3 = TimeSpan.FromDays(3);
    private static readonly TimeSpan BaseInterval4 = TimeSpan.FromDays(7);
    private static readonly TimeSpan BaseInterval5 = TimeSpan.FromDays(14);

    // previousInterval - интервал предыдущего показа, null если карточка новая
    public static TimeSpan CalculateInterval(int score, TimeSpan? previousInterval = null)
    {
        return score switch
        {
            1 => BaseInterval1,
            2 => BaseInterval2,
            3 => BaseInterval3,
            4 => GrowingInterval(previousInterval, BaseInterval4, 2.0),
            5 => GrowingInterval(previousInterval, BaseInterval5, 2.5),
            _ => throw new ArgumentOutOfRangeException(nameof(score), score, "Оценка должна быть от 1 до 5.")
        };
    }

    // Когда показать карточку снова, fromUtc передаётся параметром (не DateTime.Now внутри)
    public static DateTimeOffset NextReviewAt(int score, TimeSpan? previousInterval, DateTimeOffset fromUtc)
    {
        return fromUtc.Add(CalculateInterval(score, previousInterval));
    }

    // Перегрузка через TimeProvider для тестов
    public static DateTimeOffset NextReviewAt(int score, TimeSpan? previousInterval = null, TimeProvider? timeProvider = null)
    {
        var provider = timeProvider ?? TimeProvider.System;
        return NextReviewAt(score, previousInterval, provider.GetUtcNow());
    }

    private static TimeSpan GrowingInterval(TimeSpan? previousInterval, TimeSpan baseInterval, double factor)
    {
        if (!previousInterval.HasValue || previousInterval.Value <= TimeSpan.Zero)
        {
            return baseInterval;
        }

        var grown = TimeSpan.FromTicks((long)Math.Round(previousInterval.Value.Ticks * factor));
        return grown > baseInterval ? grown : baseInterval;
    }
}
