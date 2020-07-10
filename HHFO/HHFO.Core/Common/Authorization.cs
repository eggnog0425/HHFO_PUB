using System;
using System.Collections.Generic;
using System.Text;
using CoreTweet;
using HHFO.Core.Common;

namespace HHFO.Models
{
    public static class Authorization
    {
        private static Tokens Token = null;
        private static readonly string ConsumerKey;
        private static readonly string ConsumerSecret;

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
                throw new UnauthorizedAccessException("You have'n create token.");
            }
            return Token;
        }

        public static Tokens GetToken(OAuth.OAuthSession session, string pin)
        {
            Token = CoreTweet.OAuth.GetTokens(session, pin);
            return Token;
        }

        public static Tokens GetToken(string tokenKey, string tokenSecret)
        {
            Token = CoreTweet.Tokens.Create(ConsumerKey, ConsumerSecret, tokenKey, tokenSecret);
            return Token;
        }

    }
}
