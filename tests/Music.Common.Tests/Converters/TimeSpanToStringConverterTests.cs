using FluentAssertions;
using Music.Common.Converters;
using System.Globalization;

namespace Music.Common.Tests.Converters;

[TestFixture]
internal sealed class TimeSpanToStringConverterTests
{
    private TimeSpanToStringConverter SUT;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SUT = new TimeSpanToStringConverter();
    }

    [Test]
    [TestCase(0,"00:00")]
    [TestCase(10,"00:10")]
    [TestCase(69,"01:09")]
    [TestCase(300,"05:00")]
    [TestCase(3599,"59:59")]
    [TestCase(3600,"01:00:00")]
    [TestCase(7222, "02:00:22")]
    public void Convert_ConvertsTimeSpanToStringCorrectly(double seconds, string expected)
    {
        // arrange

        // act
        string actual = (string)SUT.Convert(seconds, typeof(Object), new object(), CultureInfo.InvariantCulture);
        
        // assert
        actual.Should().Be(expected);
    }

    [Test]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // arrange

        // act
        // assert
        SUT.Invoking(sut => sut.ConvertBack(new object(), typeof(Object), new object(), CultureInfo.InvariantCulture))
            .Should().Throw<NotImplementedException>();
    }
}
