using HHFO.Config;
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
    public static class CommonSetting
    {
        public static HHFO.Config.CommonSetting Value { get; }

        static CommonSetting()
        {
            var fileUtils = new FileUtils();
            var assembly = Assembly.GetExecutingAssembly();
            var resource = "HHFO.Core.CommonSetting.xml";
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                Value = fileUtils.ReadXml<HHFO.Config.CommonSetting>(stream, typeof(HHFO.Config.CommonSetting));
            }
        }
    }
}
