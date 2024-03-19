namespace Tests.NativeCredentialStore;

[PlatformTrait(Platform.All)]
public class StringEnumTests
{
  private class CarColor : StringEnum
  {
    protected CarColor(string value) : base(value) {}

    public static readonly CarColor Red = new("red");

    public static readonly CarColor Blue = new("blue");
  }

  private class ComputerColor : StringEnum
  {
    private ComputerColor(string value) : base(value) {}

    public static readonly ComputerColor Red = new("red");
  }

  [Fact]
  public void Equals_SameReference_ReturnsTrue()
  {
    var red = CarColor.Red;
    Assert.True(CarColor.Red.Equals(red));
  }

  [Fact]
  public void Equals_DifferentColor_ReturnsFalse()
    => Assert.False(CarColor.Red.Equals(CarColor.Blue));

  [Fact]
  public void Equals_SameColorButDifferentEnum_ReturnsFalse()
    => Assert.False(CarColor.Red.Equals(ComputerColor.Red));

  [InlineData("")]
  [InlineData(' ')]
  [InlineData(100)]
  [InlineData(null)]
  [Theory]
  public void Equals_DifferentObjectType_ReturnsFalse(object obj)
    => Assert.False(CarColor.Red.Equals(obj));
}
