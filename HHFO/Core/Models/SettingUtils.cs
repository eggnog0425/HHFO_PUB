using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using HHFO.Config;
using NLog;

namespace HHFO.Models
{
    public class SettingUtils
    {
        private FileUtils fileUtils = new FileUtils();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public UserSetting getUserSetting()
        {
            var path = GetCommonSetting().UserSettingPath;
            if (!File.Exists(path))
            {
                return null;
            }
            using (var sr = new StreamReader(GetCommonSetting().UserSettingPath, Encoding.UTF8))
            using (var xmlReader = XmlReader.Create(sr))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HHFO.Config.UserSetting));
                return (Config.UserSetting)serializer.Deserialize(xmlReader);
            }
        }

        public bool CreateUserSetting(out string message)
        {
            var fileUtils = new FileUtils();
            var dir = Path.GetDirectoryName(GetCommonSetting().UserSettingPath);
            if (!fileUtils.createDirectory(dir, out var crateDirMessage))
            {
                message = crateDirMessage;
                return false;
            }
            if (!fileUtils.CreateOrUpdateXml<UserSetting>(GetCommonSetting().UserSettingPath, new UserSetting(), out var createXmlMessage))
            {
                message = createXmlMessage;
                return false;
            }
            message = null;
            return true;
        }

        public bool UpdateUserSetting(UserSetting setting, out string message)
        {
            var fileUtils = new FileUtils();
            if (!fileUtils.CreateOrUpdateXml<UserSetting>(GetCommonSetting().UserSettingPath, setting, out var updateXmlMessage))
            {
                message = updateXmlMessage;
                return false;
            }
            message = null;
            return true;
        }

        public CommonSetting GetCommonSetting()
        {
            var path = "HHFO.Core.CommonSetting.xml";
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                return fileUtils.ReadXml<CommonSetting>(stream);
            }
        }

        public ErrorMessage GetErrorMessage()
        {
            var path = GetCommonSetting().ErrorMessagePath;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                return fileUtils.ReadXml<ErrorMessage>(stream);
            }

        }
        public DispMessage GetDispMessage()
        {
            var path = GetCommonSetting().DispMessagePath;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                return fileUtils.ReadXml<DispMessage>(stream);
            }

        }

        internal bool AddAccount(UserSettingUserAccounts account)
        {
            var userSetting = getUserSetting();
            var length = userSetting.UserAccounts == null
                       ? 1
                       : userSetting.UserAccounts.Length + 1;
            UserSettingUserAccounts[] accounts;
            if(length == 1)
            {
                accounts = new UserSettingUserAccounts[] { account };
            } 
            else
            {
                accounts = userSetting.UserAccounts.Concat(new UserSettingUserAccounts[] { account }).ToArray();
            }

            userSetting.UserAccounts = accounts;

            var message = "";
            for (var i = 1; i <= 3; i++)
            {
                if(UpdateUserSetting(userSetting, out message))
                {
                    return true;
                }
            }
            logger.Error("ユーザー設定ファイルへの書き込み失敗");
            logger.Error("書き込み内容: " + userSetting.ToString());
            logger.Error("エラー内容: " + message);
            return false;
        }

        internal bool ChangeDefaultAccount(string defaultId)
        {
            var userSetting = getUserSetting();
            if (userSetting.UserAccounts.Length == 1)
            {
                return true;
            }
            foreach (var account in userSetting.UserAccounts)
            {
                if (account.AccountId == defaultId)
                {
                    continue;
                }
                account.DefaultAccount = false;
            }

            var message = "";
            for (var i = 1; i <= 3; i++)
            {
                if (UpdateUserSetting(userSetting, out message))
                {
                    return true;
                }
            }
            logger.Error("ユーザ設定ファイルの更新に失敗しました。");
            logger.Error("更新内容: デフォルトアカウントの変更");
            logger.Error("更新内容: " + userSetting.ToString());
            logger.Error("エラー内容: " + message);
            return false;
        }
    }
}
