using System;
using Calendar.Model.Compact;
using Calendar.Persistence;
using Calendar.Service.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Service.Controller
{
    public class NewApiController : ControllerBase, IDisposable
    {
        protected readonly ISessionWrapper session;

        public NewApiController()
        {
            session = new LazyLoadedSession(() =>
            {
                var persistSession = DbConfigurationFactory.Get().GetSession();
                if (HttpContext.IsOnImpersonate())
                {
                    return new ReadyOnlySession(persistSession);
                }

                return persistSession;
            });
        }

        protected virtual void DisposeManagedResource()
        {
            
        }

        public void Dispose()
        {
            DisposeManagedResource();
            session?.Dispose();
        }
    }
}