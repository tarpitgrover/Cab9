using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cab9.Api.Compression
{
    public class CompressedContent : HttpContent
    {
        private readonly HttpContent _originalContent;
        private readonly string _encodingType;

        public CompressedContent(HttpContent content, string encodingType)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (encodingType == null)
            {
                throw new ArgumentNullException("encodingType");
            }

            _originalContent = content;
            this._encodingType = encodingType.ToLowerInvariant();

            if (this._encodingType != "gzip" && this._encodingType != "deflate")
            {
                throw new InvalidOperationException(string.Format("Encoding '{0}' is not supported. Only supports gzip or deflate encoding.", this._encodingType));
            }

            foreach (KeyValuePair<string, IEnumerable<String>> header in _originalContent.Headers)
            {
                this.Headers.Add(header.Key, header.Value);
            }

            this.Headers.ContentEncoding.Add(encodingType);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            return false;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Stream compressedStream = null;

            if (_encodingType == "gzip")
            {
                compressedStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);
            }
            else if (_encodingType == "deflate")
            {
                compressedStream = new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true);
            }

            return _originalContent.CopyToAsync(compressedStream).ContinueWith(tsk =>
            {
                if (compressedStream != null)
                {
                    compressedStream.Dispose();
                }
            });
        }
    } 

}