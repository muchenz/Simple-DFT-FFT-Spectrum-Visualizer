// -----------------------------------------------------------------------------
//  <copyright file="ViewModel.cs" company="DCOM Engineering, LLC">
//      Copyright (c) DCOM Engineering, LLC.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Wpf_FFT.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Base class for a View-Model in the MVVM pattern.
    /// </summary>
    public abstract class ViewModel : ObservableObject, IDataErrorInfo
    {
        /// <summary>
        /// Gets the validation error for a property whose name matches the specified <see cref="columnName"/>.
        /// </summary>
        /// <param name="columnName">The name of the property to validate.</param>
        /// <returns>Returns a validation error if there is one, otherwise returns null.</returns>
        public string this[string columnName]
        {
            get { return OnValidate(columnName); }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        [Obsolete]
        public string Error
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Validates a property whose name matches the specified <see cref="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>Returns a validation error, if any, otherwise returns null.</returns>
        protected virtual string OnValidate(string propertyName)
        {
            var context = new ValidationContext(this);

            var results = new Collection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(this, context, results, true);

            if (!isValid)
            {
                ValidationResult result = results.Where(p => p.MemberNames.First()
                                                == propertyName).FirstOrDefault();

                //ValidationResult result = results.SingleOrDefault(p =>
                //                                                p.MemberNames.Any(memberName =>
                //                                                                  memberName == propertyName));
                return result == null ? null : result.ErrorMessage;
            }

            return null;
        }

        protected virtual string OnValidate(IDictionary<object, object> property)
        {
            var context = new ValidationContext(this, property);

            var results = new Collection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(this, context, results, true);

            if (!isValid)
                foreach (var r in results)
                {
                    if (r.MemberNames.Any(member => property.Keys.Any(key => key.ToString() == member)))
                        return results[0].ErrorMessage;

                }
            return null;
        }
    }
}