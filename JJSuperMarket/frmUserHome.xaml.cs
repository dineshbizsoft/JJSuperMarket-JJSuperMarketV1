using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JJSuperMarket.Domain;
using MahApps.Metro.Controls;

namespace JJSuperMarket
{
    /// <summary>
    /// Interaction logic for frmUserHome.xaml
    /// </summary>
    public partial class frmUserHome : Window
    {
        bool isShowCloseConform = true;


        public frmUserHome()
        {
            InitializeComponent();
            ccContent.Content = new frmWelcome();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            ListBox lb = sender as ListBox;
            DemoItem di = lb.SelectedItem as DemoItem;
            ccContent.Content = di.Content;
            MenuToggleButton.IsChecked = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            frmLogin frm = new frmLogin();
            frm.Show();
            this.Close();
        }

        public void ShowForm(object o)
        {
            ccContent.Content = o;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.Show();
            this.Close();
            
        }
    }
}
