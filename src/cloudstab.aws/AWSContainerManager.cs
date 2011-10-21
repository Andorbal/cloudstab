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
using System.Security;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using cloudstab.aws.Credentials;
using cloudstab.core;
using cloudstab.core.Exceptions;

namespace cloudstab.aws {
  public class AWSContainerManager : IBlobContainerManager {
    private AmazonS3 _client;

    public AWSContainerManager(IAWSCredentialProvider credentialProvider)
      : this(credentialProvider.GetCredentials()) {
    }

    public AWSContainerManager(AWSCredentials credentials) {
      _client = new AmazonS3Client(credentials);
    }

    #region "IBlobContainerManager Implementation"
    public IEnumerable<IBlobContainer> List() {
      return GetBuckets().Select(x => new AWSContainer(_client, x));
    }

    public IBlobContainer Get(string name) {
      return GetBuckets().Where(x => string.Equals(x.BucketName, name, StringComparison.OrdinalIgnoreCase))
        .Select(x => new AWSContainer(_client, x))
        .SingleOrDefault();
    }

    public IBlobContainer Create(string name) {
      try {
        var request = new PutBucketRequest() { BucketName = name };
        _client.PutBucket(request);
        return Get(name);
      }
      catch (AmazonS3Exception ex) {
        throw WrapException(ex);
      }
    }

    public void Delete(string name) {
      try {
        var request = new DeleteBucketRequest() { BucketName = name };
        _client.DeleteBucket(request);
      }
      catch (AmazonS3Exception ex) {
        throw WrapException(ex);
      }
    }
    #endregion

    private static bool IsSecurityException(string errorCode) {
      return errorCode != null &&
             (errorCode.Equals("InvalidAccessKeyId") ||
              errorCode.Equals("InvalidSecurity"));
    }

    private List<S3Bucket> GetBuckets() {
      try {
        using (var response = _client.ListBuckets()) {
          return response.Buckets;
        }
      }
      catch (AmazonS3Exception ex) {
        throw WrapException(ex);
      }
    }

    private static Exception WrapException(AmazonS3Exception ex) {
      if (IsSecurityException(ex.ErrorCode)) {
        return new BlobSecurityException(ex);
      }

      return ex;
    }
  }
}
