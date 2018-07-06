# C# Vector<T\>

 A few (NET CORE 2.1) benchmarks for the _System.Numerics.Vectors_ **Vector<T\>** and  **Vector** classes.

The ` SIMDBenchmarks` project runs some simple benchmarking on integer and floating point vectors and a [Mandelbrot](https://en.wikipedia.org/wiki/Mandelbrot_set) computation with floats and `Vector<float>`. It can easily be adapted for your own measuring.

## What you need

If you develop for NET Core nothing extra is needed (the project included here is written against Net Core 2.1).

**To run the benchmarks** you need to add [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) (either by building the source or by adding a [NuGet package](https://www.nuget.org/packages/BenchmarkDotNet/) to your project).

In case you want to run on the .NET framework you´ll need the NuGet package [System.Numerics.Vectors](https://www.nuget.org/packages/System.Numerics.Vectors/).

In both environments you´ll need to compile 64 bit applications and add a `using Systems.Numerics` to your code.

> Note: under Visual Studio 2017 (up to version 15.7.4) there is a reported bug concerning the debug view of `Vector<T>` (hovering over a variable will only show the first half of its elements, the rest will display as zeroes)

## Introduction to Vector<T\>

The Vector and Vector<T\> classes open up the possibility to use SIMD (Single Instruction, Multiple Data) instruction set (SSE, AVX) when programming. See the [documentation](https://msdn.microsoft.com/en-us/library/dn858385) for a full list of functions.

It basically allows you to perform one calculation on multiple numbers in parallel. This is achieved by creating _Vectors_ of some numeric type and using operators  on them. One advantage of using .NET (over C / C++) for example is that the Vector<T\> abstracts the underlying hardware so your code will take advantage of whatever processor (and therefore vector width and instructions) is there.

The number of elements you can place in a Vector will depend on the hardware: AVX2 offers a 256 bit wide number for example, meaning that a `Vector<double>` can contain 4 double values.

As far as I understand the source for [RyuJit](https://github.com/dotnet/coreclr/blob/master/src/jit/hwintrinsiclistxarch.h) there is no support for AVX-512 and AVX / AVX2 are identified as [partially supported](https://github.com/dotnet/coreclr/blob/master/src/jit/hwintrinsicxarch.cpp).

Notice though that vectorizing a calculation **does not mean** automatic performance gains. Benchmarking is required to make sure that using vectors makes sense. A typical example is working with `Vector<long>`: since the underlying hardware does not support multiplying (or dividing) integers element per element, vectorizing will possibly _reduce_ performance due to vector overhead (Intel provides a [full list](https://software.intel.com/sites/landingpage/IntrinsicsGuide/) of available SIMD instructions).

Vector<T\> can contain any numeric type (`sbyte, byte, short, ushort, int, uint, long, ulong, float, double`).

##### Content

[Creating Vectors](#creating-vectors)  
[Retrieving Values](#retrieving-values-from-vectors)  
[Comparing Vectors](#comparing-vectors)  
[Conditional Selection](#conditional-selection)  
[Conversions](#conversions)  
[Bitwise Operations](#bitwise-operations)  
[Math Operations](#math-operations)

## Creating Vectors

##### Same value repeated

```
using System;
using System.Numerics;

[...]
//Results in a AVX / AVX 2 system
Vector<double> douZero = Vector<double>.Zero;// douZero.ToString() shows: <0, 0, 0, 0>
Vector<float> flOne = Vector<float>.One; // <1, 1, 1, 1, 1, 1, 1, 1>
Vector<ushort> shAny = new Vector<ushort>(43);
```
The above instructions in a AVX / AVX2 capable system will create vectors containing 4 repeated doubles, 8 repeated floats and 16 repeated ushorts.

##### Different values
```
[...]

double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
Span<double> douSpan= new Span<double>(doubArray, 8, 4);

//Results in a AVX / AVX 2 system
Vector<double> douV = new Vector<double>(doubArray); //Will contain <1, 2, 3, 4>
Vector<double> spanduoV = new Vector<double>(douSpan); //Will contain <-1, -2, -3, -4>
Vector<double> dou2V = new Vector<double>(doubArray, 5); //Will contain <3, 2, 1, -1>
Vector<double> sumV = douV + dou2V; //Will contain <4, 4, 4, 3>
```
Vector can be created from Arrays, Span<T\> or as a result from operations.

##### Programming for different hardware platforms

As mentioned above, .NET presents the advantage to abstract the underlying hardware. This means that you don´t have to worry if the system has a 64, 128, 256 or 512 bit vector implementation.
```
[...]

if(Vector.IsHardwareAccelerated == false)
{
  //fallback to some other code;
  return;
}

int bitWidth = Vector<byte>.Count * 8; // bitWidth will contain the available vector size in bits

int floatSlots = Vector<float>.Count;// floatSlots will contain the number of floats per vector;

int i;
for(i = 0; i < myData.Length; i += floatSlots)
{
  Vector<double> someData = new Vector<double>(data, i);
  //do some useful stuff
}
for(; i < myData.Length; i++)
{
  //handle data left over (if any) in serial fashion
}
```
`Vector.IsHardwareAccelerated` tells you if the hardware supports the functionality and gives you the chance to bail out or choose a different processing function.

The `Count` property of a specific Vector<T> type tells you how many numbers of type T can be handled simultaneously by a vector and allows you to process your data in sizeable chunks.

The examples below assume a 256 bit vector for simplicity and brevity.

## Retrieving values from Vectors

```
[...]
//Results in a AVX / AVX 2 system

double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };

Vector<double> douV = new Vector<double>(doubArray); //Will contain <1, 2, 3, 4>
Vector<double> dou2V = new Vector<double>(doubArray, 5); //Will contain <3, 2, 1, -1>
Vector<double> sumV = douV + dou2V; //Will contain <4, 4, 4, 3>

double[] sumArray = new double[48]; //Will hold several result values

sumV.CopyTo(sumArray); //Copies the vector values to start of array
sumV.CopyTo(sumArray, 8); //Copies the vector values to array starting at index 8

double lastVal = sumV[3]; //Retrieves the 4th value from vector, read only!
```
You access the values in a Vector by either copying all values to an array, or by accessing its components individually, as if the vector were an array.

Notice that you cannot write `sumV[3] = 4.0` since the elements are read only. Actually, `Vector<T>` as a whole is immutable and every new assignment creates a new instance.

## Comparing Vectors

##### Comparisons with Vector results

```
[...]
//Results in a AVX / AVX 2 system

double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
float[] flArray = new float[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
ushort[] ushArray = new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8, 8, 7, 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4 };
ulong[] ulARray = new ulong[] { 1, 2, 3, 4, 4, 3, 2, 1 };

Vector<double> left = new Vector<double>(doubArray); //<1, 2, 3, 4>
Vector<double> right = new Vector<double>(doubArray, 4); //<4, 3, 2, 1>
Vector<float> leftFl = new Vector<float>(flArray); //<1, 2, 3, 4, 4, 3, 2, 1>
Vector<float> rightFl = new Vector<float>(flArray, 4); //<4, 3, 2, 1, -1, -2, -3, -4>
Vector<ushort> leftUSh = new Vector<ushort>(ushArray); //<1, 2, 3, 4, 5, 6, 7, 8, 8, 7, 6, 5, 4, 3, 2, 1>
Vector<ushort> rightUSh = new Vector<ushort>(ushArray, 2); //<3, 4, 5, 6, 7, 8, 8, 7, 6, 5, 4, 3, 2, 1, 0, 1>
Vector<ulong> leftUl = new Vector<ulong>(ulARray);
Vector<ulong> rightUl = new Vector<ulong>(ulARray, 4);

Vector<long> lessThan = Vector.LessThan(left, right);//lessThan = <-1, -1, 0, 0>
Vector<int> lessThanInt = Vector.LessThan(leftFl, rightFl); //lessThanInt = <-1, -1, 0, 0, 0, 0, 0, 0>
Vector<ushort> greaterEqual = Vector.GreaterThanOrEqual(leftUSh, rightUSh); //greaterEqual = <0, 0, 0, 0, 0, 0, 0, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535>
Vector<ulong> lessThanul = Vector.LessThan(leftUl, rightUl);// lessTahUl = <18446744073709551615, 18446744073709551615, 0, 0>
```
A comparison between _double_ vectors results in a _long_ vector; _float_ comparisons give _int_ results.

All vector comparisons of type T _integers_ will result in a vector T result.

Although the documentation says that comparisons will return **one** if true, the actual return is a value with all bits on: for _ushort_, a successful comparison will result in 0xFFFF (65535 decimal), for _long_ the result will be 0xFFFF FFFF FFFF FFFF (-1 decimal) etc. This is only relevant if you plan to use the result as a number.

##### Comparisons with boolean results

```
[...]
//Results in a AVX / AVX 2 system

double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
Vector<double> allPositives = new Vector<double>(doubArray); //<1, 2, 3, 4>
Vector<double> someNegatives = new Vector<double>(doubArray, 6); //<2, 1, -1, -2>

bool allAreNegative = Vector.LessThanAll(allPositives, Vector<double>.Zero);//allAreNegative = false
bool allArePositive = Vector.GreaterThanAll(allPositives, Vector<double>.Zero);//allArePositive = true;
bool someAreNegative = Vector.LessThanAny(someNegatives, Vector<double>.Zero);//someAreNegative = true;
```
These comparisons allow a quick verification over the whole vector either applying to _All_ elements or to _Any_ of them.

##### Equality

```
[...]
//Results in a AVX / AVX 2 system

double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };

Vector<double> left = new Vector<double>(doubArray); //<1, 2, 3, 4>
Vector<double> right = new Vector<double>(doubArray, 1); //<2, 3, 4, 5>

if(left == right)
{
    // every element in left is equal to its corresponding element in right
}
if(left != right)
{
    //at least one element pair is different
}
```
The "==" operator is equivalent to `Vector.EqualsAll()`, "!=" is its logical negation, as expected.

## Conditional selection
```
[...]
//Results in a AVX / AVX 2 system

double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
Vector<double> left = new Vector<double>(doubArray); //<1, 2, 3, 4>
Vector<double> right = new Vector<double>(doubArray, 4); //<4, 3, 2, 1>

Vector<long> lessThan = Vector.LessThan(left, right);//lessThan = <-1, -1, 0, 0>
Vector<double> select = Vector.ConditionalSelect(lessThan, left, right);// select = <1, 2, 2, 1>
Vector<double> select2 = Vector.ConditionalSelect(lessThan, left, Vector<double>.Zero);// select2 = <1, 2, 0, 0>
```
Conditional selection permits you to combine two vectors based on conditions. In the code above, the line  
`Vector<double> select = Vector.ConditionalSelect(lessThan, left, right);`  
results in a vector with elements from `left` if they were less than the corresponding elements in `right`. For elements for which the comparison returned `false` (value zero), the result will be populated with elements from `right`.

Notice that the line  
`Vector<double> select2 = Vector.ConditionalSelect(lessThan, left, Vector<double>.Zero)`  
uses the result from the comparison to select elements from _another vector_: it will fill in zeros for elements that were _greater than or equal_ in the original comparison.

In other words, the result from comparisons can be used as a mask for vector composition.

## Conversions

There are three ways to transform from one vector type to another: conversion, reinterpretation and widen/narrow.

##### Conversions

Conversions _transform_ the values of a vector of type T into a new vector of type U.

```
[...]
//Results in a AVX / AVX 2 system

Vector<double> minOne = new Vector<double>(-1);//<-1, -1, -1, -1>
Vector<long> convToLong = Vector.ConvertToInt64(minOne);//<-1, -1, -1, -1>
```
You can convert vectors according to the following list. Notice that the bit size of the element type needs to be the same:  
**From**  `long` and `ulong` **to** `double`  
**From**  `int` and `uint` **to** `float`  
**From**  `double` **to** `long`, `ulong`  
**From**  `float` **to** `int`, `uint`  

##### Reinterpretations

Reinterpretations will treat the memory contents of a` Vector<T>` as if it were a Vector<U\>.

```
[...]
//Results in a AVX / AVX 2 system

Vector<int> convInt = Vector.ConvertToInt32(Vector<float>.One);// convInt = <1, 1, 1, 1, 1, 1, 1, 1>
Vector<int> asInt = Vector.AsVectorInt32(Vector<float>.One);
//asInt = <1065353216, 1065353216, 1065353216, 1065353216, 1065353216, 1065353216, 1065353216, 1065353216>
Vector<byte> asBytes = Vector.AsVectorByte(Vector<float>.One);
//asBytes = <0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f>
```
Notice the very different result in _convInt_ and _asInt_: the Vector.AsVectorxxx operation looks at the bytes stored at the vector´s location and composes the result´s elements from them.

The `Vector.AsVectorByte()` function simply lists all bytes in a given vector. Notice, though, that for little-endian systems (all Intel platforms for example) the _order_ of the bytes is inverted: the (hex) representation of the float number "1" is "0x3F800000", the memory order in little-endian machines is inverted.

##### Widen / narrow
 ```
 [...]
//Results in a AVX / AVX 2 system

 short[] shArray = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
Vector<short> sh = new Vector<short>(shArray);

Vector<int> low = new Vector<int>();
Vector<int> high = new Vector<int>();

Vector.Widen(sh, out low, out high);//low = <0, 1, 2, 3, 4, 5, 6, 7>             
 ```
The `Vector.Widen()` operation will create _two_ vectors out of one: in the example above, the lower half of vector `sh`  is copied into `Vector<int> low`, the rest into vector `high`.

Conversely, `Vector.Narrow()` will create _one_ vector out of two.

The widen / narrow refers to the bit width of the elements (i. e. in the first example you _widen_ the elements from 16 bit shorts to 32 bit ints).


## Bitwise operations
```
[...]

Vector<byte> byteF0 = new Vector<byte>(0b11110000);
Vector<byte> onesComp = Vector.OnesComplement(byteF0);//onesComp = <15, 15, ...> = <0b00001111, ...>
```
There are several bitwise operators available as Part of the VectorClass: OnesComplement, BitwiseAnd etc. Sadly no shifting operators have been included.

## Math operations
```
[...]
//Results in a AVX / AVX 2 system

double[] douArray = new double[] { 1, 3, 5, 7, 2, 4, 6, 8, 9, 11, 13, 15, 10, 12, 14, 16 };

Vector<int> Four = new Vector<int>(4);
Vector<double> aVector = new Vector<double>(douArray); //<1, 3, 5, 7>
Vector<double> otherVector = new Vector<double>(douArray, 4);// <2, 4, 6, 8>

Vector<double> resVDou = aVector * otherVector;// resVDou = <2, 12, 30, 56>
double resDou = Vector.Dot(aVector, otherVector);// resDou = 2 + 12 + 30 + 56 = 100
Vector<int> resVInt = Vector.SquareRoot(Four);// resVint = <2, 2, 2, 2, 2, 2, 2, 2>
resVDou = Vector.Max(aVector, new Vector<double>(4));// resVDou = <4, 4, 5, 7>
resVDou = Vector.Negate(resVDou);//resVDou = <-4, -4, -5, -7>
Vector<ushort> resVUSort = Vector.Negate(Vector<ushort>.One);
// resVUShort = <65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534>

```
The math operations `Vector.Add(), .Subtract(), .Multiply(), .Divide()` can be shorthanded using the common +, -, \* and / symbols. All math operations operate element by element.

`Vector.Negate()` will provide (-1 * each_element) for signed numbers. For unsigned integrals, the result will be(as in C) the unsigned integral type´s maximum value - element value + 1.

As said above, _not all operations are hardware supported for all Vector<T\> types_, so make sure to benchmark your code to confirm that performance is actually increasing by vectorizing.
