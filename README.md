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
* A handful of tags are being processed, still more to implement though.

Current Work
========
* Continue implementing tags.

Future
========
* R&D a better OO obstraction for Item Headers, current implementation while usable is *not* intuitive


