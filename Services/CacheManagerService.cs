using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;

namespace GoodBytes.Infrastructure.CacheManager
{
	public class CacheManagerService : ICacheManagerInterface
	{
		private bool CacheDisabled;
		private int CacheExpireTime;

		public CacheManagerService()
		{
			var cacheDisabled = ConfigurationManager.AppSettings["CacheWorking"];
			if (cacheDisabled == null || cacheDisabled == string.Empty || cacheDisabled == "Off")
				CacheDisabled = false;
			else
				CacheDisabled = true;

			var cacheExpireTime = ConfigurationManager.AppSettings["CacheMinutes"];
			if (cacheExpireTime == null || cacheExpireTime == string.Empty)
				CacheExpireTime = 0;
			else
				CacheExpireTime = Convert.ToInt32(cacheExpireTime);
		}

		private ObjectCache Cache
		{
			get { return MemoryCache.Default; }
		}

		public T Get<T>(string key)
		{
			return (T)Cache[key];
		}

		public List<T> GetList<T>(string key)
		{
			return (List<T>)Cache[key];
		}

		public void Set(string key, object data)
		{
			if (data == null)
				return;

			var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(CacheExpireTime) };

			Cache.Add(new CacheItem(key, data), policy);
		}

		public bool IsSet(string key)
		{
			if (CacheDisabled)
				return false;
			return Cache.Contains(key);
		}

		public void Remove(string key)
		{
			Cache.Remove(key);
		}

		public void Clear()
		{
			foreach (var item in Cache)
				Remove(item.Key);
		}

		public List<string> GetListKeys()
		{
			return Cache.Select(item => item.Key).ToList();
		}

		public void RemoveWithPrefix(string prefix)
		{
			var keysToRemove = GetKeysToRemoveWithPrefix(prefix);
			foreach (var key in keysToRemove)
				Remove(key);
		}

		private List<string> GetKeysToRemoveWithPrefix(string prefix)
		{
			return Cache.Where(x => x.Key.StartsWith(prefix)).Select(x => x.Key).ToList();
		}
	}
}
