using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LinqTwit.Utilities;

namespace LinqTwit.Infrastructure
{

    public abstract class ViewModelBase : IRaisePropertyChanged, INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged;

        void IRaisePropertyChanged.RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this,
                                     new PropertyChangedEventArgs(propName));
            }
        }
    }
}
