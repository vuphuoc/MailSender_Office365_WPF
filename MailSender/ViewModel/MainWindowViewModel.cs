using MailSender.Helpers;
using MailSender.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MailSender.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {

        #region PROPERTY

        ObservableCollection<FileAttachmentModel> filesAttachment;
        public ObservableCollection<FileAttachmentModel> FilesAttachment
        {
            get { return filesAttachment; }
            set { filesAttachment = value; OnPropertyChanged("FilesAttachment"); }
        }

        bool allowReset;
        public bool AllowReset{
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
        public string MailAddresses
        {
            get { return mailAddresses; }
            set
            {
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

        #endregion

        #region COMMANDS and Event COMMAND
        ICommand commandAddFileAttachment;//CommandAddFileAttachment
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
            //FilesAttachment.Add(new FileAttachmentModel() { FileName = "TrackA", FilePath = "D:\trackA.png" });
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
                    commandSendMail = new RelayCommand<object>(SendMail);
                return commandSendMail;
            }
        }

        private void SendMail(object obj)
        {
            
            SPCMail.Instance.PrepareSPCMail(ConfigurationManager.AppSettings["User"], MailAddresses, MailSubject, DocumentXaml,FilesAttachment.Select(f=>f.FilePath).ToArray());
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

      

        #endregion



        #region CONSTRUCTOR
        public MainWindowViewModel()
        {
            FilesAttachment = new ObservableCollection<FileAttachmentModel>();
            //PrepareData();
            AllowReset = false;
            
        }

        #endregion









        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
