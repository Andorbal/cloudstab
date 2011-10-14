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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemWrapper.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace cloudstab.filesystem.tests {
  [TestFixture]
  class FileSystemContainerManagerTests {
    [TestCase("foo"), TestCase("bar")]
    public void Constructor_WithValidPath_ChecksRootPathExists(string rootPath) {
      // Arrange
      var testDirectory = MockRepository.GenerateMock<IDirectoryWrap>();
      testDirectory.Expect(x => x.Exists(rootPath)).Return(true);

      // Act
      new FileSystemContainerManager(rootPath, testDirectory);

      // Assert
      testDirectory.AssertWasCalled(x => x.Exists(rootPath));
    }

    [Test]
    public void Constructor_RootPathDoesNotExist_CreatesPath() {
      // Arrange
      var testDirectory = MockRepository.GenerateMock<IDirectoryWrap>();
      testDirectory.Stub(x => x.Exists(null)).IgnoreArguments().Return(false);
      testDirectory.Expect(x => x.CreateDirectory("foo"));

      // Act
      new FileSystemContainerManager("foo", testDirectory);

      // Assert
      testDirectory.AssertWasCalled(x => x.CreateDirectory("foo"));
    }

    [Test]
    public void Constructor_RootPathExists_DoesNotCreatePath() {
      // Arrange
      var testDirectory = MockRepository.GenerateMock<IDirectoryWrap>();
      testDirectory.Stub(x => x.Exists(null)).IgnoreArguments().Return(true);
      testDirectory.Expect(x => x.CreateDirectory("foo"));

      // Act
      new FileSystemContainerManager("foo", testDirectory);

      // Assert
      testDirectory.AssertWasNotCalled(x => x.CreateDirectory("foo"));
    }

    [TestCase("foo"), TestCase("bar")]
    public void List_ReturnsDirectoryListing(string rootPath) {
      // Arrange
      var mockedDirectory = MockedDirectory();
      mockedDirectory.Stub(x => x.GetDirectories(rootPath)).Return(new[] {"a", "b", "c"});
   
      var testManager = new FileSystemContainerManager(rootPath, mockedDirectory);

      // Act
      var results = testManager.List();

      // Assert
      Assert.That(results.Count(), Is.EqualTo(3));
      Assert.IsTrue(results.OfType<FileSystemContainer>().Any(x => x.DirectoryPath == "a"));
      Assert.IsTrue(results.OfType<FileSystemContainer>().Any(x => x.DirectoryPath == "b"));
      Assert.IsTrue(results.OfType<FileSystemContainer>().Any(x => x.DirectoryPath == "c"));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Create_WithValidName_CreatesDirectory(string newDirectory) {
      // Arrange
      var mockedDirectory = MockedDirectory();
      mockedDirectory.Expect(x => x.CreateDirectory(newDirectory));
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      testManager.Create(newDirectory);

      // Assert
      mockedDirectory.AssertWasCalled(x => x.CreateDirectory(newDirectory));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Create_WithValidName_ReturnsCreatedContainer(string newDirectory) {
      // Arrange
      var mockedDirectory = MockedDirectory();
      mockedDirectory.Stub(x => x.CreateDirectory(newDirectory));
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      var result = testManager.Create(newDirectory);

      // Assert
      Assert.That(((FileSystemContainer) result).DirectoryPath, Is.EqualTo(newDirectory));
    }

    private IDirectoryWrap MockedDirectory() {
      var testDirectory = MockRepository.GenerateMock<IDirectoryWrap>();
      testDirectory.Stub(x => x.Exists(null)).IgnoreArguments().Return(true);
      return testDirectory;
    }
  }
}
