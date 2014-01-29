using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Climie3.model
{
    public class RegexManagerModel : IDisposable
    {
        // Migemo正規表現クラス
        private Migemo migemo;

        //// インスタンス保持変数
        //private static RegexManagerModel regexManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RegexManagerModel()
        {
            // Migemo使用時はMigemoオブジェクト生成
            if (File.Exists("migemo.dll") &&
                File.Exists("dict/migemo-dict"))
            {
                migemo = new Migemo("dict/migemo-dict");
            }
        }

        ///// <summary>
        ///// インスタンス取得用メソッド
        ///// </summary>
        ///// <returns></returns>
        //public static RegexManagerModel GetInstance()
        //{
        //    if (regexManager == null)
        //    {
        //        regexManager = new RegexManagerModel();
        //    }

        //    return regexManager;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Regex GetRegex(string pattern)
        {
            if (migemo != null && 
                pattern.StartsWith("/") && 
                !pattern.EndsWith("/"))
            {
                return migemo.GetRegex(pattern);
            }
            else
            {
                return new Regex(pattern);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (migemo != null)
            {
                migemo.Dispose();
            }
        }
    }
}
