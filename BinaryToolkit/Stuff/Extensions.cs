using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BinaryToolkit.Extensions
{
    public static class TypeConverters
    {
        public static T Deserialize<T>(this byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
        }

        public static byte[] ToByteArray<T>(this T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ToGenericArray<T>(this byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
    public static class IntPtrExtensions
    {
        #region Methods: Arithmetics
        public static IntPtr Decrement(this IntPtr pointer, Int32 value)
        {
            return Increment(pointer, -value);
        }

        public static IntPtr Decrement(this IntPtr pointer, Int64 value)
        {
            return Increment(pointer, -value);
        }

        public static IntPtr Decrement(this IntPtr pointer, IntPtr value)
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr(pointer.ToInt32() - value.ToInt32()));

                default:
                    return (new IntPtr(pointer.ToInt64() - value.ToInt64()));
            }
        }

        public static IntPtr Increment(this IntPtr pointer, Int32 value)
        {
            unchecked
            {
                switch (IntPtr.Size)
                {
                    case sizeof(Int32):
                        return (new IntPtr(pointer.ToInt32() + value));

                    default:
                        return (new IntPtr(pointer.ToInt64() + value));
                }
            }
        }

        public static IntPtr Increment(this IntPtr pointer, Int64 value)
        {
            unchecked
            {
                switch (IntPtr.Size)
                {
                    case sizeof(Int32):
                        return (new IntPtr((Int32)(pointer.ToInt32() + value)));

                    default:
                        return (new IntPtr(pointer.ToInt64() + value));
                }
            }
        }

        public static IntPtr Increment(this IntPtr pointer, IntPtr value)
        {
            unchecked
            {
                switch (IntPtr.Size)
                {
                    case sizeof(int):
                        return new IntPtr(pointer.ToInt32() + value.ToInt32());
                    default:
                        return new IntPtr(pointer.ToInt64() + value.ToInt64());
                }
            }
        }
        #endregion

        #region Methods: Comparison
        public static Int32 CompareTo(this IntPtr left, Int32 right)
        {
            return left.CompareTo((UInt32)right);
        }

        public static Int32 CompareTo(this IntPtr left, IntPtr right)
        {
            if (left.ToUInt64() > right.ToUInt64())
                return 1;

            if (left.ToUInt64() < right.ToUInt64())
                return -1;

            return 0;
        }

        public static Int32 CompareTo(this IntPtr left, UInt32 right)
        {
            if (left.ToUInt64() > right)
                return 1;

            if (left.ToUInt64() < right)
                return -1;

            return 0;
        }
        #endregion

        #region Methods: Conversion
        public unsafe static UInt32 ToUInt32(this IntPtr pointer)
        {
            return (UInt32)((void*)pointer);
        }

        public unsafe static UInt64 ToUInt64(this IntPtr pointer)
        {
            return (UInt64)((void*)pointer);
        }
        #endregion

        #region Methods: Equality
        public static Boolean Equals(this IntPtr pointer, Int32 value)
        {
            return (pointer.ToInt32() == value);
        }

        public static Boolean Equals(this IntPtr pointer, Int64 value)
        {
            return (pointer.ToInt64() == value);
        }

        public static Boolean Equals(this IntPtr left, IntPtr ptr2)
        {
            return (left == ptr2);
        }

        public static Boolean Equals(this IntPtr pointer, UInt32 value)
        {
            return (pointer.ToUInt32() == value);
        }

        public static Boolean Equals(this IntPtr pointer, UInt64 value)
        {
            return (pointer.ToUInt64() == value);
        }

        public static Boolean GreaterThanOrEqualTo(this IntPtr left, IntPtr right)
        {
            return (left.CompareTo(right) >= 0);
        }

        public static Boolean LessThanOrEqualTo(this IntPtr left, IntPtr right)
        {
            return (left.CompareTo(right) <= 0);
        }
        #endregion

        #region Methods: Logic
        public static IntPtr And(this IntPtr pointer, IntPtr value)
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr(pointer.ToInt32() & value.ToInt32()));

                default:
                    return (new IntPtr(pointer.ToInt64() & value.ToInt64()));
            }
        }

        public static IntPtr Not(this IntPtr pointer)
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr(~pointer.ToInt32()));

                default:
                    return (new IntPtr(~pointer.ToInt64()));
            }
        }

        public static IntPtr Or(this IntPtr pointer, IntPtr value)
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr(pointer.ToInt32() | value.ToInt32()));

                default:
                    return (new IntPtr(pointer.ToInt64() | value.ToInt64()));
            }
        }

        public static IntPtr Xor(this IntPtr pointer, IntPtr value)
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr(pointer.ToInt32() ^ value.ToInt32()));

                default:
                    return (new IntPtr(pointer.ToInt64() ^ value.ToInt64()));
            }
        }
        #endregion
    }
}
