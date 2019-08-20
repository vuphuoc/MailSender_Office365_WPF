using MailSender.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace MailSender.Controls.FileAttachment
{
    /// <summary>
    /// Interaction logic for FileAttachment.xaml
    /// </summary>
    public partial class FileAttachment : UserControl,INotifyPropertyChanged
    { 

        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(ObservableCollection<FileAttachmentModel>), typeof(FileAttachment),new PropertyMetadata(new PropertyChangedCallback(OnChanged)));       
        public ObservableCollection<FileAttachmentModel> ItemSource
        {
            get { return (ObservableCollection<FileAttachmentModel>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value);
                OnPropertyChanged("ItemSource");
                
            }
        }
        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileAttachment fa = (FileAttachment)d;
            fa.GenerateFileItem(fa.ItemSource);
        }
        public static readonly DependencyProperty IsResetProperty = DependencyProperty.Register("IsReset", typeof(bool), typeof(FileAttachment), new PropertyMetadata(new PropertyChangedCallback(IsResetOnChanged)));

        private static void IsResetOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileAttachment fa = (FileAttachment)d;
            fa.GenerateFileItem(fa.ItemSource);
        }

        public bool IsReset
        {
            get { return (bool)GetValue(IsResetProperty); }
            set
            {
                SetValue(IsResetProperty, value);
                OnPropertyChanged("IsReset");
                DoReset();
            }
        }

        public FileAttachment()
        {
            InitializeComponent();                        
        }

        public void DoReset()
        {
           
        }


       

        void GenerateFileItem(ObservableCollection<FileAttachmentModel> lstFileAttachment)
        {
            if (lstFileAttachment != null && lstFileAttachment.Count>0)
            {
                if (wrapPanel.Children != null)
                    wrapPanel.Children.Clear();

                int i = 1;
                foreach (var item in lstFileAttachment)
                {
                    StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center,Name="stack"+i };
                    TextBlock tbFileName = new TextBlock() { Text = item.FileName };
                    Button btFilePath = new Button() { Content = "X", Tag = item.FilePath };
                    btFilePath.Click += BtFilePath_Click;
                    sp.Children.Add(tbFileName);
                    sp.Children.Add(btFilePath);
                    sp.Style = Application.Current.FindResource("stackFileItem") as Style;
                    wrapPanel.Children.Add(sp);
                    i++;
                }
                IsReset = false;
            }
       
        }

        private void BtFilePath_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            string filePath = bt.Tag.ToString();           
            ItemSource.Remove(ItemSource.Where(c=>c.FilePath.Equals(filePath)).FirstOrDefault());
            //find the control parent that wrapped this button and other control
            //Ex: var parent = FindParent<DependencyObject>(bt, "wrapPanel");
            var parent = FindParent<DependencyObject>(bt);
            if(parent != null)
            {
                wrapPanel.Children.Remove(parent as StackPanel);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }


        /// <summary>
        /// Recursively finds the specified named parent in a control hierarchy
        /// </summary>
        /// <typeparam name="T">The type of the targeted Find</typeparam>
        /// <param name="child">The child control to start with</param>
        /// <param name="parentName">The name of the parent to find</param>
        /// <returns></returns>
        private static T FindParent<T>(DependencyObject child, string parentName)
            where T : DependencyObject
        {
            if (child == null) return null;

            T foundParent = null;
            var currentParent = VisualTreeHelper.GetParent(child);

            do
            {
                var frameworkElement = currentParent as FrameworkElement;
                if (frameworkElement.Name == parentName && frameworkElement is T)
                {
                    foundParent = (T)currentParent;
                    break;
                }

                currentParent = VisualTreeHelper.GetParent(currentParent);

            } while (currentParent != null);

            return foundParent;
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {

            T parent = VisualTreeHelper.GetParent(child) as T;

            if (parent != null)

                return parent;

            else

                return FindParent<T>(parent);

        }
    }
}
