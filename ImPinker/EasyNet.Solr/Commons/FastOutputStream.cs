
namespace EasyNet.Solr.Commons
{
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class FastOutputStream : Stream
    {
        private BufferedStream stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        public FastOutputStream(Stream stream, int bufferSize = 8192)
        {
            this.stream = new BufferedStream(stream, bufferSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void Write(int b)
        {
            stream.WriteByte((byte)b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void Write(byte[] b)
        {
            stream.Write(b, 0, b.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteBoolean(bool v)
        {
            Write(v ? 1 : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteByte(int v)
        {
            Write((byte)v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteShort(int v)
        {
            Write((byte)(v >> 8));
            Write((byte)v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteChar(int v)
        {
            WriteShort(v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteInt(int v)
        {
            WriteByte((byte)(v >> 24));
            WriteByte((byte)(v >> 16));
            WriteByte((byte)(v >> 8));
            WriteByte((byte)(v));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteLong(long v)
        {
            WriteByte((byte)(v >> 56));
            WriteByte((byte)(v >> 48));
            WriteByte((byte)(v >> 40));
            WriteByte((byte)(v >> 32));
            WriteByte((byte)(v >> 24));
            WriteByte((byte)(v >> 16));
            WriteByte((byte)(v >> 8));
            WriteByte((byte)(v));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteFloat(float v)
        {
            FloatConverter floatConverter = new FloatConverter();

            WriteInt(FloatConverter.ToInt(v, ref floatConverter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        public void WriteDouble(double v)
        {
            DoubleConverter doubleConverter = new DoubleConverter();

            WriteLong(DoubleConverter.ToLong(v, ref doubleConverter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public void WriteBytes(string s)
        {
            foreach (char c in s)
            {
                WriteByte((byte)c);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public void WriteChars(string s)
        {
            foreach (char c in s)
            {
                WriteChar(c);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            stream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get { return stream.Length; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get
            {
                return stream.Position;
            }
            set
            {
                stream.Position = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
}
