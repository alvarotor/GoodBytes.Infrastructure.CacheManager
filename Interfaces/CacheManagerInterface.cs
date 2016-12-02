using System.Collections.Generic;

namespace GoodBytes.Infrastructure.CacheManager
{
	public interface ICacheManagerInterface
	{
		T Get<T>(string key);

		List<T> GetList<T>(string key);

		void Set(string key, object data);

		bool IsSet(string key);

		void Remove(string key);

		void Clear();

		List<string> GetListKeys();

		void RemoveWithPrefix(string prefix);
	}
}