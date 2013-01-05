using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Repositories {

    public interface IRepository<T> {

        void Store(T entity);

        void Store(IEnumerable<T> entities);
    }
}
