# .NET Core implementation of UUIDv6, 7 and 8

Draft prototypes for UUIDv6 and beyond. Timestamp-based and machine-sortable.

- UUIDv6 - UUIDv6, aims to be the easiest to implement for
applications which already implement UUIDv1.  
The UUIDv6 specification keeps the original Gregorian timestamp source but does
not reorder the timestamp bits as per the process utilized by UUIDv1.  
UUIDv6 also requires that pseudo-random data MUST be used in place of
the MAC address.  
The rest of the UUIDv1 format remains unchanged in
UUIDv6
- UUIDv7 - UUIDv7 introduces an entirely new time-based UUID bit layout
utilizing a variable length timestamp sourced from the widely
implemented and well known Unix Epoch timestamp source.  
The timestamp is broken into a 36-bit integer sections part, and is
followed by a field of variable length which represents the  
sub-second timestamp portion, encoded so that each bit from most to least
significant adds more precision
- UUIDv8 - UUIDv8 introduces a relaxed time-based UUID format that
caters to application implementations that cannot utilize UUIDv1,
UUIDv6, or UUIDv7.  
UUIDv8 also future-proofs this specification by allowing time-based UUID formats 
from timestamp sources that are not yet be defined.  
The variable size timestamp offers lots of
flexibility to create an implementation specific RFC compliant time-
based UUID while retaining the properties that make UUID great.

## Implementation status

RFC/Draft version implementation: **[01](https://datatracker.ietf.org/doc/html/draft-peabody-dispatch-new-uuid-format-01)**  
UUIDv6: **Not implemented, planned**  
UUIDv7: **Work in progres**  
UUIDv8: **Not planned**  

## Implementation details

### UUIDv7

Layout and bit order:

```text
bytes      	0               1               2               3
		    0                   1                   2                   3
		    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
		   |                            unixts                             |
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
bytes      	4               5               6               7
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
		   |unixts |       subsec_a        |  ver  |       subsec_b        |
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
bytes      	8               9               10              11
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
		   |var|                   subsec_seq_node                         |
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
bytes      	12              13              14              15
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
		   |                       subsec_seq_node                         |
		   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
```           

## Benchmarks

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=5.0.400
  [Host]   : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT  [AttachedDebugger]
  .NET 5.0 : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT

Job=.NET 5.0  Runtime=.NET 5.0  

```

|                                Method |       Mean |    Error |   StdDev | Ratio | RatioSD |
|-------------------------------------- |-----------:|---------:|---------:|------:|--------:|
| &#39;UUIDv7 new instance generation test&#39; | 1,642.8 ns | 17.82 ns | 15.79 ns | 12.36 |    0.17 |
|   &#39;GUID new instance generation test&#39; |   133.0 ns |  1.12 ns |  1.05 ns |  1.00 |    0.00 |

|                         Method |       Mean |    Error |    StdDev |     Median | Ratio | RatioSD |
|------------------------------- |-----------:|---------:|----------:|-----------:|------:|--------:|
| &#39;UUIDv7 parse byte array test&#39; | 3,302.2 ns | 77.95 ns | 218.57 ns | 3,300.0 ns | 23.91 |    8.29 |
|   &#39;GUID parse byte array test&#39; |   155.0 ns | 16.96 ns |  50.00 ns |   200.0 ns |  1.00 |    0.00 |

## Contribution

