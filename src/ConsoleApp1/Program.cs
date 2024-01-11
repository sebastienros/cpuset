using System.Reflection;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;
using Process = System.Diagnostics.Process;

uint[] cpuSet = [1,2,3];

var process = new Process();

// The executable should be in the same folder as the agent since it references the Console project
// Use 'dotnet exec .dll' to use the current default dotnet version or the tests could fail if the .exe doesn't match what version is available locally
process.StartInfo.FileName = "dotnet";
process.StartInfo.Arguments = "exec" + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "ConsoleApp2.dll");

// .NET doesn't respect a cpu affinity if a ratio is not set too. https://github.com/dotnet/runtime/issues/94364
process.StartInfo.EnvironmentVariables.Add("DOTNET_PROCESSOR_COUNT", cpuSet.Length.ToString());
process.StartInfo.EnvironmentVariables.Add("DOTNET_Thread_AssignCpuGroups", "0");
process.StartInfo.EnvironmentVariables.Add("DOTNET_Thread_UseAllCpuGroups", "0");

process.Start();
Console.WriteLine($"[HOST] Process started: {process.Id}");

var safeProcess = OpenProcess(ACCESS_MASK.MAXIMUM_ALLOWED, false, (uint)process.Id);
var ssi = GetSystemCpuSetInformation(safeProcess).ToArray();
var cpus = cpuSet.Select(i => ssi[i].CpuSet.Id).ToArray();
var result = SetProcessDefaultCpuSets(safeProcess, cpus, (uint)cpus.Length);
Console.WriteLine($"[HOST] Limiting CpuSet: {String.Join(',', cpuSet)} ({String.Join(',', cpus)}): {(result ? "SUCCESS" : "FAILED")}");
Console.WriteLine("[HOST] Limits applied");

process.WaitForExit();

safeProcess.Dispose();
