# PythonStructCS

A quick port of the essential features of [python's struct library](https://docs.python.org/3/library/struct.html) to C#

PyStruct.pack(string format, List\<object> data) -- returns a byte array (byte[]) with the packed data
PyStruct.unpack(string format, byte[] data) -- returns an object List (List\<object>) with the unpacked data
PyStruct.calcSize(string format) -- returns the byte size of the given format

This library is not meant to be exactly the same as the original library, nor is it even meant to maintain 100% compatibility, its a single "good enough" class for my personal use.

## Formats
A format should be a string starting with the endian of the data, followed by specifier characters, prefixed with their count (or size),
for example:

\<32s4i

is interpreted as little endian (Least significant Byte first),
a 32 byte long string (as strings are variable length),
and 4 integers (as integers are all 4 bytes)

## Endianness

In the original Python struct library there were 5 supported endian characters:
- @ - Native Endian with native sizing
- = - Native Endian with standard sizing
- < - Little Endian with standard sizing
- \> - Big Endian with standard sizing
- ! - Network (Big Endian with standard sizing)

only < and > are *actually* implemented in this library:

@, and = are aliases for <,
and ! is an alias for >

this should produce the same results in the vast majority of cases.

## Supported format specifiers

the majority of this documentation is copied straight from the [previously linked python page](https://docs.python.org/3/library/struct.html)

### 1 byte

- x - Pad Byte
- c - Character (Char) 
- b - Signed Byte (SByte) (signed character in python)
- B - Unsigned Byte (Byte) (unsigned character in python)
- ? - Boolean
- e - Half

### 2 bytes

- h - short (Int16)
- H - unsigned short (UInt16)

### 4 bytes
- i - Integer (Int32)
- I - Unsigned Integer (UInt32)
- l - Long (Int32)
- L - Unsigned Long (UInt32)
- f - Float (Single)
- d - Double

### 8 bytes
- q - Long Long (Int64)
- Q - Unsigned Long Long (UInt64)

### Variable
- s (String)

## (Currently) Unsupported format specifiers
- n ssize_t
- N size_t 
- p char[] (Pascal string)
- P void*
