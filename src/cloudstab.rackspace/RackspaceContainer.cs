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

using System.Collections.Generic;
using cloudstab.core;
using Rackspace.CloudFiles.Domain;

namespace cloudstab.rackspace {
  public class RackspaceContainer : IBlobContainer {
    private IContainer _container;
    private string _name;
    private IAccount _account;

    public RackspaceContainer(string name, IAccount account) {
      _name = name;
      _account = account;
    }

    public RackspaceContainer(IContainer container) {
      _container = container;
      _name = container.Name;
    }

    #region Implementation of IBlobContainer
    public IEnumerable<IBlobObject> ListObjects() {
      throw new System.NotImplementedException();
    }

    public void AddObject(object key, object blob) {
      throw new System.NotImplementedException();
    }

    public void DeleteObject(object key) {
      throw new System.NotImplementedException();
    }

    public IBlobObject GetObject(object key) {
      throw new System.NotImplementedException();
    }

    public string Name {
      get { return _name; }
    }

    #endregion
  }
}
