using System;
using System.Collections.Generic;

namespace TypeSupport
{
    /// <summary>
    /// A hashset for tracking hashcodes in use
    /// </summary>
    public class ObjectHashcodeMap : HashSet<ObjectHashcode>
    {
        /// <summary>
        /// A hashset for tracking hashcodes in use
        /// </summary>
        public ObjectHashcodeMap()
        {

        }

        /// <summary>
        /// Add an object to the hashcode map
        /// </summary>
        /// <param name="obj"></param>
        public void Add(object obj)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException(nameof(obj));
            var hashCode = GetHashcodeForObject(obj);
            var type = obj.GetType();
            Add(hashCode, type);
        }

        /// <summary>
        /// Add a hashcode/type pair to the hashcode map
        /// </summary>
        /// <param name="hashcode"></param>
        /// <param name="type">Object Extended type</param>
        public void Add(int hashcode, ExtendedType type)
        {
            if (ReferenceEquals(type, null))
                throw new ArgumentNullException(nameof(type));
            var key = new ObjectHashcode(hashcode, type.Type);
            base.Add(key);
        }

        /// <summary>
        /// Add a hashcode/type pair to the hashcode map
        /// </summary>
        /// <param name="hashcode"></param>
        /// <param name="type">Object type</param>
        public void Add(int hashcode, Type type)
        {
            if (ReferenceEquals(type, null))
                throw new ArgumentNullException(nameof(type));
            var key = new ObjectHashcode(hashcode, type);
            base.Add(key);
        }

        /// <summary>
        /// Check if an object is in the hashcode map
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Contains(object obj)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException(nameof(obj));
            var hashcode = GetHashcodeForObject(obj);
            var type = obj.GetType();
            var key = new ObjectHashcode(hashcode, type);
            return base.Contains(key);
        }

        /// <summary>
        /// Check if an object is in the hashcode map
        /// </summary>
        /// <param name="hashcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Contains(int hashcode, Type type)
        {
            if (ReferenceEquals(type, null))
                throw new ArgumentNullException(nameof(type));
            var key = new ObjectHashcode(hashcode, type);
            return base.Contains(key);
        }

        /// <summary>
        /// Check if an object is in the hashcode map
        /// </summary>
        /// <param name="hashcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Contains(int hashcode, ExtendedType type)
        {
            if (ReferenceEquals(type, null))
                throw new ArgumentNullException(nameof(type));
            var key = new ObjectHashcode(hashcode, type.Type);
            return base.Contains(key);
        }

        /// <summary>
        /// Return the hashcode for an object instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private int GetHashcodeForObject(object obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
