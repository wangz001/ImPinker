
namespace EasyNet.Solr.Commons
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    /**
     * The class is designed to optimaly serialize/deserialize any supported types in Solr response. As we know there are only a limited type of
     * items this class can do it with very minimal amount of payload and code. There are 15 known types and if there is an
     * object in the object tree which does not fall into these types, It must be converted to one of these. Implement an
     * ObjectResolver and pass it over It is expected that this class is used on both end of the pipes. The class has one
     * read method and one write method for each of the datatypes
     * <p/>
     * Note -- Never re-use an instance of this class for more than one marshal or unmarshall operation. Always create a new
     * instance.
     */
    public class JavaBinCodec : ICodec
    {

        public const byte
                NULL = 0,
                BOOL_TRUE = 1,
                BOOL_FALSE = 2,
                BYTE = 3,
                SHORT = 4,
                DOUBLE = 5,
                INT = 6,
                LONG = 7,
                FLOAT = 8,
                DATE = 9,
                MAP = 10,
                SOLRDOC = 11,
                SOLRDOCLST = 12,
                BYTEARR = 13,
                ITERATOR = 14,
            /**
             * this is a special tag signals an end. No value is associated with it
             */
                END = 15,

                SOLRINPUTDOC = 16,

                // types that combine tag + length (or other info) in a single byte
                TAG_AND_LEN = (byte)(1 << 5),
                STR = (byte)(1 << 5),
                SINT = (byte)(2 << 5),
                SLONG = (byte)(3 << 5),
                ARR = (byte)(4 << 5), //
                ORDERED_MAP = (byte)(5 << 5), // SimpleOrderedMap (a NamedList subclass, and more common)
                NAMED_LST = (byte)(6 << 5), // NamedList
                EXTERN_STRING = (byte)(7 << 5);


        private static byte VERSION = 2;
        protected FastOutputStream daos;

        public JavaBinCodec()
        {
        }

        public void Marshal(Object nl, Stream os)
        {
            daos = new FastOutputStream(os);
            try
            {
                daos.WriteByte(VERSION);
                WriteVal(nl);
            }
            finally
            {
                daos.Flush();
                daos.Close();
            }
        }

        byte version;

        public Object Unmarshal(Stream inputStream)
        {
            FastInputStream dis = new FastInputStream(inputStream);
            version = (byte)dis.ReadByte();
            if (version != VERSION)
            {
                throw new ApplicationException("Invalid version (expected " + VERSION +
                    ", but " + version + ") or the data in not in 'javabin' format");
            }
            return ReadVal(dis);
        }


        public SimpleOrderedMap ReadOrderedMap(FastInputStream dis)
        {
            int sz = ReadSize(dis);
            SimpleOrderedMap nl = new SimpleOrderedMap();
            for (int i = 0; i < sz; i++)
            {
                String name = (String)ReadVal(dis);
                Object val = ReadVal(dis);
                nl.Add(name, val);
            }
            return nl;
        }

        public NamedList ReadNamedList(FastInputStream dis)
        {
            int sz = ReadSize(dis);
            NamedList nl = new NamedList();
            for (int i = 0; i < sz; i++)
            {
                String name = (String)ReadVal(dis);
                Object val = ReadVal(dis);
                nl.Add(name, val);
            }
            return nl;
        }

        public void WriteNamedList(NamedList nl)
        {
            WriteTag(nl is SimpleOrderedMap ? ORDERED_MAP : NAMED_LST, nl.Count);
            for (int i = 0; i < nl.Count; i++)
            {
                String name = nl.GetName(i);
                WriteExternString(name);
                Object val = nl.GetVal(i);
                WriteVal(val);
            }
        }

        public void WriteVal(Object val)
        {
            WriteKnownType(val);
        }

        protected static readonly Object END_OBJ = new Object();

        protected byte tagByte;

        public Object ReadVal(FastInputStream dis)
        {
            tagByte = (byte)dis.ReadByte();

            // if ((tagByte & 0xe0) == 0) {
            // if top 3 bits are clear, this is a normal tag

            // OK, try type + size in single byte
            switch (tagByte >> 5)
            {
                case STR >> 5:
                    return ReadStr(dis);
                case SINT >> 5:
                    return ReadSmallInt(dis);
                case SLONG >> 5:
                    return ReadSmallLong(dis);
                case ARR >> 5:
                    return ReadArray(dis);
                case ORDERED_MAP >> 5:
                    return ReadOrderedMap(dis);
                case NAMED_LST >> 5:
                    return ReadNamedList(dis);
                case EXTERN_STRING >> 5:
                    return ReadExternString(dis);
            }

            switch (tagByte)
            {
                case NULL:
                    return null;
                case DATE:
                    try
                    {
                        return dis.ReadLong().ConvertToDateTime();
                    }
                    catch
                    {
                        return null;
                    }
                case INT:
                    return dis.ReadInt();
                case BOOL_TRUE:
                    return true;
                case BOOL_FALSE:
                    return false;
                case FLOAT:
                    return dis.ReadFloat();
                case DOUBLE:
                    return dis.ReadDouble();
                case LONG:
                    return dis.ReadLong();
                case BYTE:
                    return dis.ReadByte();
                case SHORT:
                    return dis.ReadShort();
                case MAP:
                    return ReadMap(dis);
                case SOLRDOC:
                    return ReadSolrDocument(dis);
                case SOLRDOCLST:
                    return ReadSolrDocumentList(dis);
                case BYTEARR:
                    return ReadByteArray(dis);
                case ITERATOR:
                    return ReadIterator(dis);
                case END:
                    return END_OBJ;
                case SOLRINPUTDOC:
                    return ReadSolrInputDocument(dis);
            }

            throw new ApplicationException("Unknown type " + tagByte);
        }

        public bool WriteKnownType(Object val)
        {
            if (WritePrimitive(val)) return true;
            if (val is NamedList)
            {
                WriteNamedList((NamedList)val);
                return true;
            }
            if (val is SolrDocumentList)
            { // SolrDocumentList is a List, so must come before List check
                WriteSolrDocumentList((SolrDocumentList)val);
                return true;
            }
            if (val is IList)
            {
                WriteArray((IList)val);
                return true;
            }
            if (val is Object[])
            {
                WriteArray((Object[])val);
                return true;
            }
            if (val is SolrDocument)
            {
                WriteSolrDocument((SolrDocument)val);
                return true;
            }
            if (val is SolrInputDocument)
            {
                WriteSolrInputDocument((SolrInputDocument)val);
                return true;
            }
            if (val is IDictionary)
            {
                WriteMap((IDictionary)val);
                return true;
            }
            if (val is IEnumerator)
            {
                WriteIterator((IEnumerator)val);
                return true;
            }
            if (val is IEnumerable)
            {
                WriteIterator(((IEnumerable)val).GetEnumerator());
                return true;
            }
            return false;
        }

        public void WriteTag(byte tag)
        {
            daos.WriteByte(tag);
        }

        public void WriteTag(byte tag, int size)
        {
            if ((tag & 0xe0) != 0)
            {
                if (size < 0x1f)
                {
                    daos.WriteByte(tag | size);
                }
                else
                {
                    daos.WriteByte(tag | 0x1f);
                    WriteVInt(size - 0x1f, daos);
                }
            }
            else
            {
                daos.WriteByte(tag);
                WriteVInt(size, daos);
            }
        }

        public void WriteByteArray(byte[] arr, int offset, int len)
        {
            WriteTag(BYTEARR, len);
            daos.Write(arr, offset, len);
        }

        public byte[] ReadByteArray(FastInputStream dis)
        {
            byte[] arr = new byte[ReadVInt(dis)];
            dis.ReadFully(arr);
            return arr;
        }

        public void WriteSolrDocument(SolrDocument doc)
        {
            WriteTag(SOLRDOC);
            WriteTag(ORDERED_MAP, doc.Count);
            foreach (KeyValuePair<string, object> entry in doc)
            {
                String name = entry.Key;
                WriteExternString(name);
                Object val = entry.Value;
                WriteVal(val);
            }
        }

        public SolrDocument ReadSolrDocument(FastInputStream dis)
        {
            NamedList nl = (NamedList)ReadVal(dis);
            SolrDocument doc = new SolrDocument();
            for (int i = 0; i < nl.Count; i++)
            {
                String name = nl.GetName(i);
                Object val = nl.GetVal(i);
                doc.SetField(name, val);
            }
            return doc;
        }

        public SolrDocumentList ReadSolrDocumentList(FastInputStream dis)
        {
            SolrDocumentList solrDocs = new SolrDocumentList();
            IList list = (IList)ReadVal(dis);
            solrDocs.NumFound = (long)list[0];
            solrDocs.Start = (long)list[1];
            solrDocs.MaxScore = (float?)list[2];

            ArrayList l = (ArrayList)ReadVal(dis);
            foreach (SolrDocument doc in l)
            {
                solrDocs.Add(doc);
            }
            return solrDocs;
        }

        public void WriteSolrDocumentList(SolrDocumentList docs)
        {
            WriteTag(SOLRDOCLST);
            IList l = new ArrayList(3);
            l.Add(docs.NumFound);
            l.Add(docs.Start);
            l.Add(docs.MaxScore);
            WriteArray(l);
            WriteArray(docs);
        }

        public SolrInputDocument ReadSolrInputDocument(FastInputStream dis)
        {
            int sz = ReadVInt(dis);
            float? docBoost = (float?)ReadVal(dis);
            SolrInputDocument sdoc = new SolrInputDocument();
            sdoc.Boost = docBoost;
            for (int i = 0; i < sz; i++)
            {
                float boost = 1.0f;
                String fieldName;
                Object boostOrFieldName = ReadVal(dis);
                if (boostOrFieldName is float)
                {
                    boost = (float)boostOrFieldName;
                    fieldName = (String)ReadVal(dis);
                }
                else
                {
                    fieldName = (String)boostOrFieldName;
                }
                Object fieldVal = ReadVal(dis);
                sdoc[fieldName] = new SolrInputField(fieldName, fieldVal, boost);
            }
            return sdoc;
        }

        public void WriteSolrInputDocument(SolrInputDocument sdoc)
        {
            WriteTag(SOLRINPUTDOC, sdoc.Count);
            WriteFloat(sdoc.Boost.Value);
            foreach (SolrInputField inputField in sdoc.Values)
            {
                if (inputField.Boost != 1.0f)
                {
                    WriteFloat(inputField.Boost.Value);
                }
                WriteExternString(inputField.Name);
                WriteVal(inputField.Value);
            }
        }


        public IDictionary<Object, Object> ReadMap(FastInputStream dis)
        {
            int sz = ReadVInt(dis);
            IDictionary<Object, Object> m = new LinkedHashMap<Object, Object>();
            for (int i = 0; i < sz; i++)
            {
                Object key = ReadVal(dis);
                Object val = ReadVal(dis);
                m[key] = val;

            }
            return m;
        }

        public void WriteIterator(IEnumerator iter)
        {
            WriteTag(ITERATOR);
            while (iter.MoveNext())
            {
                WriteVal(iter.Current);
            }
            WriteVal(END_OBJ);
        }

        public IList ReadIterator(FastInputStream fis)
        {
            ArrayList l = new ArrayList();
            while (true)
            {
                Object o = ReadVal(fis);
                if (o == END_OBJ) break;
                l.Add(o);
            }
            return l;
        }

        public void WriteArray(IList l)
        {
            WriteTag(ARR, l.Count);
            for (int i = 0; i < l.Count; i++)
            {
                WriteVal(l[i]);
            }
        }

        public void WriteArray(Object[] arr)
        {
            WriteTag(ARR, arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                Object o = arr[i];
                WriteVal(o);
            }
        }

        public IList ReadArray(FastInputStream dis)
        {
            int sz = ReadSize(dis);
            ArrayList l = new ArrayList(sz);
            for (int i = 0; i < sz; i++)
            {
                l.Add(ReadVal(dis));
            }
            return l;
        }

        /**
         * write the string as tag+length, with length being the number of UTF-8 bytes
         */
        public void WriteStr(String s)
        {
            if (s == null)
            {
                WriteTag(NULL);
                return;
            }
            int end = s.Length;
            int maxSize = end * 4;
            if (bytes == null || bytes.Length < maxSize) bytes = new byte[maxSize];
            int upto = 0;
            for (int i = 0; i < end; i++)
            {
                int code = s[i];

                if (code < 0x80)
                    bytes[upto++] = (byte)code;
                else if (code < 0x800)
                {
                    bytes[upto++] = (byte)(0xC0 | (code >> 6));
                    bytes[upto++] = (byte)(0x80 | (code & 0x3F));
                }
                else if (code < 0xD800 || code > 0xDFFF)
                {
                    bytes[upto++] = (byte)(0xE0 | (code >> 12));
                    bytes[upto++] = (byte)(0x80 | ((code >> 6) & 0x3F));
                    bytes[upto++] = (byte)(0x80 | (code & 0x3F));
                }
                else
                {
                    // surrogate pair
                    // confirm valid high surrogate
                    if (code < 0xDC00 && (i < end - 1))
                    {
                        int utf32 = s[i + 1];
                        // confirm valid low surrogate and write pair
                        if (utf32 >= 0xDC00 && utf32 <= 0xDFFF)
                        {
                            utf32 = ((code - 0xD7C0) << 10) + (utf32 & 0x3FF);
                            i++;
                            bytes[upto++] = (byte)(0xF0 | (utf32 >> 18));
                            bytes[upto++] = (byte)(0x80 | ((utf32 >> 12) & 0x3F));
                            bytes[upto++] = (byte)(0x80 | ((utf32 >> 6) & 0x3F));
                            bytes[upto++] = (byte)(0x80 | (utf32 & 0x3F));
                            continue;
                        }
                    }
                    // replace unpaired surrogate or out-of-order low surrogate
                    // with substitution character
                    bytes[upto++] = (byte)0xEF;
                    bytes[upto++] = (byte)0xBF;
                    bytes[upto++] = (byte)0xBD;
                }
            }
            WriteTag(STR, upto);
            daos.Write(bytes, 0, upto);
        }

        byte[] bytes;
        char[] chars;

        public String ReadStr(FastInputStream dis)
        {
            int sz = ReadSize(dis);
            if (chars == null || chars.Length < sz) chars = new char[sz];
            if (bytes == null || bytes.Length < sz) bytes = new byte[sz];
            dis.ReadFully(bytes, 0, sz);

            int outUpto = 0;
            for (int i = 0; i < sz; )
            {
                int b = bytes[i++] & 0xff;
                int ch;
                if (b < 0xc0)
                {
                    ch = b;
                }
                else if (b < 0xe0)
                {
                    ch = ((b & 0x1f) << 6) + (bytes[i++] & 0x3f);
                }
                else if (b < 0xf0)
                {
                    ch = ((b & 0xf) << 12) + ((bytes[i++] & 0x3f) << 6) + (bytes[i++] & 0x3f);
                }
                else
                {
                    ch = ((b & 0x7) << 18) + ((bytes[i++] & 0x3f) << 12) + ((bytes[i++] & 0x3f) << 6) + (bytes[i++] & 0x3f);
                }
                if (ch <= 0xFFFF)
                {
                    // target is a character <= 0xFFFF
                    chars[outUpto++] = (char)ch;
                }
                else
                {
                    // target is a character in range 0xFFFF - 0x10FFFF
                    int chHalf = ch - 0x10000;
                    chars[outUpto++] = (char)((chHalf >> 0xA) + 0xD800);
                    chars[outUpto++] = (char)((chHalf & 0x3FF) + 0xDC00);
                }
            }
            return new String(chars, 0, outUpto);
        }

        public void WriteInt(int val)
        {
            if (val > 0)
            {
                int b = SINT | (val & 0x0f);

                if (val >= 0x0f)
                {
                    b |= 0x10;
                    daos.WriteByte(b);
                    WriteVInt(val >> 4, daos);
                }
                else
                {
                    daos.WriteByte(b);
                }

            }
            else
            {
                daos.WriteByte(INT);
                daos.WriteInt(val);
            }
        }

        public int ReadSmallInt(FastInputStream dis)
        {
            int v = tagByte & 0x0F;
            if ((tagByte & 0x10) != 0)
                v = (ReadVInt(dis) << 4) | v;
            return v;
        }


        public void WriteLong(long val)
        {
            if (((ulong)val & 0xff00000000000000L) == 0)
            {
                int b = SLONG | ((int)val & 0x0f);
                if (val >= 0x0f)
                {
                    b |= 0x10;
                    daos.WriteByte(b);
                    WriteVLong(val >> 4, daos);
                }
                else
                {
                    daos.WriteByte(b);
                }
            }
            else
            {
                daos.WriteByte(LONG);
                daos.WriteLong(val);
            }
        }

        public long ReadSmallLong(FastInputStream dis)
        {
            long v = tagByte & 0x0F;
            if ((tagByte & 0x10) != 0)
                v = (ReadVLong(dis) << 4) | v;
            return v;
        }

        public void WriteFloat(float val)
        {
            daos.WriteByte(FLOAT);
            daos.WriteFloat(val);
        }

        public bool WritePrimitive(Object val)
        {
            if (val == null)
            {
                daos.WriteByte(NULL);
                return true;
            }
            else if (val is String)
            {
                WriteStr((String)val);
                return true;
            }
            else if (val is int)
            {
                WriteInt((int)val);
                return true;
            }
            else if (val is long)
            {
                WriteLong((long)val);
                return true;
            }
            else if (val is float)
            {
                WriteFloat((float)val);
                return true;
            }
            else if (val is double)
            {
                daos.WriteByte(DOUBLE);
                daos.WriteDouble((double)val);
                return true;
            }
            else if (val is byte)
            {
                daos.WriteByte(BYTE);
                daos.WriteByte((byte)val);
                return true;
            }
            else if (val is short)
            {
                daos.WriteByte(SHORT);
                daos.WriteShort((short)val);
                return true;
            }


            else if (val is DateTime)
            {
                daos.WriteByte(DATE);
                daos.WriteLong(((DateTime)val).ConvertToLong());
                return true;
            }
            else if (val is Boolean)
            {
                if ((Boolean)val) daos.WriteByte(BOOL_TRUE);
                else daos.WriteByte(BOOL_FALSE);
                return true;
            }
            else if (val is byte[])
            {
                WriteByteArray((byte[])val, 0, ((byte[])val).Length);
                return true;
            }
            else if (val is byte[])
            {
                byte[] buf = (byte[])val;
                WriteByteArray(buf, 0, buf.Length);
                return true;
            }
            else if (val == END_OBJ)
            {
                WriteTag(END);
                return true;
            }
            return false;
        }


        public void WriteMap(IDictionary val)
        {
            WriteTag(MAP, val.Count);
            foreach (DictionaryEntry entry in val)
            {
                Object key = entry.Key;
                if (key is String)
                {
                    WriteExternString((String)key);
                }
                else
                {
                    WriteVal(key);
                }
                WriteVal(entry.Value);
            }
        }


        public int ReadSize(FastInputStream stream)
        {
            int sz = tagByte & 0x1f;
            if (sz == 0x1f) sz += ReadVInt(stream);
            return sz;
        }


        /**
         * Special method for variable length int (copied from lucene). Usually used for writing the length of a
         * collection/array/map In most of the cases the length can be represented in one byte (length < 127) so it saves 3
         * bytes/object
         *
         * @ If there is a low-level I/O error.
         */
        public static void WriteVInt(int i, FastOutputStream stream)
        {
            while ((i & ~0x7F) != 0)
            {
                stream.WriteByte((byte)((i & 0x7f) | 0x80));
                i >>= 7;
            }
            stream.WriteByte((byte)i);
        }

        /**
         * The counterpart for {@link #writeVInt(int, FastOutputStream)}
         *
         * @ If there is a low-level I/O error.
         */
        public static int ReadVInt(FastInputStream stream)
        {
            byte b = (byte)stream.ReadByte();
            int i = b & 0x7F;
            for (int shift = 7; (b & 0x80) != 0; shift += 7)
            {
                b = (byte)stream.ReadByte();
                i |= (b & 0x7F) << shift;
            }
            return i;
        }


        public static void WriteVLong(long i, FastOutputStream stream)
        {
            while ((i & ~0x7F) != 0)
            {
                stream.WriteByte((byte)((i & 0x7f) | 0x80));
                i >>= 7;
            }
            stream.WriteByte((byte)i);
        }

        public static long ReadVLong(FastInputStream stream)
        {
            byte b = (byte)stream.ReadByte();
            long i = b & 0x7F;
            for (int shift = 7; (b & 0x80) != 0; shift += 7)
            {
                b = (byte)stream.ReadByte();
                i |= (long)(b & 0x7F) << shift;
            }
            return i;
        }

        private int stringsCount = 0;
        private IDictionary<String, int?> stringsMap;
        private List<String> stringsList;

        public void WriteExternString(String s)
        {
            if (s == null)
            {
                WriteTag(NULL);
                return;
            }
            int? idx;
            if (stringsMap == null)
            {
                idx = null;
            }
            else
            {

                stringsMap.TryGetValue(s, out idx);
            }

            if (idx == null) idx = 0;
            WriteTag(EXTERN_STRING, idx.Value);
            if (idx == 0)
            {
                WriteStr(s);
                if (stringsMap == null) stringsMap = new Dictionary<String, int?>();
                stringsMap[s] = (++stringsCount);
            }

        }

        public String ReadExternString(FastInputStream fis)
        {
            int idx = ReadSize(fis);
            if (idx != 0)
            {// idx != 0 is the index of the extern string
                return stringsList[idx - 1];
            }
            else
            {// idx == 0 means it has a string value
                String s = (String)ReadVal(fis);
                if (stringsList == null) stringsList = new List<String>();
                stringsList.Add(s);
                return s;
            }
        }

    }
}
