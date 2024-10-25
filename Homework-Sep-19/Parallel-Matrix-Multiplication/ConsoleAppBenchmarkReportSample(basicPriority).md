```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5011/22H2/2022Update)
AMD Ryzen 3 3250U with Radeon Graphics, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.401
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2


```
| Method               | leftMatrix | rightMatrix | Mean              | Error             | StdDev            | Median            |
|--------------------- |----------- |------------ |------------------:|------------------:|------------------:|------------------:|
| **MultiplyParallel**     | **1000x1**     | **1x1000**      |   **4,574,393.19 ns** |     **76,707.935 ns** |     **67,999.586 ns** |   **4,574,361.72 ns** |
| MultiplySequentially | 1000x1     | 1x1000      |   4,766,625.49 ns |     78,136.379 ns |     73,088.817 ns |   4,780,275.39 ns |
| **MultiplyParallel**     | **1000x1000**  | **1000x100**    | **238,324,958.79 ns** |  **4,690,588.834 ns** |  **9,996,027.983 ns** | **238,679,433.33 ns** |
| MultiplySequentially | 1000x1000  | 1000x100    | 366,284,640.00 ns | 10,057,457.813 ns | 29,654,643.434 ns | 376,850,600.00 ns |
| **MultiplyParallel**     | **100x200**    | **200x100**     |   **5,281,450.28 ns** |     **95,884.229 ns** |    **128,002.702 ns** |   **5,280,599.22 ns** |
| MultiplySequentially | 100x200    | 200x100     |   6,538,494.98 ns |    241,279.797 ns |    711,418.976 ns |   6,471,202.34 ns |
| **MultiplyParallel**     | **100x3000**   | **3000x100**    |  **75,510,870.33 ns** |  **1,378,606.489 ns** |  **1,151,198.856 ns** |  **75,636,328.57 ns** |
| MultiplySequentially | 100x3000   | 3000x100    |  81,082,990.65 ns |  1,605,123.870 ns |  2,894,369.662 ns |  80,128,733.33 ns |
| **MultiplyParallel**     | **10x1**       | **1x10**        |     **445,487.84 ns** |      **3,267.554 ns** |      **2,896.601 ns** |     **444,974.56 ns** |
| MultiplySequentially | 10x1       | 1x10        |         391.43 ns |          3.288 ns |          2.746 ns |         390.91 ns |
| **MultiplyParallel**     | **1x1**        | **1x1**         |     **122,047.62 ns** |      **2,389.934 ns** |      **2,752.251 ns** |     **121,186.44 ns** |
| MultiplySequentially | 1x1        | 1x1         |          59.76 ns |          1.205 ns |          1.127 ns |          59.37 ns |
| **MultiplyParallel**     | **2x3**        | **3x4**         |     **250,221.40 ns** |      **4,977.984 ns** |      **7,139.276 ns** |     **249,596.01 ns** |
| MultiplySequentially | 2x3        | 3x4         |         139.90 ns |          2.517 ns |          2.355 ns |         138.79 ns |
