﻿using HHFO.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace HHFO.Models
{
    internal class FileUtils
    {
        private static Logger Logger;

        static FileUtils()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }

        internal bool CreateOrUpdateXml<T>(string path, T data, out string message)
        {
            try
            {
                using (var fs = File.Create(path))
                using (var sw = new StreamWriter(fs))
                {
                    XmlSerializer serializer = new XmlSerializer(data.GetType());
                    serializer.Serialize(sw, data);
                }
                message = null;
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                message = string.Format(new SettingUtils().GetErrorMessage().UnAuthrizeCreateFile, path);
                return false;
            }
        }

        internal T ReadXml<T>(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(sr))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(xmlReader);
            }
        }
        internal T ReadXml<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(sr))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(xmlReader);
            }
        }

        internal bool createDirectory(string path, out string message)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                message = null;
                return true;
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Error("ディレクトリ作成失敗");
                Logger.Error("エラー内容: " + e.Message);
                Logger.Error(e.StackTrace);
                message = string.Format(new SettingUtils().GetErrorMessage().UnAuthrizeCreateDir, path);
                return false;

            }
            catch (Exception e)
            {
                Logger.Error("ディレクトリ作成失敗");
                Logger.Error("エラー内容: " + e.Message);
                Logger.Error(e.StackTrace);
                message = string.Format(new SettingUtils().GetErrorMessage().UnExpectedExceptionCreateDir, path);
                return false;
            }
        }

    }
}
