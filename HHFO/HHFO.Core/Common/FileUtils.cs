using HHFO.Config;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using HHFO.Core;

namespace HHFO.Core
{
    public class FileUtils
    {
        private static Logger Logger;

        static FileUtils()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        public (bool success, String dispErrMessage) CreateXml<T>(string path, T data)
        {
            try
            {
                using (var fs = File.Create(path))
                using (var sw = new StreamWriter(fs))
                {
                    XmlSerializer serializer = new XmlSerializer(data.GetType());
                    serializer.Serialize(sw, data);
                }
                return (true, null);
            }
            catch (UnauthorizedAccessException)
            {
                return (false, string.Format(ErrorMessage.value.UnAuthrizeCreateFile, path));
            }
        }

        public T ReadXml<T>(string path, Type obj)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(sr))
            {
                XmlSerializer serializer = new XmlSerializer(obj);
                return (T)serializer.Deserialize(xmlReader);
            }
        }
        public T ReadXml<T>(Stream stream, Type obj)
        {
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(sr))
            {
                XmlSerializer serializer = new XmlSerializer(obj);
                return (T)serializer.Deserialize(xmlReader);
            }
        }

        public (bool b, string message) createDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return (true, null);
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Error("ディレクトリ作成失敗");
                Logger.Error("エラー内容: " + e.Message);
                Logger.Error(e.StackTrace);
                return (false, string.Format(ErrorMessage.value.UnAuthrizeCreateDir, path));

            }
            catch (Exception e)
            {
                Logger.Error("ディレクトリ作成失敗");
                Logger.Error("エラー内容: " + e.Message);
                Logger.Error(e.StackTrace);
                return (false, string.Format(ErrorMessage.value.UnExpectedExceptionCreateDir, path));
            }
        }

        public UserSetting getUserSetting()
        {
            var commonSetting = CommonSetting.Value;
            using (var sr = new StreamReader(commonSetting.UserSettingPath, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(sr))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HHFO.Config.UserSetting));
                return (UserSetting)serializer.Deserialize(xmlReader);
            }
        }
    }
}
