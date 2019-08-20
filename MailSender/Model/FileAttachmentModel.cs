using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.Model
{
    public class FileAttachmentModel
    {
        string filePath;
        string fileName;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
    }
}
