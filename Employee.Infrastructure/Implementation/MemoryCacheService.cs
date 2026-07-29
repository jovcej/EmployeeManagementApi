using Employee.Core.Abstraction;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Infrastructure.Implementation
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
        }
    }
}
