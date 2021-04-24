// -----------------------------------------------------------------------------
//  <copyright file="ObservableObject.cs" company="DCOM Engineering, LLC">
//      Copyright (c) DCOM Engineering, LLC.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Wpf_FFT.MVVM
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An object that supports change notification.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for the property whose name matches <see cref="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">Optional. The name of the property whose value has changed.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}