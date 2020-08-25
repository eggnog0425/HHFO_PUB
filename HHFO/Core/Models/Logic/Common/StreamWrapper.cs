using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HHFO.Models.Logic.Common
{
    /// <summary>
    /// MemoryStreamのDispose時にバッファが残るのに対応するためのラッパ。Dispose時にbaseの参照を外して対応
    /// See: https://pierre3.hatenablog.com/entry/2015/10/25/001207
    /// </summary>
    public class StreamWrapper : Stream
    {
        private Stream BaseStream;

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public override bool CanTimeout => base.CanTimeout;

        public override int ReadTimeout { get => base.ReadTimeout; set => base.ReadTimeout = value; }
        public override int WriteTimeout { get => base.WriteTimeout; set => base.WriteTimeout = value; }

        public StreamWrapper(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }
            BaseStream = stream;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }

        public override object InitializeLifetimeService()
        {
            return base.InitializeLifetimeService();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return base.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return base.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            base.Close();
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            base.CopyTo(destination, bufferSize);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return base.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        [Obsolete]
        protected override WaitHandle CreateWaitHandle()
        {
            return base.CreateWaitHandle();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                BaseStream.Dispose();
                BaseStream = null;
            }
            base.Dispose(disposing);
        }

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return base.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            base.EndWrite(asyncResult);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return base.FlushAsync(cancellationToken);
        }

        [Obsolete]
        protected override void ObjectInvariant()
        {
            base.ObjectInvariant();
        }

        public override int Read(Span<byte> buffer)
        {
            return base.Read(buffer);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return base.ReadAsync(buffer, cancellationToken);
        }

        public override int ReadByte()
        {
            return base.ReadByte();
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            base.Write(buffer);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return base.WriteAsync(buffer, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
        }
    }
}
