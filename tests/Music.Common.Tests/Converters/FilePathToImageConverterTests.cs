using FluentAssertions;
using Music.Common.Converters;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Music.Common.Tests.Converters;

[TestFixture]
internal sealed class FilePathToImageConverterTests
{
    private FilePathToImageConverter SUT;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        SUT = new FilePathToImageConverter();
    }

    /*
    [Test]
    [TestCase(1234, FilePathToImageConverter.DEFAULT_ALBUM_ICON)]
    [TestCase(null, FilePathToImageConverter.DEFAULT_ALBUM_ICON)]
    [TestCase("", FilePathToImageConverter.DEFAULT_ALBUM_ICON)]
    public void Convert_ConvertsCountToStringCorrectly(object? filePath, string expected)
    {
        // arrange
        var expectedOutput = new BitmapImage(new Uri(expected));

        // act
#pragma warning disable CS8604 // Possible null reference argument.
        var actual = (BitmapImage)SUT.Convert(filePath, typeof(Object), new object(), CultureInfo.InvariantCulture);
#pragma warning restore CS8604 // Possible null reference argument.

        // assert
        actual.Should().BeEquivalentTo(expectedOutput);
    }
    */
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
