using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AikidoWebsite.Web.Service {

    public class FlickrService {
        private Flickr Flickr;

        public FlickrService() {
            // Todo, in config auslagern
            this.Flickr = new Flickr("128101479@N04");
        }

        public async Task<PhotosetCollection> ListPhotosetsAsync() {
            var t = new TaskCompletionSource<FlickrResult<PhotosetCollection>>();
            
            var photosets = await Flickr.PhotosetsGetListAsync();
            return photosets;
        }

        public async Task<PhotosetPhotoCollection> ListPhotosAsync(string galleryId) {
            return await Flickr.PhotosetsGetPhotosAsync(galleryId);
        }
    }
}