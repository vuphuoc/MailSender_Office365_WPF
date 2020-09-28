using MailSender.CustomUC;
using MailSender.Helpers;
using MailSender.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MailSender.ViewModel
{
    class MainWindowViewModel : ObservableObject
    {

        #region PROPERTY

        ObservableCollection<FileAttachmentModel> filesAttachment;
        public ObservableCollection<FileAttachmentModel> FilesAttachment
        {
            get { return filesAttachment; }
            set { filesAttachment = value; OnPropertyChanged("FilesAttachment"); }
        }

        bool allowReset;
        public bool AllowReset {
            get { return allowReset; }
            set { allowReset = value; OnPropertyChanged("AllowReset"); }
        }

        string documentXaml;
        public string DocumentXaml
        {
            get { return documentXaml; }
            set {
                documentXaml = value;
                OnPropertyChanged("DocumentXaml");
            }
        }

        //Email info       
        string mailAddresses;

        //[StringLength(50, MinimumLength = 5, ErrorMessage = "Must be at least 5 characters.")]
        //[RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?",ErrorMessage ="Email is not valid!")]
        public string MailAddresses
        {
            get { return mailAddresses; }
            set
            {
                //ValidateProperty(value, "MailAddresses");
                //OnPropertyChanged(ref mailAddresses, value);
                mailAddresses = value;
                OnPropertyChanged("MailAddresses");
            }
        }

        string mailSubject;
        public string MailSubject
        {
            get { return mailSubject; }
            set
            {
                mailSubject = value;
                OnPropertyChanged("MailSubject");
            }
        }


       
        private string mailCc;
        public string MailCc
        {
            get { return mailCc; }
            set
            {
                mailCc = value;
                OnPropertyChanged(nameof(MailCc));
            }
        }

        private bool checkMailCc;
        public bool CheckMailCc
        {
            get { return checkMailCc; }
            set
            {
                checkMailCc = value;
                OnPropertyChanged(nameof(CheckMailCc));

            }
        }
      

        
        
        public string MailHostUser {
            get { return Properties.Settings.Default._hostUser; }
            set
            {
                Properties.Settings.Default._hostUser = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(MailHostUser));
            }
        }

        public string MailHostPassword
        {
            get { return Properties.Settings.Default._hostPassword; }
            set
            {
                Properties.Settings.Default._hostPassword = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(MailHostPassword));
            }
        }
       
        #endregion

        #region COMMANDS and Event COMMAND
        ICommand commandAddFileAttachment;
        public ICommand CommandAddFileAttachment
        {
            get {
                if (commandAddFileAttachment == null)
                    commandAddFileAttachment = new RelayCommand<object>(AddFileAttachment);
                return commandAddFileAttachment;
            }

        }

        private void AddFileAttachment(object obj)
        {
          
            using (System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                if(fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var size = new FileInfo(fileDialog.FileName).Length;
                    long mb = size / (1024 * 1024);
                    if (mb > 30)
                    {
                        MessageBox.Show("Your file is greater than 30MB, file must be less 30MB!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        FilesAttachment.Add(new FileAttachmentModel() { FileName = System.IO.Path.GetFileName(fileDialog.FileName), FilePath=fileDialog.FileName });
                    }
                   
                }
            }
            AllowReset = true;
        }

        ICommand commandSendMail;
        public ICommand CommandSendMail
        {
            get
            {
                if (commandSendMail == null)
                    commandSendMail = new RelayCommand<object>(CanSendMail,SendMail);
                return commandSendMail;
            }
        }

        private bool CanSendMail(object parameter)
        {
            if (parameter != null)
            {
                var values = parameter as object[];

                var pattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
                string mailInfo = values[0] as string;
                string mailCcInfo = values[1] as string;
                string[] mails = mailInfo.Replace(',',';').Split(';');
                var invalidMail = mails.Any(c => Regex.IsMatch(c, pattern) == false);

              
                if (!string.IsNullOrEmpty(mailCcInfo))
                {
                    string[] mailsCc = mailCcInfo.Replace(',',';').Split(';');
                    var invalidMailCc = mailsCc.Any(c => Regex.IsMatch(c, pattern) == false);
                    if (invalidMail || invalidMailCc)
                        return false;
                    else return true;
                }
                else
                {
                    if (invalidMail)
                        return false;
                    return true;
                }

               
            }
            else
                return false;
        }

        private async void SendMail(object obj)
        {
            string message = "";
            if (string.IsNullOrEmpty(MailAddresses) || string.IsNullOrEmpty(MailSubject))
                message = "Please enter mail addresses and subject!";
            else
            {
                int result = await SPCMail.Instance.PrepareSPCMail(Properties.Settings.Default._hostUser, MailAddresses, MailSubject, DocumentXaml, MailCc, FilesAttachment.Select(f => f.FilePath).ToArray());
                if (result == -1)
                    message = "Something went wrong, please check out mail addresses!";
                else if (result == 1)
                    message = "Mail(s) sent successfully!";
                else message = "There is no mail sent!";
            }
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

        private ICommand commandSaveDefaultMails;
        public ICommand CmdSaveDefaultMails
        {
            get
            {
                if (commandSaveDefaultMails == null)
                    commandSaveDefaultMails = new RelayCommand<object>(CanSendMail, SaveDefaultMails);
                return commandSaveDefaultMails;
            }
            private set
            {
                commandSaveDefaultMails = value;
            }
        }

        private void SaveDefaultMails(object parameter)
        {
            if(parameter != null)
            {
                try
                {
                    var values = parameter as object[];
                    string mails = values[0] as string;
                    string mailsCc = values[1] as string;

                    Properties.Settings.Default.defaultMails = mails;
                    Properties.Settings.Default.defaultMailsCc = mailsCc;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Mail addresses have been saved!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong, please try again later!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
            }
            
        }

        
        private ICommand cmdOpenSettings;
        public ICommand CmdOpenSettings
        {
            get
            {
                if (cmdOpenSettings == null)
                    cmdOpenSettings = new RelayCommand<object>(OpenSettings);
                return cmdOpenSettings;
            }
            private set { cmdOpenSettings = value; }
        }

        private void OpenSettings(object obj)
        {
            Window wd = new Window()
            {
                Content = new HostMailConfigUC(),
                Title = "Mail Configuration",
                Width = 400,
                Height = 250,                
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            wd.ShowDialog();
        }

   
        #endregion

        #region FUNCTIONS

        private ObservableCollection<FileAttachmentModel> PrepareData()
        {
            FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackA", FilePath = "D:\trackA.png" });
            FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackB", FilePath = "D:\trackB.png" });
            FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackC", FilePath = "D:\trackC.png" });
            FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackD", FilePath = "D:\trackD.png" });
            FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackE", FilePath = "D:\trackE.png" });
            FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackF", FilePath = "D:\trackF.png" });
            FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackG", FilePath = "D:\trackG.png" });
            return FilesAttachment;
        }

        private void ValidateProperty<T>(T value, string name)
        {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null)
            {
                MemberName = name
            });
        }

        #endregion



        #region CONSTRUCTOR
        public MainWindowViewModel()
        {
            FilesAttachment = new ObservableCollection<FileAttachmentModel>();
            //PrepareData();
            AllowReset = false;

        
            MailAddresses = Properties.Settings.Default.defaultMails;
        
            MailCc = Properties.Settings.Default.defaultMailsCc;

          
            if(string.IsNullOrEmpty(Properties.Settings.Default._hostUser))
            {
                Window wd = new Window()
                {
                    Content = new HostMailConfigUC(),
                    Title = "Mail Configuration",
                    Width = 400,
                    Height = 250,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                wd.ShowDialog();
            }
            
        }

        #endregion









        //public event PropertyChangedEventHandler PropertyChanged;
        //public void OnPropertyChanged(string propName)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(propName));
        //}
    }
}
