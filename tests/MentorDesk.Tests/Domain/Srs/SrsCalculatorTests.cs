using FluentAssertions;
using MentorDesk.Domain.Srs;

namespace MentorDesk.Tests.Domain.Srs;

public class SrsCalculatorTests
{
    // Тесты на базовые интервалы без предыдущего интервала (карточка новая)

    [Fact]
    public void CalculateInterval_Score1_Returns10Minutes()
    {
        var interval = SrsCalculator.CalculateInterval(1);

        interval.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(3, 3)]
    [InlineData(4, 7)]
    [InlineData(5, 14)]
    public void CalculateInterval_ScoreWithoutPreviousInterval_ReturnsBaseInterval(int score, int expectedDays)
    {
        var interval = SrsCalculator.CalculateInterval(score);

        interval.Should().Be(TimeSpan.FromDays(expectedDays));
    }

    // Тест на рост интервала при повторных 4

    [Fact]
    public void CalculateInterval_Score4_GrowsWithFactor2()
    {
        var interval1 = SrsCalculator.CalculateInterval(4);
        interval1.Should().Be(TimeSpan.FromDays(7));

        var interval2 = SrsCalculator.CalculateInterval(4, interval1);
        interval2.Should().Be(TimeSpan.FromDays(14));

        var interval3 = SrsCalculator.CalculateInterval(4, interval2);
        interval3.Should().Be(TimeSpan.FromDays(28));
    }

    // Тест на рост интервала при повторных 5

    [Fact]
    public void CalculateInterval_Score5_GrowsWithFactor2_5()
    {
        var interval1 = SrsCalculator.CalculateInterval(5);
        interval1.Should().Be(TimeSpan.FromDays(14));

        var interval2 = SrsCalculator.CalculateInterval(5, interval1);
        interval2.Should().Be(TimeSpan.FromDays(35));

        var interval3 = SrsCalculator.CalculateInterval(5, interval2);
        interval3.Should().Be(TimeSpan.FromDays(87.5));
    }

    // Если выросший интервал меньше базы — возвращаем базу

    [Fact]
    public void CalculateInterval_GrownIntervalLessThanBase_ReturnsBase()
    {
        var intervalAfterScore4 = SrsCalculator.CalculateInterval(4, TimeSpan.FromMinutes(10));
        intervalAfterScore4.Should().Be(TimeSpan.FromDays(7));

        var intervalAfterScore5 = SrsCalculator.CalculateInterval(5, TimeSpan.FromDays(1));
        intervalAfterScore5.Should().Be(TimeSpan.FromDays(14));
    }

    // Для оценок 1-3 предыдущий интервал не влияет, всегда база

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CalculateInterval_Scores1To3_AlwaysReturnBase(int score)
    {
        var previousInterval = TimeSpan.FromDays(35);

        var interval = SrsCalculator.CalculateInterval(score, previousInterval);

        var expected = score switch
        {
            1 => TimeSpan.FromMinutes(10),
            2 => TimeSpan.FromDays(1),
            3 => TimeSpan.FromDays(3),
            _ => throw new InvalidOperationException()
        };

        interval.Should().Be(expected);
    }

    // После большого интервала оценка 1 сбрасывает до 10 минут

    [Fact]
    public void CalculateInterval_AfterScore5_Score1ResetsToTenMinutes()
    {
        var previousInterval = TimeSpan.FromDays(35);

        var nextInterval = SrsCalculator.CalculateInterval(1, previousInterval);

        nextInterval.Should().Be(TimeSpan.FromMinutes(10));
    }

    // Оценка вне 1-5 — исключение

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void CalculateInterval_InvalidScore_ThrowsArgumentOutOfRangeException(int invalidScore)
    {
        var action = () => SrsCalculator.CalculateInterval(invalidScore);

        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("score");
    }

    // Тесты на NextReviewAt с передачей времени параметром (не DateTime.Now)

    [Fact]
    public void NextReviewAt_WithFromUtc_AddsInterval()
    {
        var now = new DateTimeOffset(2026, 10, 21, 12, 0, 0, TimeSpan.Zero);

        var nextReviewAt = SrsCalculator.NextReviewAt(score: 2, previousInterval: null, fromUtc: now);

        nextReviewAt.Should().Be(now.AddDays(1));
    }

    [Fact]
    public void NextReviewAt_WithTimeProvider_UsesInjectedTime()
    {
        var fixedTime = new DateTimeOffset(2026, 10, 21, 10, 0, 0, TimeSpan.Zero);
        var timeProvider = new TestTimeProvider(fixedTime);

        var nextReviewAt = SrsCalculator.NextReviewAt(score: 1, previousInterval: null, timeProvider: timeProvider);

        nextReviewAt.Should().Be(fixedTime.AddMinutes(10));
    }

    private sealed class TestTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
