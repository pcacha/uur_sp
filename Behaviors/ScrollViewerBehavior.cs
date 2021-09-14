using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WPF_Malovani.Behaviors
{
    public class ScrollViewerBehavior: Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel += MouseWheelHandler;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();           
            AssociatedObject.PreviewMouseWheel -= MouseWheelHandler;
        }

        private void MouseWheelHandler(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
        }
    }
}
