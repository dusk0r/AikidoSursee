using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AikidoWebsite.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;

namespace AikidoWebsite.Web.Security
{
    public class RavenUserProvider : IUserStore<Benutzer>, IUserRoleStore<Benutzer>/*, IUserEmailStore<Benutzer>, IQueryableUserStore<Benutzer> , IRoleStore<Role>*/
    {
        private IDocumentStore DocumentStore { get; }

        public RavenUserProvider(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        public async Task<IdentityResult> CreateAsync(Benutzer user, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                await asyncDocumentSession.StoreAsync(user);
                await asyncDocumentSession.SaveChangesAsync();
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Benutzer user, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                var loaded = await asyncDocumentSession.LoadAsync<Benutzer>(user.Id);
                if (loaded != null)
                {
                    asyncDocumentSession.Delete(loaded);
                    await asyncDocumentSession.SaveChangesAsync();
                    return IdentityResult.Success;
                }
            }

            return IdentityResult.Failed(new IdentityError { Description = $"Benutzer '{user.Id}' nicht gefunden" });
        }

        public async Task<Benutzer> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                return await asyncDocumentSession.LoadAsync<Benutzer>(userId);
            }
        }

        public async Task<Benutzer> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                return await asyncDocumentSession.Query<Benutzer>().FirstOrDefaultAsync(b => b.Username == normalizedUserName);
            }
        }

        public Task<string> GetNormalizedUserNameAsync(Benutzer user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task<string> GetUserIdAsync(Benutzer user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(Benutzer user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public async Task SetNormalizedUserNameAsync(Benutzer user, string normalizedName, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                var loaded = await asyncDocumentSession.LoadAsync<Benutzer>(user.Id);
                loaded.Username = normalizedName;
                await asyncDocumentSession.SaveChangesAsync();
            }
        }

        public async Task SetUserNameAsync(Benutzer user, string userName, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                var loaded = await asyncDocumentSession.LoadAsync<Benutzer>(user.Id);
                loaded.Username = userName;
                await asyncDocumentSession.SaveChangesAsync();
            }
        }

        public async Task<IdentityResult> UpdateAsync(Benutzer user, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                await asyncDocumentSession.StoreAsync(user);
                await asyncDocumentSession.SaveChangesAsync();
                return IdentityResult.Success;
            }
        }

        public void Dispose()
        {
        }

        public async Task AddToRoleAsync(Benutzer user, string roleName, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                var loaded = await asyncDocumentSession.LoadAsync<Benutzer>(user.Id);
                loaded.Gruppen.Add(roleName);
                await asyncDocumentSession.SaveChangesAsync();
            }
        }

        public async Task RemoveFromRoleAsync(Benutzer user, string roleName, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                var loaded = await asyncDocumentSession.LoadAsync<Benutzer>(user.Id);
                loaded.Gruppen.Remove(roleName);
                await asyncDocumentSession.SaveChangesAsync();
            }
        }

        public async Task<IList<string>> GetRolesAsync(Benutzer user, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                var loaded = await asyncDocumentSession.LoadAsync<Benutzer>(user.Id);
                return loaded.Gruppen.ToList();
            }
        }

        public async Task<bool> IsInRoleAsync(Benutzer user, string roleName, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                var loaded = await asyncDocumentSession.LoadAsync<Benutzer>(user.Id);
                return loaded.Gruppen.Contains(roleName);
            }
        }

        public async Task<IList<Benutzer>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            using (var asyncDocumentSession = DocumentStore.OpenAsyncSession())
            {
                return await asyncDocumentSession.Query<Benutzer>().Where(b => b.Gruppen.Any(x => x == roleName)).ToListAsync();
            }
        }
    }
}
