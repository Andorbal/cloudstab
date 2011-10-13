# CLOUD STorage ABstraction

The goal behind Cloudstab is to provide a set of interfaces that will allow access to and usage of basic cloud storage facilities.  The initial motivation behind this project is to allow applications built against Amazon's S3 to use local storage during development.  Because Azure already has facilities for this, I decided to make the interface generic enough to work against both S3 and Azure Blob storage which means that I will be able to leverage Azure's development infrastructure.

Creating an abstraction for multiple cloud providers' storage implementations means that I will only be able to support the lowest common denominator.  In this case, I think this will be enough as both Amazon and Microsoft have a concept of a container and an object.

## Please note:

I'm only just beginning this project so please don't take a dependency on it at the moment.  That said, I'm *more* that happy to take suggestions, criticism, pull requests, or anything else!

## License

Copyright (c) 2011, Andrew Benz
All rights reserved.

Redistribution and use in source and binary forms, with or without 
modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this 
list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, 
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
THE POSSIBILITY OF SUCH DAMAGE.
