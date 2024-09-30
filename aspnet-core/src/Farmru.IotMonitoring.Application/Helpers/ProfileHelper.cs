using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Helpers
{
    public class ProfileHelper : Profile
    {
        public static T GetEntity<T, TId>(TId id) where T : class, IEntity<TId>
        {
            if (id == null || id is Guid guid && guid == Guid.Empty)
                return null;

            var repo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<T, TId>>();
            return repo.Get(id);
        }
        protected static T GetEntity<T>(EntityWithDisplayNameDto<Guid?> dto) where T : class, IEntity<Guid>
        {
            if (dto?.Id == null || dto.Id == Guid.Empty)
                return null;

            var repo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<T, Guid>>();
            return repo.Get(dto.Id.Value);
        }


        protected static T GetEntity<T>(Guid? id) where T : class, IEntity<Guid>
        {
            if (id == null || id == Guid.Empty)
                return null;

            var repo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<T, Guid>>();
            return repo.Get(id.Value);
        }
    }
}
