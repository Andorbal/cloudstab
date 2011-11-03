using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;
using cloudstab.core.Exceptions;
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
using NUnit.Framework;

namespace cloudstab.core.tests {
  [TestFixture]
  public class ContainerManagerTests {
    [Test]
    public void List_CallsListMethodOnProvider() {
      // Arrange
      var provider = MockRepository.GenerateMock<IBlobContainerManager>();
      provider.Expect(x => x.List()).Return(new List<IBlobContainer>());
      var testManager = new ContainerManager(provider);

      // Act
      testManager.List();

      // Arrange
      provider.AssertWasCalled(x => x.List());
    }

    [Test]
    public void List_ReturnsWrappedContainers() {
      // Arrange
      var container1 = MockRepository.GenerateStub<IBlobContainer>();
      var container2 = MockRepository.GenerateStub<IBlobContainer>();
      var provider = MockRepository.GenerateStub<IBlobContainerManager>();
      provider.Stub(x => x.List()).Return(new[] {container1, container2});
      var testManager = new ContainerManager(provider);

      // Act
      var results = testManager.List();

      // Assert
      Assert.That(results.Count(), Is.EqualTo(2));
      Assert.That(results.Select(x => x.Provider), Has.Member(container1));
      Assert.That(results.Select(x => x.Provider), Has.Member(container2));
    }

    [TestCase(null), TestCase("")]
    public void Create_WithInvalidName_ThrowsInvalidNameException(string name) {
      // Arrange
      var testManager = new ContainerManager(null);

      // Act & Assert
      Assert.Throws<InvalidNameException>(() => testManager.Create(name));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Create_WithValidName_CallsCreateOnProvider(string name) {
      // Arrange
      var provider = MockRepository.GenerateMock<IBlobContainerManager>();
      provider.Expect(x => x.Create(name)).Return(MockRepository.GenerateStub<IBlobContainer>());
      var testManager = new ContainerManager(provider);

      // Act
      testManager.Create(name);

      // Assert
      provider.AssertWasCalled(x => x.Create(name));
    }

    [Test]
    public void Create_WithValidName_ReturnsWrappedProviderContainer() {
      // Arrange
      var providerContainer = MockRepository.GenerateStub<IBlobContainer>();
      var provider = MockRepository.GenerateStub<IBlobContainerManager>();
      provider.Stub(x => x.Create(null)).IgnoreArguments().Return(providerContainer);
      var testManager = new ContainerManager(provider);

      // Act
      var result = testManager.Create("foo");

      // Assert
      Assert.That(result.Provider, Is.EqualTo(providerContainer));
    }

    [TestCase(""), TestCase(null)]
    public void Get_WithInvalidName_ThrowsInvalidNameException(string name) {
      // Arrange
      var testManager = new ContainerManager(null);

      // Act
      Assert.Throws<InvalidNameException>(() => testManager.Get(name));
    }

    [TestCase(""), TestCase(null)]
    public void Delete_WithInvalidName_ThrowsInvalidNameException(string name) {
      // Arrange
      var testManager = new ContainerManager(null);

      // Act
      Assert.Throws<InvalidNameException>(() => testManager.Delete(name));
    }
  }
}
