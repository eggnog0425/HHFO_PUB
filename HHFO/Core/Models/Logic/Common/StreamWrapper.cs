using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HHFO.Models.Logic.Common
{
    public class StreamWrapper : Stream
    {
        Stream Base;

        public StreamWrapper(Stream stream)
        {
            if(stream == null)
            {
                throw new ArgumentNullException("Argument Stream is null");
            }
            Base = stream;
        }

        public override bool CanRead => Base.CanRead;

        public override bool CanSeek => Base.CanSeek;

        public override bool CanWrite => Base.CanWrite;

        public override long Length => Base.Length;

        public override long Position { get => Base.Position; set => Base.Position = value; }

        public override bool CanTimeout => Base.CanTimeout;

        public override int ReadTimeout { get => Base.ReadTimeout; set => Base.ReadTimeout = value; }
        public override int WriteTimeout { get => Base.WriteTimeout; set => Base.WriteTimeout = value; }

        public override void Flush()
        {
            Base.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Base.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Base.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            Base.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Base.Write(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Base.ReadAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return Base.ReadAsync(buffer, cancellationToken);
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return Base.BeginRead(buffer, offset, count, callback, state);
        }

        public override bool Equals(object obj)
        {
            return Base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Base.GetHashCode();
        }

        public override string ToString()
        {
            return Base.ToString();
        }

        public override object InitializeLifetimeService()
        {
            return Base.InitializeLifetimeService();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return Base.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            Base.Close();
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            Base.CopyTo(destination, bufferSize);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return Base.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        [Obsolete]
        protected override WaitHandle CreateWaitHandle()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Base.Dispose();
                Base = null;
            }
            base.Dispose(disposing);
        }

        public override ValueTask DisposeAsync()
        {
            return Base.DisposeAsync();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return Base.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            Base.EndWrite(asyncResult);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Base.FlushAsync(cancellationToken);
        }

        [Obsolete]
        protected override void ObjectInvariant()
        {
            throw new NotImplementedException();
        }

        public override int Read(Span<byte> buffer)
        {
            return Base.Read(buffer);
        }

        public override int ReadByte()
        {
            return Base.ReadByte();
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            Base.Write(buffer);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return Base.WriteAsync(buffer, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            Base.WriteByte(value);
        }
    }
}
