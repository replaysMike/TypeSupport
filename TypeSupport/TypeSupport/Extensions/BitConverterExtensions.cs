using System;
using System.Collections.Generic;

namespace TypeSupport.Extensions
{
    public static class BitConverterExtensions
    {
        /// <summary>
        /// Convert a decimal to a byte array
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        public static byte[] GetBytes(decimal dec)
        {
            var bits = decimal.GetBits(dec);
            var bytes = new List<byte>();
            foreach (var i in bits)
            {
                bytes.AddRange(BitConverter.GetBytes(i));
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Convert a byte array to a decimal
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static decimal ToDecimal(byte[] bytes)
        {
            if (bytes.Length != 16)
                throw new ArgumentException("A decimal must be created from exactly 16 bytes");
            var bits = new int[4];
            for (var i = 0; i <= 15; i += 4)
            {
                //convert every 4 bytes into an int32
                bits[i / 4] = BitConverter.ToInt32(bytes, i);
            }
            return new decimal(bits);
        }
    }
}
