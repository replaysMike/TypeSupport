using System;

namespace TypeSupport
{
    /// <summary>
    /// Tracks an object reference by hashcode and type
    /// </summary>
    public struct ObjectHashcode
    {
        /// <summary>
        /// The object's hashcode
        /// </summary>
        public int Hashcode { get; set; }

        /// <summary>
        /// The object's type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Tracks an object reference by hashcode and type
        /// </summary>
        /// <param name="hashcode">The hashcode of the object</param>
        /// <param name="type">The object type</param>
        public ObjectHashcode(int hashcode, Type type)
        {
            if (ReferenceEquals(type, null))
                throw new ArgumentNullException(nameof(type));

            Hashcode = hashcode;
            Type = type;
        }

        public override int GetHashCode()
        {
            // recompute a new hashcode based on the type's hashcode and the object's hashcode
            var computedHashcode = 23;
            computedHashcode = computedHashcode * 31 + Hashcode;
            computedHashcode = computedHashcode * 31 + Type.GetHashCode();
            return computedHashcode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var typedObj = (ObjectHashcode)obj;
            return typedObj.Hashcode.Equals(Hashcode) && typedObj.Type.Equals(Type);
        }
    }
}
