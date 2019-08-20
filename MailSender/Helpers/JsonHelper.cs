using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.Helpers
{
    class JsonHelper
    {
        //save file to json
        public static void SaveJsonCheckAccessory_Post_Info(object data, string fileName)
        {
            using (StreamWriter writer = File.CreateText(AppDomain.CurrentDomain.BaseDirectory + @"Chart\"+fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data);
            }

        }

        //read json file as text
        public static string ReadFileToString(string filePath)
        {
            string text = "";
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }
            return text;
        }

        //read json file to object
        public static T ReadJsonCheckAccessory_Post_Info<T>(string fileName)
        {
            T data;
            using (StreamReader reader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\Data\"+ fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                data = (T)serializer.Deserialize(reader, typeof(T));
            }
            return data;
        }
    }
}
