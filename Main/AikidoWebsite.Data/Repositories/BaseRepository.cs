using AikidoWebsite.Common;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AikidoWebsite.Data.Repositories {

    public abstract class BaseRepository<T> : IRepository<T> {

        [Inject]
        public IDocumentSession Session { get; set; }

        public virtual void Store(T entity) {
            Session.Store(entity);
        }

        public virtual void Store(IEnumerable<T> entities) {
            foreach (var entity in entities) {
                Session.Store(entity);
            }
        }

    }
}
