namespace NativeCredentialStore;

public abstract class StringEnum
{
  public string Value { get; }

  protected StringEnum(string value) => Value = value;

  public override string ToString() => Value;

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(this, obj))
    {
      return true;
    }

    if (obj is not StringEnum stringEnum)
    {
      return false;
    }

    return stringEnum.Value == Value;
  }

  public override int GetHashCode() => Value.GetHashCode();

  public static implicit operator string(StringEnum value) => value.Value;

  public static bool operator ==(StringEnum stringEnum1, StringEnum stringEnum2)
    => stringEnum1.Value == stringEnum2.Value;

  public static bool operator !=(StringEnum stringEnum1, StringEnum stringEnum2)
    => !(stringEnum1.Value == stringEnum2.Value);
}
