using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Climie3.model
{
    /// <summary>
    /// 検索用正規表現パターン生成
    /// </summary>
    public class RegexManagerModel : IDisposable
    {
        // Migemo正規表現クラス
        private Migemo migemo;

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

        /// <summary>
        /// 入力値に応じた正規表現パターンを生成する
        /// </summary>
        /// <param name="pattern">入力値</param>
        /// <returns>正規表現パターン</returns>
        public Regex GetRegex(string pattern)
        {
            try
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Migemoオブジェクトのリリース
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
