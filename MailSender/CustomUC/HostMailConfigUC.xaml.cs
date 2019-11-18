using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MailSender.CustomUC
{
    /// <summary>
    /// Interaction logic for HostMailConfigUC.xaml
    /// </summary>
    public partial class HostMailConfigUC : UserControl
    {
        //mail regex pattern
        string pattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
        public HostMailConfigUC()
        {
            InitializeComponent();
            btnSaveConfig.IsEnabled = false;
        }

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            

            if(!string.IsNullOrEmpty(txtEmail.Text) && !string.IsNullOrEmpty(txtPass.Text))
            {
                //nếu email hợp lệ
                if(Regex.IsMatch(txtEmail.Text,pattern))
                {
                    try
                    {
                        //lưu thông tin config
                        Properties.Settings.Default._hostUser = txtEmail.Text;
                        Properties.Settings.Default._hostPassword = txtPass.Text;
                        Properties.Settings.Default.Save();

                        MessageBox.Show("Your information has been saved!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);

                        //clear textbox
                        txtEmail.Text = string.Empty;
                        txtPass.Text = string.Empty;
                    }
                    catch (Exception)
                    {
                        
                    }
            
                }
            }
            else
            {
                MessageBox.Show("Please enter your email and password for the configuaration!","Message", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Regex.IsMatch(txtEmail.Text, pattern))
            {
                btnSaveConfig.IsEnabled = true;
            }
        }
    }
}
