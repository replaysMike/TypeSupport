using System;
using System.Collections.Generic;
using System.Threading;
using TypeSupport.Extensions;
using TypeSupport.Assembly;

namespace TypeSupport
{
	/// <summary>
	/// Caches extended types
	/// </summary>
	public sealed class ExtendedTypeCache
	{
		private static readonly SemaphoreSlim _cacheLock = new (1);
		private static readonly ExtendedTypeCache _instance = new ();

		/// <summary>
		/// Get an instance of the type cache
		/// </summary>
		public static ExtendedTypeCache Instance => _instance;

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
            var isCachingSupported = options.BitwiseHasFlag(TypeSupportOptions.Caching);
            if (!isCachingSupported)
                return null;

            _cacheLock.Wait();
			try
			{
                if (!ContainsInternal(type, options))
                    return null;
				var key = GenerateKey(type, options);
				return Instance.CachedTypes[key];
			}
			finally
			{
				_cacheLock.Release();
			}
		}

        /// <summary>
		/// Get an extended type from the cache, or create it if it does not exist
		/// </summary>
		/// <param name="type">The type to lookup</param>
		/// <param name="options">The options for the extended type</param>
		/// <returns></returns>
		public static ExtendedType GetOrCreate(Type type)
            => GetOrCreate(type, TypeSupportOptions.All);

        /// <summary>
		/// Get an extended type from the cache, or create it if it does not exist
		/// </summary>
		/// <param name="type">The type to lookup</param>
		/// <param name="options">The options for the extended type</param>
		/// <returns></returns>
		public static ExtendedType GetOrCreate(Type type, TypeSupportOptions options)
        {
            var isCachingSupported = options.BitwiseHasFlag(TypeSupportOptions.Caching);
            if (!isCachingSupported)
                return new ExtendedType(type, options);

            if (type == null)
                throw new ArgumentNullException(nameof(type));
            _cacheLock.Wait();
            try
            {
                if (!ContainsInternal(type, options))
                {
                    var newExtendedType = new ExtendedType(type, options);
                    // cache the type
                    CacheTypeInternal(newExtendedType, options);
                    return newExtendedType;
                }
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
            var isCachingSupported = options.BitwiseHasFlag(TypeSupportOptions.Caching);
            if (!isCachingSupported)
                return;
            _cacheLock.Wait();
			try
			{
                CacheTypeInternal(type, options);
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
        private static void CacheTypeInternal(ExtendedType type, TypeSupportOptions options)
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

        /// <summary>
        /// Returns true if the extended type is cached
        /// </summary>
        /// <param name="type">The type to lookup</param>
        /// <param name="options">The options for the extended type</param>
        /// <returns></returns>
        private static bool ContainsInternal(Type type, TypeSupportOptions options)
		{
            var isCachingSupported = options.BitwiseHasFlag(TypeSupportOptions.Caching);
            if (!isCachingSupported)
                return false;
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
}
