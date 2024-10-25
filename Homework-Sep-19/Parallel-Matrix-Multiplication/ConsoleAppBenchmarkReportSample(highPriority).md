```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5011/22H2/2022Update)
AMD Ryzen 3 3250U with Radeon Graphics, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.401
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2


```
| Method               | leftMatrix | rightMatrix | Mean              | Error            | StdDev           |
|--------------------- |----------- |------------ |------------------:|-----------------:|-----------------:|
| **MultiplyParallel**     | **1000x1**     | **1x1000**      |   **4,726,575.78 ns** |    **53,242.167 ns** |    **47,197.795 ns** |
| MultiplySequentially | 1000x1     | 1x1000      |   5,099,624.16 ns |   101,777.409 ns |   249,661.737 ns |
| **MultiplyParallel**     | **1000x1000**  | **1000x100**    | **239,020,445.61 ns** | **4,686,767.350 ns** | **8,084,444.045 ns** |
| MultiplySequentially | 1000x1000  | 1000x100    | 268,061,445.00 ns | 5,217,436.432 ns | 6,008,407.556 ns |
| **MultiplyParallel**     | **100x200**    | **200x100**     |   **5,401,597.50 ns** |    **76,043.122 ns** |    **71,130.783 ns** |
| MultiplySequentially | 100x200    | 200x100     |   6,576,133.90 ns |   233,103.925 ns |   687,312.233 ns |
| **MultiplyParallel**     | **100x3000**   | **3000x100**    |  **76,097,742.86 ns** | **1,511,284.710 ns** | **2,118,610.270 ns** |
| MultiplySequentially | 100x3000   | 3000x100    |  80,420,793.55 ns | 1,606,971.524 ns | 2,454,015.441 ns |
| **MultiplyParallel**     | **10x1**       | **1x10**        |     **453,359.84 ns** |     **7,839.759 ns** |     **8,713.871 ns** |
| MultiplySequentially | 10x1       | 1x10        |         400.19 ns |         2.902 ns |         2.714 ns |
| **MultiplyParallel**     | **1x1**        | **1x1**         |     **125,321.41 ns** |     **1,261.935 ns** |     **1,180.414 ns** |
| MultiplySequentially | 1x1        | 1x1         |          59.09 ns |         1.048 ns |         1.247 ns |
| **MultiplyParallel**     | **2x3**        | **3x4**         |     **246,837.46 ns** |     **4,226.817 ns** |     **3,953.767 ns** |
| MultiplySequentially | 2x3        | 3x4         |         138.12 ns |         1.670 ns |         1.480 ns |
