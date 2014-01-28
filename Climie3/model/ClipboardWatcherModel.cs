//-----------------------------------------------------------------------
// <summary>クリップボード監視クラス</summary>
// <author>MayaTakimoto</author>
// <date>$Date: 2013-11-01 14:00:00 +9:00 $</date>
// <copyright file="$Name: ClipboardWatcherModel.cs $" >
// Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------
using Livet;
using System.Threading;
using System.Windows;

namespace Climie3.model
{
    /// <summary>
    /// クリップボード監視クラス
    /// </summary>
    public class ClipboradWatcherModel : ViewModel
    {
        // テキストデータ
        private string cbText;

        public string CbText
        {
            get
            {
                return this.cbText;
            }
            set
            {
                if (!value.Equals(this.cbText))
                {
                    this.cbText = value;
                    RaisePropertyChanged("CbText");
                }
            }
        }

        // 監視停止フラグ
        public bool StopFlg { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClipboradWatcherModel()
        {
            Clipboard.Clear();
            CbText = string.Empty;
        }

        /// <summary>
        /// クリップボードのテキストデータを取得する
        /// </summary>
        public void WatchClipboard()
        {
            while (StopFlg == false)
            {
                CbText = Clipboard.GetText();
                Thread.Sleep(1000);
            }
        }
    }
}
