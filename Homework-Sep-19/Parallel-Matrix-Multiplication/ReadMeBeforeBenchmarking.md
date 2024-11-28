# How to run the project

The project is to be run with working directory containing the solution directory in the "Release" configuration

The command to run the project. You must be in the directory ConsoleAppBenchmark to make the command work.
```bash
dotnet run -c Release
```

The command to run the project. You must be in the directory ConsoleAppBenchmark to make the command work. The command will run the application with high process priority. It may affect the benchmarking results. You may see it in attached benchmark report samples.
```bash
nice -n -20 dotnet run -c Release
```

## Where to find the benchmark output
It is to be contained in the working directory in the folder BenchmarkDotNet.Artifacts

## Here are some basic benchmarking advices
* read about process priority above;
* restart PC;
* it is better to run the test using the dotnet CLI;
* turn off all other applications except the current and standard operating system processes;
* if a laptop is used for testing, it is better to connect it to the mains and use the most productive mode;
