﻿using FlickrNet;
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
            this.Flickr = new Flickr("eb86f95fb5474b06d3ecb2a3f6bf02aa");
        }

        public Task<FlickrResult<PhotosetCollection>> ListPhotosets() {
            var t = new TaskCompletionSource<FlickrResult<PhotosetCollection>>();
            // Todo, in config auslagern
            Flickr.PhotosetsGetListAsync("128101479@N04", r => t.TrySetResult(r));
            return t.Task;
        }

        public PhotosetPhotoCollection ListPhotos(string galleryId) {
            return Flickr.PhotosetsGetPhotos(galleryId);
        }
    }
}