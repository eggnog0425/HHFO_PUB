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

namespace HHFO.Models
{
    public class Authorization
    {
        private static Tokens Token = null;
        private static readonly string ConsumerKey;
        private static readonly string ConsumerSecret;
        private OAuth.OAuthSession Session { get; set; }

        static Authorization()
        {
            var setting = new SettingUtils().getCommonSetting();
            ConsumerKey = setting.ConsumerKey;
            ConsumerSecret = setting.ConsumerSecret;
        }

        public static OAuth.OAuthSession Authorize()
        {
            return CoreTweet.OAuth.Authorize(ConsumerKey, ConsumerSecret);
        }

        public static Tokens GetToken()
        {
            if(Token == null)
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
            // ファイルの作成に失敗した場合は3回リトライ。リトライしてもダメな場合メッセージを表示してアプリ終了
            // TODO 設定ファイルへのユーザーアカウントの書き込み処理実装
            MessageBox.Show("設定ファイルの書き込みに失敗しました。");
        }
        private void UpdateAccount()
        {

        }
    }
}
