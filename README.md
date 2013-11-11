CBOR.NET
========

.NET Implementation of CBOR (RFC 7049)

GOAL
========
* Produce a standards compliant Encoder and Decoder library for the CBOR data format.
* Decoder should produce native .NET classes where possible*

*C# does not provide an 'undefined' type, which is specified in CBOR as something different than 'null'.
Status
========
* All major types have been accounted for and are processed into their C# native types.
* Tags are being consumed but currently nothing is done with them

Current Work
========
* Create a test suite of various CBOR encodings and verify Decoder works properly
* Begin supporting Tags, Positive BigIntegers will the be first Tag to have decoding support

Future
========
* R&D a better OO obstraction for Item Headers, current implementation while usable is *not* intuitive


