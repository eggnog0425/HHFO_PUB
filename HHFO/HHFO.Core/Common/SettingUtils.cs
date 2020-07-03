using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using HHFO.Config;
namespace HHFO.Core.Common
{
    public class SettingUtils
    {
        private FileUtils fileUtils = new FileUtils();

        public UserSetting getUserSetting()
        {
            var path = getCommonSetting().UserSettingPath;
            if (!File.Exists(path))
            {
                return null;
            }
            using (var sr = new StreamReader(getCommonSetting().UserSettingPath, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(sr))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HHFO.Config.UserSetting));
                return (Config.UserSetting)serializer.Deserialize(xmlReader);
            }
        }

        public bool createUserSetting(out string message)
        {
            var fileUtils = new FileUtils();
            var dir = Path.GetDirectoryName(getCommonSetting().UserSettingPath);
            if (!fileUtils.createDirectory(dir, out var crateDirMessage))
            {
                message = crateDirMessage;
                return false;
            }
            if (!fileUtils.CreateOrUpdateXml<UserSetting>(getCommonSetting().UserSettingPath, new UserSetting(), out var createXmlMessage))
            {
                message = createXmlMessage;
                return false;
            }
            message = null;
            return true;
        }

        public bool updateUserSetting(UserSetting setting, out string message)
        {
            var fileUtils = new FileUtils();
            if (!fileUtils.CreateOrUpdateXml<UserSetting>(getCommonSetting().UserSettingPath, new UserSetting(), out var updateXmlMessage))
            {
                message = updateXmlMessage;
                return false;
            }
            message = null;
            return true;
        }

        public CommonSetting getCommonSetting()
        {
            var path = "HHFO.Core.CommonSetting.xml";
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                return fileUtils.ReadXml<CommonSetting>(stream);
            }
        }

        public ErrorMessage getErrorMessage()
        {
            var path = getCommonSetting().ErrorMessagePath;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                return fileUtils.ReadXml<ErrorMessage>(stream);
            }

        }
        public DispMessage getDispMessage()
        {
            var path = getCommonSetting().DispMessagePath;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                return fileUtils.ReadXml<DispMessage>(stream);
            }

        }
    }
}
