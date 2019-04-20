using System;
using System.Collections.Generic;
using System.Threading;

namespace TypeSupport
{
	/// <summary>
	/// Caches extended types
	/// </summary>
	public sealed class ExtendedTypeCache
	{
		private static SemaphoreSlim _cacheLock = new SemaphoreSlim(1);
		private static readonly ExtendedTypeCache _instance = new ExtendedTypeCache();

		/// <summary>
		/// Get an instance of the type cache
		/// </summary>
		public static ExtendedTypeCache Instance { get { return _instance; } }

		/// <summary>
		/// Get the cached types registry
		/// </summary>
		public IDictionary<CacheKey, ExtendedType> CachedTypes { get; set; }

		static ExtendedTypeCache()
		{
			_instance.CachedTypes = new Dictionary<CacheKey, ExtendedType>();
		}

		private ExtendedTypeCache()
		{
		}

        /// <summary>
        /// Clear the cache
        /// </summary>
        public static void Clear()
        {
            _cacheLock.Wait();
            try
            {
                Instance.CachedTypes.Clear();
            }
            finally
            {
                _cacheLock.Release();
            }
        }

		/// <summary>
		/// Returns true if the extended type is cached
		/// </summary>
		/// <param name="type">The type to lookup</param>
		/// <param name="options">The options for the extended type</param>
		/// <returns></returns>
		public static bool Contains(Type type, TypeSupportOptions options)
		{
			_cacheLock.Wait();
			try
			{
				return ContainsInternal(type, options);
			}
			finally
			{
				_cacheLock.Release();
			}
		}

		/// <summary>
		/// Get an extended type from the cache
		/// </summary>
		/// <param name="type">The type to lookup</param>
		/// <param name="options">The options for the extended type</param>
		/// <returns></returns>
		public static ExtendedType Get(Type type, TypeSupportOptions options)
		{
			_cacheLock.Wait();
			try
			{
				if (!ContainsInternal(type, options))
					throw new InvalidOperationException($"Requested type '{type.Name}' is not cached");
				var key = GenerateKey(type, options);
				return Instance.CachedTypes[key];
			}
			finally
			{
				_cacheLock.Release();
			}
		}

		/// <summary>
		/// Cache an extended type
		/// </summary>
		/// <param name="type">The extended type to cache</param>
		/// <param name="options">The options for the extended type</param>
		public static void CacheType(ExtendedType type, TypeSupportOptions options)
		{
			_cacheLock.Wait();
			try
			{
				var key = GenerateKey(type.Type, options);
				if (!Instance.CachedTypes.ContainsKey(key))
				{
					Instance.CachedTypes.Add(key, type);
					if (key.Options == TypeSupportOptions.All)
					{
						// remove any types with options less than all
						RemoveLowerOptions(type.Type);
					}
				}
			}
			finally
			{
				_cacheLock.Release();
			}
		}

		/// <summary>
		/// Returns true if the extended type is cached
		/// </summary>
		/// <param name="type">The type to lookup</param>
		/// <param name="options">The options for the extended type</param>
		/// <returns></returns>
		private static bool ContainsInternal(Type type, TypeSupportOptions options)
		{
			if (options != TypeSupportOptions.All)
			{
				// does it exist with all options available?
				var allKey = new CacheKey(type, TypeSupportOptions.All);
				var containsAllKey = Instance.CachedTypes.ContainsKey(allKey);
				if (containsAllKey) return true;
			}
			// use the key as-is
			var key = new CacheKey(type, options);
			var containsKey = Instance.CachedTypes.ContainsKey(key);
			return containsKey;
		}

		/// <summary>
		/// Generates the type of key to use for the type cache
		/// </summary>
		/// <param name="type">The type to lookup</param>
		/// <param name="options">The options for the extended type</param>
		/// <returns></returns>
		private static CacheKey GenerateKey(Type type, TypeSupportOptions options)
		{
			if (options != TypeSupportOptions.All)
			{
				// does it exist with all options available?
				var allKey = new CacheKey(type, TypeSupportOptions.All);
				var containsAllKey = Instance.CachedTypes.ContainsKey(allKey);
				if (containsAllKey)
					return allKey;
			}
			// use the key as-is
			var key = new CacheKey(type, options);
			return key;
		}

		/// <summary>
		/// Remove any cached types with options less than TypeSupportOptions.All
		/// </summary>
		/// <param name="type">The type to filter by</param>
		private static void RemoveLowerOptions(Type type)
		{
			var typesToRemove = new List<CacheKey>();
			foreach (var cachedType in Instance.CachedTypes)
			{
				if (cachedType.Key.Type.Equals(type) && cachedType.Key.Options != TypeSupportOptions.All)
					typesToRemove.Add(cachedType.Key);
			}
			// remove types
			foreach (var typeToRemove in typesToRemove)
				Instance.CachedTypes.Remove(typeToRemove);
		}
	}

	/// <summary>
	/// A composite cache key
	/// </summary>
	public struct CacheKey
	{
		public Type Type;
		public TypeSupportOptions Options;
		public CacheKey(Type type, TypeSupportOptions options)
		{
			Type = type;
			Options = options;
		}

		public override int GetHashCode()
		{
			var hashCode = 23;
			hashCode = hashCode * 31 + (int)Options;
			hashCode = hashCode * 31 + Type.GetHashCode();
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(CacheKey))
				return false;
			var objTyped = (CacheKey)obj;
			return objTyped.Options == Options && objTyped.Type.Equals(Type);
		}

		public override string ToString()
		{
			return $"{(int)Options}-({Options.ToString()}) {Type.FullName}";
		}
	}
}
