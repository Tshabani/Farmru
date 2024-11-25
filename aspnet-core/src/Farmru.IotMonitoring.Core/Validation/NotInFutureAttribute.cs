using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Validation
{
    public class NotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (!ValidationHelper.IsNotInFuture(dateValue))
                {
                    return new ValidationResult("The date of birth cannot be in the future.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
