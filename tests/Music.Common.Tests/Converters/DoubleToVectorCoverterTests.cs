using FluentAssertions;
using Music.Common.Converters;
using System.Globalization;

namespace Music.Common.Tests.Converters;

[TestFixture]
internal sealed class DoubleToVectorCoverterTests
{
    private DoubleToVectorConverter SUT;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SUT = new DoubleToVectorConverter();
    }

    [Test]
    [TestCase(0, VolumeSvgIcons.MUTE_VOLUME)]
    [TestCase(10, VolumeSvgIcons.HALF_VOLUME)]
    [TestCase(50, VolumeSvgIcons.HALF_VOLUME)]
    [TestCase(51, VolumeSvgIcons.FULL_VOLUME)]
    [TestCase(87, VolumeSvgIcons.FULL_VOLUME)]
    [TestCase(100, VolumeSvgIcons.FULL_VOLUME)]
    public void Convert_ConvertsTimeSpanToStringCorrectly(double volume, string expected)
    {
        // arrange

        // act
        string actual = (string)SUT.Convert(volume, typeof(Object), new object(), CultureInfo.InvariantCulture);

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
