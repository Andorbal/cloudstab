﻿/*
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
using cloudstab.core;
using cloudstab.core.Exceptions;
using NUnit.Framework;
using Rhino.Mocks;

namespace cloudstab.memory.tests {
  [TestFixture]
  public class MemoryContainerManagerTests {
    [Test]
    public void Constructor_WithNullStore_ThrowsArgumentNullException() {
      // Arrange, Act & Assert
      Assert.Throws<ArgumentNullException>(() => new MemoryContainerManager(null));
    }

    [TestCase(null), TestCase("")]
    public void Create_WithInvalidName_ThrowsInvalidNameException(string name) {
      // Arrange
      var testManager = new MemoryContainerManager();

      // Act & Assert
      Assert.Throws<InvalidNameException>(() => testManager.Create(name));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Create_WithValidName_CreatesNewContainer(string name) {
      // Arrange
      var store = new Dictionary<string, IBlobContainer>();
      var testManager = new MemoryContainerManager(store);

      // Act
      testManager.Create(name);

      // Assert
      Assert.That(store.Keys, Has.Member(name));
      Assert.That(store[name], Is.InstanceOf<IBlobContainer>());
    }

    [TestCase("foo"), TestCase("bar")]
    public void Create_WithValidName_ReturnsCreatedContainer(string name) {
      // Arrange
      var testManager = new MemoryContainerManager();

      // Act
      var container = testManager.Create(name);

      // Assert
      Assert.That(container.Name, Is.EqualTo(name));
    }

    [Test]
    public void Create_WithNameThatAlreadyExists_ReturnsExistingContainer() {
      // Arrange
      var testContainer = MockRepository.GenerateStub<IBlobContainer>();
      var store = new Dictionary<string, IBlobContainer> { { "foo", testContainer } };
      var testManager = new MemoryContainerManager(store);

      // Act
      var container = testManager.Create("foo");

      // Assert
      Assert.That(container, Is.EqualTo(testContainer));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Get_WithExistingName_ReturnsContainerWithSpecifiedName(string name) {
      // Arrange
      var store = new Dictionary<string, IBlobContainer> {
                                                           { "foo", MockRepository.GenerateStub<IBlobContainer>() }, 
                                                           { "bar", MockRepository.GenerateStub<IBlobContainer>() }
                                                         };
      var testManager = new MemoryContainerManager(store);

      // Act
      var container = testManager.Get(name);

      // Assert
      Assert.That(container, Is.EqualTo(store[name]));
    }

    [Test]
    public void Get_WithNonExistentName_ReturnsNull() {
      // Arrange
      var testManager = new MemoryContainerManager();

      // Act
      var container = testManager.Get("foo");

      // Assert
      Assert.That(container, Is.Null);
    }

    [TestCase(""), TestCase(null)]
    public void Get_WithInvalidName_ThrowsInvalidNameException(string name) {
      // Arrange
      var testManager = new MemoryContainerManager();

      // Act
      Assert.Throws<InvalidNameException>(() => testManager.Get(name));
    }

    [Test]
    public void List_WithNoContainers_ReturnsEmptyCollection() {
      // Arrange
      var testManager = new MemoryContainerManager();

      // Act
      var containers = testManager.List();

      // Assert
      Assert.That(containers, Is.Empty);
    }

    [Test]
    public void List_WithContainers_ReturnsAllContainers() {
      // Arrange
      var fooContainer = MockRepository.GenerateStub<IBlobContainer>();
      var barContainer = MockRepository.GenerateStub<IBlobContainer>();
      var store = new Dictionary<string, IBlobContainer> { 
          { "foo", fooContainer }, 
          { "bar", barContainer } 
      };
      var testManager = new MemoryContainerManager(store);

      // Act
      var containers = testManager.List();

      // Assert
      Assert.That(containers.Count(), Is.EqualTo(2));
      Assert.That(containers, Has.Member(fooContainer));
      Assert.That(containers, Has.Member(barContainer));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Delete_WithExistingContainer_ContainerIsDeleted(string name) {
      // Arrange
      var store = new Dictionary<string, IBlobContainer> { 
          { "foo", MockRepository.GenerateStub<IBlobContainer>() }, 
          { "bar", MockRepository.GenerateStub<IBlobContainer>() } 
      };
      var deletedContainer = store[name];
      var testManager = new MemoryContainerManager(store);

      // Act
      testManager.Delete(name);

      // Assert
      Assert.That(store, Has.No.Member(deletedContainer));
      Assert.That(store.Keys.Count(), Is.EqualTo(1));
    }

    [TestCase(""), TestCase(null)]
    public void Delete_WithInvalidName_ThrowsInvalidNameException(string name) {
      // Arrange
      var testManager = new MemoryContainerManager();

      // Act
      Assert.Throws<InvalidNameException>(() => testManager.Delete(name));
    }

    [Test]
    public void Delete_WithNonExistentContainer_NoExceptionIsThrown() {
      // Arrange
      var testManager = new MemoryContainerManager();

      // Act & Assert
      Assert.DoesNotThrow(() => testManager.Delete("foo"));
    }
  }
}
