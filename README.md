`ConsoleApp2` creates some threads and displays the CPU id it runs on.
`ConsoleApp1` starts `ConsoleApp2` and calls `SetProcessDefaultCpuSets` to limit the CPUs `ConsoleApp2` should create threads on.
