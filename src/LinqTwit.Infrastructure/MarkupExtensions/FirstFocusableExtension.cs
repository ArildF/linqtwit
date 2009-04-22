using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace LinqTwit.Infrastructure.MarkupExtensions
{
    public class FirstFocusableExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target =
                (IProvideValueTarget)
                serviceProvider.GetService(typeof(IProvideValueTarget));

            if (target == null)
            {
                return null;
            }

            FrameworkElement fe = target.TargetObject as FrameworkElement;
            if (fe == null)
            {
                return null;
            }

            if (fe.IsLoaded)
            {
                return LeafFocusableChild(fe);
            }

            fe.Loaded += delegate
                {
                    IInputElement element = LeafFocusableChild(fe);
                    if (element != null)
                    {
                        element.Focus(); 
                    }
                };

            return null;

        }

        private static IInputElement LeafFocusableChild(DependencyObject element)
        {
            IInputElement leaf;

            do
            {
                leaf = element as IInputElement;
                element = (DependencyObject)FirstFocusableChild(element);

            } while (element != null);

            return leaf;
        }

        private static IInputElement FirstFocusableChild(DependencyObject element)
        {
            return WalkVisualTree(element)
                .OfType<IInputElement>()
                .FirstOrDefault(item => item.Focusable && item.IsEnabled);
        }

        private static IEnumerable<T> WalkVisualTree<T>(T element) where T : DependencyObject
        {
            if (element == null)
            {
                yield break;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as T;
                if (child != null)
                {
                    yield return child;
                    foreach (var item in WalkVisualTree(child))
                    {
                        yield return item;
                    }

                }
            }
        }
    }
}
