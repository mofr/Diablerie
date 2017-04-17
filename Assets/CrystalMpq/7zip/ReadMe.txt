The files in this directory (the "7zip" directory) are a *slightly modified* subset of the LZMA SDK 9.20:
Only the part of the LZMA SDK required for CrystalMpq to work, and all the classes are marked as internal.
Some XML comments have also been fixed in order not to generate any warning.

Since I am not the developper of the LZMA SDK, I claim no copyright on those files derived from the LZMA SDK.
The original license (public domain, see below) still applies to these files.

I do not think there is any value in those files (other than being used by CrystalMpq) over the original SDK.
Consider visiting the official website at http://www.7-zip.org/sdk.html for downloading the full package of the latest LZMA SDK if you need it.


Excerpt from lzma.txt in LZMA SDK 9.20:

LZMA SDK 9.20
-------------

LZMA SDK provides the documentation, samples, header files, libraries, 
and tools you need to develop applications that use LZMA compression.

LZMA is default and general compression method of 7z format
in 7-Zip compression program (www.7-zip.org). LZMA provides high 
compression ratio and very fast decompression.

LZMA is an improved version of famous LZ77 compression algorithm. 
It was improved in way of maximum increasing of compression ratio,
keeping high decompression speed and low memory requirements for 
decompressing.



LICENSE
-------

LZMA SDK is written and placed in the public domain by Igor Pavlov.

Some code in LZMA SDK is based on public domain code from another developers:
  1) PPMd var.H (2001): Dmitry Shkarin
  2) SHA-256: Wei Dai (Crypto++ library)
