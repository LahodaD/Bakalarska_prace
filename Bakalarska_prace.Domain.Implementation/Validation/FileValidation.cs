using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bakalarska_prace.Domain.Implementation.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class FileValidation : ValidationAttribute, IClientModelValidator
    {
        string contentType;
        public FileValidation(string contentType)
        {
            this.contentType = contentType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else if (value is IFormFile formFile)
            {
                if (contentType.ToLower().Contains(formFile.ContentType.ToLower()))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"The {validationContext.MemberName} field is not .docx/.xlsx/.pdf.");
                }
            }
            else
            {
                throw new NotImplementedException($"The {nameof(FileValidation)} is not implemented for the type: {value.GetType()}");
            }
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-filecontent", $"The {context.ModelMetadata.Name} field is not .docx/.xlsx/.pdf.");
            context.Attributes.Add("data-val-filecontent-type", contentType);
        }
    }
}
