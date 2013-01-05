using AikidoWebsite.Data.Entities;
using System;

namespace AikidoWebsite.Data.Security {

    public interface IUserIdentity {
        Benutzer Benutzer { get; }
        bool IsAdmin { get; }
    }
}
