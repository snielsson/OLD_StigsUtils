// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

namespace StigsUtils.Applications;

public class App
{
  public static IAppBuilder Services => new AppBuilder();
}