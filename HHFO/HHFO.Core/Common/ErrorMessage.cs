using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace HHFO.Core
{
    /// <summary>
    /// CommonSettingを保持する静的クラス
    /// アプリケーションからは修正を走らせないため、本クラス内で保持した値を利用する。
    /// </summary>
    public static class ErrorMessage
    {
        public static HHFO.Config.ErrorMessage value { get; }

        static ErrorMessage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = CommonSetting.Value.ErrorMessagePath;

            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                var fileUtils = new FileUtils();
                value = fileUtils.ReadXml<HHFO.Config.ErrorMessage>(stream, typeof(HHFO.Config.ErrorMessage));
            }
        }
    }
}
