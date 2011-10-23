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
using cloudstab.core;
using cloudstab.core.Exceptions;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace cloudstab.azure {
  public class AzureContainerManager : IBlobContainerManager {
    private CloudBlobClient _client;

    public AzureContainerManager(CloudStorageAccount account) {
      _client = account.CreateCloudBlobClient();
    }

    #region Implementation of IBlobContainerManager
    /// <summary>
    /// Lists all the containers in the store.
    /// </summary>
    /// <returns>A list of all the containers currently in the store.</returns>
    public IEnumerable<IBlobContainer> List() {
      try {
        return _client.ListContainers().Select(x => new AzureContainer(x));
      }
      catch (StorageClientException ex) {
        throw WrapException(ex);
      }
    }

    /// <summary>
    /// Gets the container with the specified name.
    /// </summary>
    /// <param name="name">Name of the container to retrieve.</param>
    /// <returns>The container with the specified name, or null if it doesn't exist.</returns>
    public IBlobContainer Get(string name) {
      BlobContainerUtilities.EnsureValidContainerName(name);

      try {
        return new AzureContainer(_client.GetContainerReference(name));
      }
      catch (StorageClientException ex) {
        throw WrapException(ex);
      }
    }

    /// <summary>
    /// Creates a new container if it doesn't already exist.
    /// </summary>
    /// <param name="name">Name of the container to create.</param>
    /// <returns>The newly created container, or the existing container if it already exists.</returns>
    public IBlobContainer Create(string name) {
      BlobContainerUtilities.EnsureValidContainerName(name);

      try {
        var container = _client.GetContainerReference(name);
        container.CreateIfNotExist();
        return new AzureContainer(container);
      }
      catch (StorageClientException ex) {
        throw WrapException(ex);
      }
    }

    /// <summary>
    /// Deletes the container with the specified name.
    /// </summary>
    /// <param name="name">Name of the container to delete.</param>
    public void Delete(string name) {
      BlobContainerUtilities.EnsureValidContainerName(name);

      try {
        var container = _client.GetContainerReference(name);
        container.Delete();
      }
      catch (StorageClientException ex) {
        throw WrapException(ex);
      }
    }
    #endregion

    private Exception WrapException(StorageClientException ex) {
      if (ex.ErrorCode == StorageErrorCode.AccessDenied 
          || ex.ErrorCode == StorageErrorCode.AccountNotFound 
          || ex.ErrorCode == StorageErrorCode.AuthenticationFailure) {
        return new BlobSecurityException(ex);
      }

      return ex;
    }
  }
}
