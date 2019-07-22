# Asar
[![Travis build](https://travis-ci.org/RPGHacker/asar.svg?branch=master)](https://travis-ci.org/RPGHacker/asar) [![Appveyor build](https://ci.appveyor.com/api/projects/status/github/RPGHacker/asar?svg=true)](https://ci.appveyor.com/project/RPGHacker/asar)

Asar is a SNES assembler designed for applying patches to existing ROM images, or creating new ROM images from scratch. It supports 65c816, SPC700, and Super FX architextures. It was originally created by Alcaro, who based it on [xkas v0.06](https://www.romhacking.net/utilities/269/) by byuu.

For a guide on using Asar (including how to write patches), see [`README.txt`](https://github.com/RPGHacker/asar/blob/master/README.txt). This readme was made with tool programmers and contributors in mind.

## Building
You can build Asar with [CMake](https://cmake.org). On Linux, the most basic build would look like `cmake src && make`. On Windows, using Visual Studio, you would do `cmake src`, then open the project file it generates in Visual Studio and click Build. Alternately, you might be able to use Visual Studio's [CMake integration](https://docs.microsoft.com/en-us/cpp/build/cmake-projects-in-visual-studio).

If you'd rather not build from source, check out the [Releases](https://github.com/RPGHacker/asar/releases) page.

## Asar DLL
Asar can also be built as a DLL. This makes it easier and faster to use in other programs (such as a sprite insertion tool). You can find documentation on the DLL API in the respective bindings (asardll.h, asar.cs, asar.py).

## Folder layout
* `docs` contains the source of the manual and changelog.
  (You can view an online version of the manual [here](https://rpghacker.github.io/asar/manual/) and an online version of the changelog [here](https://rpghacker.github.io/asar/changelog/)).
* `ext` contains syntax highlighting files for Notepad++ and Sublime Text
* `src`
  * `asar` contains the source code of the main app and DLL
  * `asar-tests` contains code for the testing application (both the app test and DLL test)
  * `asar-dll-bindings` contains bindings of the Asar DLL to other languages (currently C/C++, C# and Python)
* `tests` contains tests to verify Asar works correctly

## Test format
At the beginning of your asm files, you can write tests to ensure the correct values were written to the ROM after patching is complete. (Intended for SMW, but there's also a dummy ROM included that should work with all tests.)

These two characters should precede each test line, so that Asar sees them as comments and ignores them.
```
;`
```

* 5-6 hex digits - the ROM offset to check 
  * Specify it as a PC address, not a SNES address
  * When left blank, it defaults to `0x000000`
* 2 hex digits - a byte for it to check for 
  * You can specify more than one, like in the examples below, and it will automatically increment the offset.
* A line starting with `+` tells the testing app to patch the SMW ROM instead of creating a new ROM
* `errEXXXX` and `warnWXXXX` (where `XXXX` is an ID number) means that the test is expected to throw that specific error or warning while patching. The test will succeed if only these errors and warnings are thrown.

**Example tests:**

This line tests that the bytes `5A`, `40` and `00` (in that order) were written to the start of the ROM.
```
;`5A 40 00
```

This line tests that `22`, `20`, `80` and `90` were written to the ROM offset `0x007606`.
```
;`007606 22 20 80 90
```
