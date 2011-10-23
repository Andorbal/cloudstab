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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rackspace.CloudFiles;
using Rackspace.CloudFiles.Domain;
using Rhino.Mocks;
using cloudstab.core.Exceptions;

namespace cloudstab.rackspace.tests {
  [TestFixture]
  public class RackspaceContainerManagerTests {
    [Test]
    public void List_WithValidConnection_ReturnsAllContainers() {
      // Arrange
      var connection = MockRepository.GenerateStub<IConnection>();
      connection.Stub(x => x.GetContainers()).Return(new List<string> {"foo", "bar"});
      var manager = new RackspaceContainerManager(connection);

      // Act
      var containers = manager.List();

      // Assert
      Assert.IsTrue(containers.Where(x => x.Name == "foo").Any());
      Assert.IsTrue(containers.Where(x => x.Name == "bar").Any());
    }

    [Test]
    public void Get_WithInvalidName_ThrowsInvalidNameException() {
      // Arrange
      var manager = new RackspaceContainerManager(MockRepository.GenerateStub<IConnection>());

      // Act & Assert
      Assert.Throws<InvalidNameException>(() => manager.Get(null));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Get_WithValidName_ReturnsContainer(string name) {
      // Arrange
      var container = MockRepository.GenerateStub<IContainer>();
      container.Stub(x => x.Name).Return(name);
      var account = MockRepository.GenerateStub<IAccount>();
      account.Stub(x => x.ContainerExists(name)).Return(true);
      account.Stub(x => x.GetContainer(name)).Return(container);
      var connection = MockRepository.GenerateStub<IConnection>();
      connection.Stub(x => x.Account).Return(account);

      var manager = new RackspaceContainerManager(connection);

      // Act
      var result = manager.Get(name);

      // Assert
      Assert.That(result.Name, Is.EqualTo(name));
    }

    [TestCase("foo"), TestCase("bar")]
    public void Get_NonExistentContainer_ReturnsNull(string name) {
      // Arrange
      var account = MockRepository.GenerateStub<IAccount>();
      account.Stub(x => x.ContainerExists(name)).Return(false);
      var connection = MockRepository.GenerateStub<IConnection>();
      connection.Stub(x => x.Account).Return(account);

      var manager = new RackspaceContainerManager(connection);

      // Act
      var result = manager.Get(name);

      // Assert
      Assert.That(result, Is.Null);
    }
  }
}
