namespace NativeCredentialStore;

/// <summary>
/// Represent a string enum.
/// </summary>
public abstract class StringEnum
{
  /// <summary>
  /// The enum value.
  /// </summary>
  public string Value { get; }

  /// <summary>
  /// Constructor.
  /// </summary>
  /// <param name="value">Enum value.</param>
  protected StringEnum(string value) => Value = value;

  /// <inheritdoc/>
  public override string ToString() => Value;

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public override int GetHashCode() => Value.GetHashCode();

  /// <summary>
  /// Implicitly convert <paramref name="stringEnum"/> to string.
  /// </summary>
  /// <param name="stringEnum">String enum object.</param>
  public static implicit operator string(StringEnum stringEnum) => stringEnum.Value;

  /// <summary>
  /// Compare 2 string enums.
  /// </summary>
  /// <param name="stringEnum1"></param>
  /// <param name="stringEnum2"></param>
  /// <returns>True if equal, false otherwise.</returns>
  public static bool operator ==(StringEnum stringEnum1, StringEnum stringEnum2)
    => stringEnum1.Equals(stringEnum2);

  /// <summary>
  /// The invert of "==".
  /// </summary>
  /// <param name="stringEnum1"></param>
  /// <param name="stringEnum2"></param>
  /// <returns>True if not equal, false otherwise.</returns>
  public static bool operator !=(StringEnum stringEnum1, StringEnum stringEnum2)
    => !(stringEnum1.Value == stringEnum2.Value);
}
