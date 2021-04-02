# Asar history

This repo is a mirror of [the main Asar repository](https://github.com/RPGHacker/asar), but with some extra commits spliced onto the start of the Git history, corresponding to releases of Asar from before we used Git. This is useful for getting a bit more context on old `git blame`s, or just digging around in old code.

Update process to sync with new commits in Asar:

* clone RPGHacker:master to the `main` branch (might need some amount of --force)
* `git replace --graft b6139957 old-releases` (first commit of the real tree, grafted on to the last commit of the fabricated tree)
* `git checkout main`
* `git filter-branch`
* Redo the readme edit

Update process to add another old version of Asar to the tree:

* Checkout the commit right before where the new version would go (or `git checkout --orphan random-branch-name` to add one before the first), also write down the commit right after the new version
* Delete everything in the working directory
* Copy over the source from whichever zip you got it from
* `git add .`
* `git commit --author "Author Name" --date "timestamp in ISO8601" --no-gpg-sign` (if this author hasn't commited anything to asar yet, you also need to provide an email, like `"Name <email@example.com>"`, if Git has seen this name before then it can figure the email out by itself)
* `git replace --graft <next commit hash> HEAD` (use the commit hash that's supposed to be right after the inserted commit - i told you to write it down)
* `git checkout main`
* `git filter-branch --env-filter 'export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"'`
* make sure the `old-releases` branch points to the right commit (not sure if filter-branch will get this right)
* force-push the new `main` and `old-releases` branches

# Asar
[![Travis build](https://travis-ci.org/RPGHacker/asar.svg?branch=master)](https://travis-ci.org/RPGHacker/asar) [![Appveyor build](https://ci.appveyor.com/api/projects/status/github/RPGHacker/asar?svg=true)](https://ci.appveyor.com/project/RPGHacker/asar)

Asar is an SNES assembler designed for applying patches to existing ROM images, or creating new ROM images from scratch. It supports 65c816, SPC700, and Super FX architextures. It was originally created by Alcaro, who based it on [xkas v0.06](https://www.romhacking.net/utilities/269/) by byuu.

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
*Please note that these tests are intended for use with Asar's test suite. Only contributors will need to use this functionality - people who just want to create and apply patches don't need to worry about it.*

At the beginning of your ASM files, you can write tests to ensure the correct values were written to the ROM after patching is complete. (It's common to use a SMW ROM, but there's also a dummy ROM included that should work with all tests.)

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
