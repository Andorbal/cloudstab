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
using System.Text.RegularExpressions;
using cloudstab.core.Exceptions;

namespace cloudstab.core {
  public class ContainerManager {
    private IBlobContainerManager _provider;
    
    public ContainerManager(IBlobContainerManager provider) {
      _provider = provider;
    }

    /// <summary>
    /// Lists all the containers in the store.
    /// </summary>
    /// <returns>A list of all the containers currently in the store.</returns>
    public IEnumerable<Container> List() {
      return _provider.List().Select(x => new Container(x));
    }

    /// <summary>
    /// Gets the container with the specified name.
    /// </summary>
    /// <param name="name">Name of the container to retrieve.</param>
    /// <returns>The container with the specified name, or null if it doesn't exist.</returns>
    public Container Get(string name) {
      EnsureValidContainerName(name);
      return new Container(_provider.Get(name));
    }

    /// <summary>
    /// Creates a new container if it doesn't already exist.
    /// </summary>
    /// <param name="name">Name of the container to create.</param>
    /// <returns>The newly created container, or the existing container if it already exists.</returns>
    public Container Create(string name) {
      EnsureValidContainerName(name);
      return new Container(_provider.Create(name));
    }

    /// <summary>
    /// Deletes the container with the specified name.
    /// </summary>
    /// <param name="name">Name of the container to delete.</param>
    public void Delete(string name) {
      EnsureValidContainerName(name);
      _provider.Delete(name);
    }

    private static void EnsureValidContainerName(string name) {
      if (name == null) {
        throw new InvalidNameException(name, "Container names cannot be null.");
      }

      if (name.Length > 63 || name.Length < 3) {
        throw new InvalidNameException(name, "Container names must be between 3 and 63 characters.");
      }

      if (!Regex.IsMatch(name, @"^([a-z0-9]+[\-]?)*[a-z0-9]$")) {
        throw new InvalidNameException(name);
      }
    }
  }
}
