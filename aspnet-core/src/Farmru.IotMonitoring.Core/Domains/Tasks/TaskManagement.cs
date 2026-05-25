using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Persons;
using System;

namespace Farmru.IotMonitoring.Domains.Tasks
{
    public class TaskManagement : FullAuditedAggregateRoot<Guid>
    {
        protected TaskManagement()
        {
        }

        public virtual Person AssignedTo { get; private set; }
        public virtual Person AssignedBy { get; private set; }
        public virtual RefListTaskStatus? Status { get; private set; }
        public virtual string Title { get; private set; }
        public virtual string Description { get; private set; }

        public static TaskManagement Create(string title, string description, Person assignedTo, Person assignedBy)
        {
            if (string.IsNullOrWhiteSpace(title?.Trim()))
            {
                throw new DomainRuleException("Task title is required.");
            }

            return new TaskManagement
            {
                Title = title.Trim(),
                Description = description?.Trim(),
                AssignedTo = assignedTo,
                AssignedBy = assignedBy,
                Status = RefListTaskStatus.New
            };
        }

        public virtual void UpdateDetails(string title, string description, Person assignedTo, RefListTaskStatus? status)
        {
            if (!string.IsNullOrWhiteSpace(title?.Trim()))
            {
                Title = title.Trim();
            }

            Description = description?.Trim();
            AssignedTo = assignedTo;
            if (status.HasValue)
            {
                Status = status;
            }
        }

        public virtual void ChangeStatus(RefListTaskStatus status)
        {
            Status = status;
        }
    }
}
