using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

// stolen from http://www.wpfmentor.com/2008/11/adding-bindings-to-clr-properties-in.html

namespace LinqTwit.Infrastructure
{
    public class DataResource : Freezable
    {
        /// <summary>
        /// Identifies the <see cref="BindingTarget"/> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="BindingTarget"/> dependency property.
        /// </value>
        public static readonly DependencyProperty BindingTargetProperty = 
            DependencyProperty.Register("BindingTarget", typeof(object), typeof(DataResource), new UIPropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResource"/> class.
        /// </summary>
        public DataResource()
        {
        }

        /// <summary>
        /// Gets or sets the binding target.
        /// </summary>
        /// <value>The binding target.</value>
        public object BindingTarget
        {
            get { return GetValue(BindingTargetProperty); }
            set { SetValue(BindingTargetProperty, value); }
        }

        /// <summary>
        /// Creates an instance of the specified type using that type's default constructor. 
        /// </summary>
        /// <returns>
        /// A reference to the newly created object.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return (Freezable)Activator.CreateInstance(GetType());
        }

        /// <summary>
        /// Makes the instance a clone (deep copy) of the specified <see cref="Freezable"/>
        /// using base (non-animated) property values. 
        /// </summary>
        /// <param name="sourceFreezable">
        /// The object to clone.
        /// </param>
        protected sealed override void CloneCore(Freezable sourceFreezable)
        {
            base.CloneCore(sourceFreezable);
        }
    }

    public class DataResourceBindingExtension : MarkupExtension
    {
        private object mTargetObject;
        private object mTargetProperty;
        private DataResource mDataResouce;

        /// <summary>
        /// Gets or sets the data resource.
        /// </summary>
        /// <value>The data resource.</value>
        public DataResource DataResource
        {
            get
            {
                return this.mDataResouce;
            }
            set
            {
                if (this.mDataResouce != value)
                {
                    if (this.mDataResouce != null)
                    {
                        this.mDataResouce.Changed -= DataResource_Changed;
                    }
                    this.mDataResouce = value;

                    if (this.mDataResouce != null)
                    {
                        this.mDataResouce.Changed += DataResource_Changed;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResourceBindingExtension"/> class.
        /// </summary>
        public DataResourceBindingExtension()
        {
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            this.mTargetObject = target.TargetObject;
            this.mTargetProperty = target.TargetProperty;

            // mTargetProperty can be null when this is called in the Designer.
            Debug.Assert(this.mTargetProperty != null || DesignerProperties.GetIsInDesignMode(new DependencyObject()));

            if (this.DataResource.BindingTarget == null && this.mTargetProperty != null)
            {
                PropertyInfo propInfo = this.mTargetProperty as PropertyInfo;
                if (propInfo != null)
                {
                    try
                    {
                        // extra support for commands here. Some classes don't allow a null
                        // command for example InputBinding. In this case we use a dummy
                        // command (NullCommand) instead of setting a null value.

                        if (propInfo.PropertyType.IsAssignableFrom(typeof(ICommand)))
                        {
                            return new NullCommand();
                        }

                        return Activator.CreateInstance(propInfo.PropertyType);
                    }
                    catch (MissingMethodException)
                    {
                        // there isn't a default constructor
                    }
                }

                DependencyProperty depProp = this.mTargetProperty as DependencyProperty;
                if (depProp != null)
                {
                    DependencyObject depObj = (DependencyObject)this.mTargetObject;
                    return depObj.GetValue(depProp);
                }
            }

            return this.DataResource.BindingTarget;
        }

        private void DataResource_Changed(object sender, EventArgs e)
        {
            // Ensure that the bound object is updated when DataResource changes.
            DataResource dataResource = (DataResource)sender;
            DependencyProperty depProp = this.mTargetProperty as DependencyProperty;

            if (depProp != null)
            {
                DependencyObject depObj = (DependencyObject)this.mTargetObject;
                object value = Convert(dataResource.BindingTarget, depProp.PropertyType);
                depObj.SetValue(depProp, value);
            }
            else
            {
                PropertyInfo propInfo = this.mTargetProperty as PropertyInfo;
                if (propInfo != null)
                {
                    object value = Convert(dataResource.BindingTarget, propInfo.PropertyType);

                    // extra support for commands here. Some classes don't allow a null
                    // command for example InputBinding. In this case we use a dummy
                    // command (NullCommand) instead of setting a null value.

                    if (value == null && propInfo.PropertyType.IsAssignableFrom(typeof(ICommand)))
                    {
                        value = new NullCommand();
                    }

                    propInfo.SetValue(this.mTargetObject, value, new object[0]);
                }
            }
        }

        private static object Convert(object obj, Type toType)
        {
            try
            {
                if (!toType.IsAssignableFrom(obj.GetType()))
                {
                    return System.Convert.ChangeType(obj, toType);
                }
                
            }
            catch (InvalidCastException)
            {
                return obj;
            }
            return obj;
        }

        private class NullCommand : ICommand
        {
            #region ICommand Members

            public bool CanExecute(object parameter)
            {
                return false;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
            }

            #endregion
        }
    }
}