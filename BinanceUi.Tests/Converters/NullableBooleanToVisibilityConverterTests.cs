using System.Globalization;
using System.Windows;
using BinanceUi.Converters;

namespace BinanceUi.Tests.Converters;

[TestFixture]
public class NullableBooleanToVisibilityConverterTests
{
    [SetUp]
    public void SetUp()
    {
        _itemUnderTest = new NullableBooleanToVisibilityConverter();
    }

    private NullableBooleanToVisibilityConverter _itemUnderTest;

    [TestCase(null, false, Visibility.Collapsed)]
    [TestCase(null, true, Visibility.Collapsed)]
    [TestCase(false, false, Visibility.Collapsed)]
    [TestCase(true, true, Visibility.Collapsed)]
    [TestCase(true, false, Visibility.Visible)]
    [TestCase(false, true, Visibility.Visible)]
    public void Convert_ReturnsExpectedResult(object value, bool inverse, Visibility expected)
    {
        _itemUnderTest!.Inverse = inverse;

        var result = _itemUnderTest.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _itemUnderTest.ConvertBack(null, null, null, null));
    }
}