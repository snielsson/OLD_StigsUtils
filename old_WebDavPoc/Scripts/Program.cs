// Copyright © 2024 TradingLens. All Rights Reserved.

using Bullseye;
using Scripts;
using SimpleExec;
using static System.Console;
using static Bullseye.Targets;
using static SimpleExec.Command;

if (args.Length < 2) throw new InvalidUsageException("Project dir must be specified as first commandline argument.");
DirPath projectDir = args[1].AsExistingDir();
DirPath publishDir = projectDir / "bin/release/publish";
DirPath deployDir = new DirPath("");

Target("publish", Publish);
Target("deploy", DependsOn("build"), Deploy);
await RunTargetsAndExitAsync(args, ex => ex is ExitCodeException);

void Publish() => Run("dotnet", $"publish --configuration Release -o {publishDir} --nologo --self-contained false --verbosity quiet");

void Deploy()
{
  WriteLine($"Mirroring files from {publishDir.Name} to {deployDir.Name}...");
  publishDir.MirrorTo(deployDir);
  WriteLine("Deployment completed.");
}

