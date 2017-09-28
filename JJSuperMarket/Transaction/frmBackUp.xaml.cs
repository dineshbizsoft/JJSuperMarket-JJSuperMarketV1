using JJSuperMarket.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JJSuperMarket.Transaction
{
    /// <summary>
    /// Interaction logic for frmBackUp.xaml
    /// </summary>
    public partial class frmBackUp : UserControl
    {
        public frmBackUp()
        {
            InitializeComponent();
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLocation.Clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtLocation.Text))
                {
                    MessageBox.Show("Please select Database Destination Folder !", "Save Path is Empty");
                    txtLocation.Focus();
                    return;
                }
                else
                {

                    System.Windows.Forms.Application.DoEvents();
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                    using (SqlConnection con = new SqlConnection(AppLib.conStr))
                    {
                        string str = " BACKUP DATABASE JJSuperMarket \r" +
                                     " TO DISK = '" + txtLocation.Text + "'";

                        System.Windows.Forms.Application.DoEvents();
                        SqlCommand cmd = new SqlCommand(str, con);
                        cmd.Connection.Open();
                        cmd.CommandTimeout = 0;

                        int i = cmd.ExecuteNonQuery();

                        System.Windows.Forms.Application.DoEvents();
                        if (i == 0)
                        {
                            MessageBox.Show("Backup Not Execute!", "Error");
                        }
                        else
                        {
                            MessageBox.Show("Backup Completed Successfully!", "Executed");
                        }
                        cmd.Connection.Close();
                    }
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                    txtLocation.Clear();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Backup Not Execute! Choose Different Location.", "Error");

            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.InitialDirectory = Environment.CurrentDirectory;
                dlg.Title = "JJSuperMarket Back Up";
                dlg.Filter = "Bakup files|*.bak;*.BAK|All files|*.*";
                dlg.FileName = "JJSuperMarket" + string.Format("{0:ddMMyyy}", DateTime.Today) + ".bak";
                if (dlg.ShowDialog() == true)
                {
                    txtLocation.Text = dlg.FileName;
                }
            }
            catch (Exception ex)
            {


            }
        }

    }
}
