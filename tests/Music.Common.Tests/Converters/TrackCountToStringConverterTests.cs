using FluentAssertions;
using Music.Common.Converters;
using System.Globalization;

namespace Music.Common.Tests.Converters;

[TestFixture]
internal sealed class TrackCountToStringConverterTests
{
    private TrackCountToStringConverter SUT;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SUT = new TrackCountToStringConverter();
    }

    [Test]
    [TestCase(0, "0 Tracks")]
    [TestCase(1, "1 Track")]
    [TestCase(100, "100 Tracks")]
    [TestCase(1000, "1000 Tracks")]
    public void Convert_ConvertsCountToStringCorrectly(int count, string expected)
    {
        // arrange

        // act
        string actual = (string)SUT.Convert(count, typeof(Object), new object(), CultureInfo.InvariantCulture);
            
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
