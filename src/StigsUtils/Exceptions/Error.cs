// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

namespace StigsUtils.Exceptions;

public class Error : Exception
{
  public Error() { }
  public Error(string? message) : base(message) { }
  public Error(string? message, Exception? innerException) : base(message, innerException) { }
}
#pragma warning disable RCS1194
public class Error<T> : Error
#pragma warning restore RCS1194
{
  public Error(T thrower, string? message = null, Exception? innerException = null) : base(message, innerException)
  {
    Thrower = thrower;
  }
  public T? Thrower { get; protected set; }
}

public static class ErrorExtensions
{
  public static Error<T> Error<T>(this T @this, string message) => new(@this, message);
  public static Error Error(this object @this, string message) => new(message);
  public static Error ToError(this Exception @this) => new(@this.Message,@this);
  public static Error<T> ToError<T>(this Exception @this, T thrower) => new(thrower, @this.Message,@this);
}