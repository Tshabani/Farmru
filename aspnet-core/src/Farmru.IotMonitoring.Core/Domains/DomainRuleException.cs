using System;

namespace Farmru.IotMonitoring.Domains
{
    public class DomainRuleException : Exception
    {
        public DomainRuleException(string message) : base(message)
        {
        }
    }
}
