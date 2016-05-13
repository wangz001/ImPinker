
namespace EasyNet.Solr.Commons
{
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class FastInputStream : Stream
    {
        private BufferedStream stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        public FastInputStream(Stream stream, int bufferSize = 8192)
        {
            this.stream = new BufferedStream(stream, bufferSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            return stream.ReadByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            return stream.ReadByte() & 0xff;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadUnsignedByte()
        {
            return stream.ReadByte() & 0xff;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            return (short)((ReadUnsignedByte() << 8) | ReadUnsignedByte());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadUnsignedShort()
        {
            return (ReadUnsignedByte() << 8) | ReadUnsignedByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public char ReadChar()
        {
            return (char)((ReadUnsignedByte() << 8) | ReadUnsignedByte());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            return ((ReadUnsignedByte() << 24)
                    | (ReadUnsignedByte() << 16)
                    | (ReadUnsignedByte() << 8)
                    | ReadUnsignedByte());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            return (((long)ReadUnsignedByte()) << 56)
                    | (((long)ReadUnsignedByte()) << 48)
                    | (((long)ReadUnsignedByte()) << 40)
                    | (((long)ReadUnsignedByte()) << 32)
                    | (((long)ReadUnsignedByte()) << 24)
                    | (uint)(ReadUnsignedByte() << 16)
                    | (uint)(ReadUnsignedByte() << 8)
                    | (uint)(ReadUnsignedByte());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void ReadFully(byte[] b)
        {
            ReadFully(b, 0, b.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="off"></param>
        /// <param name="len"></param>
        public void ReadFully(byte[] b, int off, int len)
        {
            while (len > 0)
            {
                int ret = Read(b, off, len);
                if (ret == -1)
                {
                    throw new EndOfStreamException();
                }
                off += ret;
                len -= ret;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            FloatConverter floatConverter = new FloatConverter();

            return FloatConverter.ToFloat(ReadInt(), ref floatConverter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            DoubleConverter doubleConverter = new DoubleConverter();

            return DoubleConverter.ToDouble(ReadLong(), ref doubleConverter);
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
