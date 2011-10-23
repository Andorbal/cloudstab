/*
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
 */
using cloudstab.core.Exceptions;
using NUnit.Framework;

namespace cloudstab.core.tests {
  [TestFixture]
  public class BlobContainerUtilitiesTests {
    [TestCase(@"abc"), TestCase(@"abc1"), TestCase(@"ab-c")]
    public void EnsureValidContainerName_NameContainsOnlyValidCharacters_DoesNotThrowException(string name) {
      Assert.DoesNotThrow(() => BlobContainerUtilities.EnsureValidContainerName(name));
    }
    
    [Test]
    public void EnsureValidContainerName_NameBeginsWithNumber_DoesNotThrowException() {
      Assert.DoesNotThrow(() => BlobContainerUtilities.EnsureValidContainerName("2abc"));
    }

    [TestCase(@"ab!c"), TestCase(@"ab@c")]
    public void EnsureValidContainerName_NameContainsInvalidCharacter_ThrowsInvalidNameException(string name) {
      Assert.Throws<InvalidNameException>(() => BlobContainerUtilities.EnsureValidContainerName(name));
    }

    [Test]
    public void EnsureValidContainerName_NameIsNull_ThrowsInvalidNameException() {
      Assert.Throws<InvalidNameException>(() => BlobContainerUtilities.EnsureValidContainerName(null));
    }

    [Test]
    public void EnsureValidContainerName_NameBeginsWithDash_ThrowsInvalidNameException() {
      Assert.Throws<InvalidNameException>(() => BlobContainerUtilities.EnsureValidContainerName("-abc"));
    }

    [Test]
    public void EnsureValidContainerName_NameEndsWithDash_ThrowsInvalidNameException() {
      Assert.Throws<InvalidNameException>(() => BlobContainerUtilities.EnsureValidContainerName("abc-"));
    }

    [Test]
    public void EnsureValidContainerName_NameHasTwoConsecutiveDashes_ThrowsInvalidNameException() {
      Assert.Throws<InvalidNameException>(() => BlobContainerUtilities.EnsureValidContainerName("ab--c"));
    }

    [Test]
    public void EnsureValidContainerName_NameIsTwoCharacters_ThrowsInvalidNameException() {
      Assert.Throws<InvalidNameException>(() => BlobContainerUtilities.EnsureValidContainerName("ab"));
    }

    [Test]
    public void EnsureValidContainerName_NameIsTooLong_ThrowsInvalidNameException() {
      Assert.Throws<InvalidNameException>(() => BlobContainerUtilities.EnsureValidContainerName(new string('x', 64)));
    }
  }
}
