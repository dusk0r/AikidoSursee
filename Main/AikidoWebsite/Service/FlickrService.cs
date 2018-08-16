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
            return TryGetFromCacheAsync<PhotosetCollection>("photosets", t => Flickr.PhotosetsGetListAsync("128101479@N04", r => t.TrySetResult(r.Result)));
        }

        public Task<PhotosetPhotoCollection> ListPhotosAsync(string galleryId) {
            return TryGetFromCacheAsync<PhotosetPhotoCollection>(galleryId, t => Flickr.PhotosetsGetPhotosAsync(galleryId, r => t.TrySetResult(r.Result)));
        }

        private async Task<T> TryGetFromCacheAsync<T>(string key, Action<TaskCompletionSource<T>> getFunc)
        {
            if (Cache.TryGetValue(key, value: out (DateTime creationDate, object entry) value) && DateTime.Now - value.creationDate < CacheTime)
            {
                return (T)value.entry;
            } else
            {
                var t = new TaskCompletionSource<T>();
                getFunc(t);
                var result = await t.Task;
                Cache[key] = (DateTime.Now, result);
                return result;
            }
        }
    }
}