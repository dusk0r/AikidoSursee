using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlickrNet;

namespace AikidoWebsite.Web.Service
{

    public class FlickrService {
        private readonly Flickr Flickr;
        private readonly IDictionary<string, (DateTime, object)> Cache = new Dictionary<string, (DateTime, object)>();
        private static readonly TimeSpan CacheTime = TimeSpan.FromHours(1);

        public FlickrService(string apiKey) {
            this.Flickr = new Flickr(apiKey);
        }

        public Task<PhotosetCollection> ListPhotosetsAsync() {
            return TryGetFromCacheAsync<PhotosetCollection>("photosets", () => Flickr.PhotosetsGetListAsync("128101479@N04", perPage: 500));
        }

        public Task<PhotosetPhotoCollection> ListPhotosAsync(string galleryId) {
            return TryGetFromCacheAsync<PhotosetPhotoCollection>(galleryId, () => Flickr.PhotosetsGetPhotosAsync(galleryId, perPage: 500));
        }

        private async Task<T> TryGetFromCacheAsync<T>(string key, Func<Task<T>> getFunc)
        {
            if (Cache.TryGetValue(key, value: out (DateTime creationDate, object entry) value) && DateTime.Now - value.creationDate < CacheTime)
            {
                return (T)value.entry;
            }
            
            var result = await getFunc();
            Cache[key] = (DateTime.Now, result);
            return result;
        }
    }
}