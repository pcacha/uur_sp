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
    public class TreeViewBehavior: Behavior<TreeView>
    {
        public Action<string> DirectoryUpdate
        {
            get { return (Action<string>)GetValue(DirectoryUpdateProperty); }
            set { SetValue(DirectoryUpdateProperty, value); }
        }

        
        public static readonly DependencyProperty DirectoryUpdateProperty =
            DependencyProperty.Register("DirectoryUpdate", typeof(Action<string>), typeof(TreeViewBehavior), new PropertyMetadata(null));


        protected override void OnAttached()
        {
            base.OnDetaching();
            AssociatedObject.SelectedItemChanged += SelectedItemChangedHandler;            
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectedItemChanged -= SelectedItemChangedHandler;           
        }

        private void SelectedItemChangedHandler(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DirectoryUpdate(((sender as TreeView).SelectedItem as TreeViewItem).Tag.ToString());
        }
    }
}
