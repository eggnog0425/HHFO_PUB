using System;
using System.Collections.Generic;
using System.Text;
using CoreTweet;
using HHFO.Models;
using System.Linq;
using System.Diagnostics;
using CoreTweet.Rest;
using HHFO.Config;
using System.Windows;
using NLog;

namespace HHFO.Models
{
    public class Authorization
    {
        private static Tokens Token = null;
        private static readonly string ConsumerKey;
        private static readonly string ConsumerSecret;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private OAuth.OAuthSession Session { get; set; }

        static Authorization()
        {
            var setting = new SettingUtils().GetCommonSetting();
            ConsumerKey = setting.ConsumerKey;
            ConsumerSecret = setting.ConsumerSecret;
        }

        public static OAuth.OAuthSession Authorize()
        {
            return CoreTweet.OAuth.Authorize(ConsumerKey, ConsumerSecret);
        }

        public static Tokens GetToken()
        {
            if (Token == null)
            {
                throw new UnauthorizedAccessException("You have not create a token.");
            }
            return Token;
        }

        public static Tokens GetToken(string token, string tokenSecret)
        {
            Token = CoreTweet.Tokens.Create(ConsumerKey, ConsumerSecret, token, tokenSecret);
            Token.Account.VerifyCredentials();
            return Token;
        }

        public static bool Authed()
        {
            var account = new SettingUtils().getUserSetting().UserAccounts;
            if (account == null || account.Length == 0)
            {
                return false;
            }
            var defaultAccount = account.Where(a => a.DefaultAccount)
                                  .FirstOrDefault(a => a.Token != null && a.TokenSecret != null);
            return (defaultAccount != null);
        }

        public bool InitialAuth(string pin)
        {
            if (Session == null || String.IsNullOrWhiteSpace(pin))
            {
                return false;
            }
            try
            {
                Token = CoreTweet.OAuth.GetTokens(Session, pin);
            }
            catch (TwitterException)
            {
                return false;
            }

            AddAccount();
            ChangeDefaultAccount();
            return true;
        }

        public void OpenAuthPage()
        {
            Session = Authorization.Authorize();
            var target = Session.AuthorizeUri.ToString();
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = target;
            System.Diagnostics.Process.Start(startInfo);
        }

        private void AddAccount()
        {
            var account = new UserSettingUserAccounts();
            account.AccountId = Token.UserId.ToString();
            account.DefaultAccount = true;
            account.Token = Token.AccessToken;
            account.TokenSecret = Token.AccessTokenSecret;
            if (!new SettingUtils().AddAccount(account))
            {
                MessageBox.Show("ユーザ設定ファイルへの書き込みに失敗しました。");
                System.Windows.Application.Current.Shutdown(1);
            }
        }
        private void ChangeDefaultAccount()
        {
            if (!new SettingUtils().ChangeDefaultAccount(Token.UserId.ToString()))
            {
                MessageBox.Show("ユーザ設定ファイルへの書き込みに失敗しました。");
                System.Windows.Application.Current.Shutdown(1);
            }

        }
    }
}
