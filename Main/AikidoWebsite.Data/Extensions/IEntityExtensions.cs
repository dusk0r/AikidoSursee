using AikidoWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data {
    
    public static class IEntityExtensions {

        public static bool IsNew(this IEntity entity) {
            return entity.Id == null;
        }
    }
}
