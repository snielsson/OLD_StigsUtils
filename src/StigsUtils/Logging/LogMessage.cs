// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

using Microsoft.Extensions.Logging;

namespace StigsUtils.Logging;

public record LogMessage(
  LogLevel LogLevel,
  string Message,
  string Category,
  EventId EventId,
  Exception? Exception,
  object? Context) : LogMessage<object?>(LogLevel, Message, Category, EventId, Exception, Context);
  
public record LogMessage<T>(
  LogLevel LogLevel,
  string Message,
  string Category,
  EventId EventId,
  Exception? Exception,
  T? Context);