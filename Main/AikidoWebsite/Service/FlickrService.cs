using System.Threading.Tasks;
using FlickrNet;

namespace AikidoWebsite.Web.Service
{

    public class FlickrService {
        private readonly Flickr Flickr;

        public FlickrService(string apiKey) {
            this.Flickr = new Flickr(apiKey);
        }

        public async Task<PhotosetCollection> ListPhotosetsAsync() {
            return await Flickr.PhotosetsGetListAsync();
        }

        public async Task<PhotosetPhotoCollection> ListPhotosAsync(string galleryId) {
            return await Flickr.PhotosetsGetPhotosAsync(galleryId);
        }
    }
}