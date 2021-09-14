using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using WPF_Malovani.ViewModels;

namespace WPF_Malovani.Behaviors
{
    public class PaintBehavior: Behavior<Canvas>
    {
        #region logic from vm
        public MouseActionDelegate MouseDown
        {
            get { return (MouseActionDelegate)GetValue(MouseDownProperty); }
            set { SetValue(MouseDownProperty, value); }
        }        
        public static readonly DependencyProperty MouseDownProperty =
            DependencyProperty.Register("MouseDown", typeof(MouseActionDelegate), typeof(PaintBehavior), new PropertyMetadata(null));


        public MouseActionDelegate MouseUp
        {
            get { return (MouseActionDelegate)GetValue(MouseUpProperty); }
            set { SetValue(MouseUpProperty, value); }
        }       
        public static readonly DependencyProperty MouseUpProperty =
            DependencyProperty.Register("MouseUp", typeof(MouseActionDelegate), typeof(PaintBehavior), new PropertyMetadata(null));

        
        public MouseMoveActionDelegate MouseMove
        {
            get { return (MouseMoveActionDelegate)GetValue(MouseMoveProperty); }
            set { SetValue(MouseMoveProperty, value); }
        }       
        public static readonly DependencyProperty MouseMoveProperty =
            DependencyProperty.Register("MouseMove", typeof(MouseMoveActionDelegate), typeof(PaintBehavior), new PropertyMetadata(null));
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseDown += MouseDownHandler;
            AssociatedObject.MouseUp += MouseUpHandler;
            AssociatedObject.MouseMove += MouseMoveHandler;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseDown -= MouseDownHandler;
            AssociatedObject.MouseUp -= MouseUpHandler;
            AssociatedObject.MouseMove -= MouseMoveHandler;
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            MouseDown((int)e.GetPosition(AssociatedObject).X, (int)e.GetPosition(AssociatedObject).Y);
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            MouseUp((int)e.GetPosition(AssociatedObject).X, (int)e.GetPosition(AssociatedObject).Y);
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {           
            MouseMove((int)e.GetPosition(AssociatedObject).X, (int)e.GetPosition(AssociatedObject).Y, e.MouseDevice.LeftButton); 
        }
    }
}
