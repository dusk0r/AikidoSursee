using System.Threading.Tasks;
using FlickrNet;

namespace AikidoWebsite.Web.Service
{

    public class FlickrService {
        private readonly Flickr Flickr;

        public FlickrService(string apiKey) {
            this.Flickr = new Flickr(apiKey);
        }

        public Task<PhotosetCollection> ListPhotosetsAsync() {
            var t = new TaskCompletionSource<PhotosetCollection>();
            Flickr.PhotosetsGetListAsync("128101479@N04", r => t.TrySetResult(r.Result));
            return t.Task;
        }

        public Task<PhotosetPhotoCollection> ListPhotosAsync(string galleryId) {
            var t = new TaskCompletionSource<PhotosetPhotoCollection>();
            Flickr.PhotosetsGetPhotosAsync(galleryId, r => t.TrySetResult(r.Result));
            return t.Task;
        }
    }
}