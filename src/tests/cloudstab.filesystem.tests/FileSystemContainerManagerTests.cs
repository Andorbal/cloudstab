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
using System.IO;
using System.Linq;
using cloudstab.core.Exceptions;
using NUnit.Framework;
using Rhino.Mocks;
using SystemWrapper.IO;

namespace cloudstab.filesystem.tests {
  [TestFixture]
  public class FileSystemContainerManagerTests {
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
      mockedDirectory.Stub(x => x.GetDirectories(rootPath)).Return(new[] { "a", "b", "c" });

      var testManager = new FileSystemContainerManager(rootPath, mockedDirectory);

      // Act
      var results = testManager.List();

      // Assert
      Assert.That(results.Count(), Is.EqualTo(3));
      Assert.IsTrue(results.OfType<FileSystemContainer>().Any(x => x.DirectoryPath == "a"));
      Assert.IsTrue(results.OfType<FileSystemContainer>().Any(x => x.DirectoryPath == "b"));
      Assert.IsTrue(results.OfType<FileSystemContainer>().Any(x => x.DirectoryPath == "c"));
    }

    [TestCase(""), TestCase(null), TestCase(" "), TestCase("\t")]
    public void Create_WithEmptyName_ThrowsInvalidNameException(string directoryName) {
      // Arrange
      var testManager = new FileSystemContainerManager("test", MockedDirectory());

      // Act & Assert
      Assert.Throws<InvalidNameException>(() => testManager.Create(directoryName));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Create_WithValidName_CreatesDirectory(string newDirectory) {
      // Arrange
      var expectedPath = "test" + Path.DirectorySeparatorChar + newDirectory;

      var mockedDirectory = MockedDirectory();
      mockedDirectory.Expect(x => x.CreateDirectory(expectedPath));
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      testManager.Create(newDirectory);

      // Assert
      mockedDirectory.AssertWasCalled(x => x.CreateDirectory(expectedPath));
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
      Assert.That(((FileSystemContainer)result).DirectoryPath, Is.EqualTo(newDirectory));
    }

    [TestCase(""), TestCase(null), TestCase(" "), TestCase("\t")]
    public void Delete_WithEmptyName_ThrowsInvalidNameException(string directoryName) {
      // Arrange
      var testManager = new FileSystemContainerManager("test", MockedDirectory());

      // Act & Assert
      Assert.Throws<InvalidNameException>(() => testManager.Delete(directoryName));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Delete_WithValidName_DeletesContainer(string directoryName) {
      // Arrange
      var expectedPath = "test" + System.IO.Path.DirectorySeparatorChar + directoryName;

      var mockedDirectory = MockedDirectory();
      mockedDirectory.Expect(x => x.Delete(expectedPath, true));
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      testManager.Delete(directoryName);

      // Assert
      mockedDirectory.AssertWasCalled(x => x.Delete(expectedPath, true));
    }
    
    [TestCase("foo"), TestCase("bar")]
    public void Delete_WithDirectoryName_ChecksProperPath(string directoryName) {
      // Arrange
      var expectedPath = "test" + Path.DirectorySeparatorChar + directoryName;

      var mockedDirectory = MockRepository.GenerateMock<IDirectoryWrap>();
      mockedDirectory.Expect(x => x.Exists(expectedPath)).Return(false);
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      testManager.Delete(directoryName);

      // Assert
      mockedDirectory.AssertWasCalled(x => x.Exists(expectedPath));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Delete_DirectoryDoesNotExist_DoesNotCallDelete(string directoryName) {
      // Arrange
      var mockedDirectory = MockedDirectory(false);
      mockedDirectory.Expect(x => x.Delete(directoryName, true));
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      testManager.Delete(directoryName);

      // Assert
      mockedDirectory.AssertWasNotCalled(x => x.Delete(directoryName, true));
    }

    [TestCase(null), TestCase("")]
    public void Get_WithInvalidName_ThrowsInvalidNameException(string name) {
      // Arrange
      var testManager = new FileSystemContainerManager("test", MockedDirectory());

      // Act & Assert
      Assert.Throws<InvalidNameException>(() => testManager.Get(name));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Get_WithExistingContainer_ReturnsSelectedContainer(string directoryName) {
      // Arrange
      var mockedDirectory = MockRepository.GenerateStub<IDirectoryWrap>();
      mockedDirectory.Stub(x => x.GetDirectories("test")).Return(new[] { "foo", "bar" });
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      var container = testManager.Get(directoryName);

      // Assert
      Assert.That(container.Name, Is.EqualTo(directoryName));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Get_WithNonExistentContainer_ReturnsNull(string directoryName) {
      // Arrange
      var mockedDirectory = MockRepository.GenerateStub<IDirectoryWrap>();
      mockedDirectory.Stub(x => x.GetDirectories("test")).Return(new[] { "foo", "bar" });
      var testManager = new FileSystemContainerManager("test", mockedDirectory);

      // Act
      var container = testManager.Get("baz");

      // Assert
      Assert.That(container, Is.Null);
    }



    private IDirectoryWrap MockedDirectory(bool exists = true) {
      var testDirectory = MockRepository.GenerateMock<IDirectoryWrap>();
      testDirectory.Stub(x => x.Exists(null)).IgnoreArguments().Return(exists);
      return testDirectory;
    }
  }
}
