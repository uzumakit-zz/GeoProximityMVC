using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace GeoProximityMVC.Utils
{
    /// <summary>
    /// TODO : Need to Work on this.
    /// </summary>
    public sealed class CacheUtil
    {
        public static dynamic Get(string key)
        {
            try
            {
                ObjectCache cache = MemoryCache.Default;
                return cache.Get(key);                
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting value from cache");
            }
        }

        public static void Add(string key, dynamic newValue, int timeInMin = 0)
        {
            try
            {
                ObjectCache cache = MemoryCache.Default;

                DateTimeOffset expiration = timeInMin > 0 ? DateTimeOffset.Now.AddMinutes(timeInMin) : 
                    DateTimeOffset.Now.AddDays(3);
                CacheItemPolicy policy =  new CacheItemPolicy {
                    AbsoluteExpiration = expiration                    
                };
                cache.Add(key, newValue, policy);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding value into cache");
            }
        }

        public static void Delete(string key)
        {
            try
            {
                ObjectCache cache = MemoryCache.Default;
                cache.Remove(key);
            }
            catch(Exception ex)
            {
                throw new Exception("Error deleting cache value");
            }
        }
    }
}