using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Climie3.config
{
    /// <summary>
    /// キー情報の保持
    /// </summary>
    public sealed class KeyInfo : IDisposable
    {
        /// <summary>
        /// このクラスの唯一のインスタンスを保持するオブジェクト
        /// </summary>
        private static KeyInfo _instance;
                
        /// <summary>
        /// パスワード
        /// </summary>
        public SecureString Key { get; set; }

        /// <summary>
        /// インスタンスへのアクセサ
        /// </summary>
        public static KeyInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KeyInfo();
                }

                return _instance;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        private KeyInfo() 
        {
            Key = new SecureString();
            Key.Clear();
        }

        /// <summary>
        /// キーをセットする
        /// </summary>
        public void SetKey(SecureString key)
        {
            // アンマネージリソース割り当て
            var bstr = Marshal.SecureStringToBSTR(key);

            try
            {
                // 保持しているキー情報を破棄する
                Key.Clear();

                // SecureStringを展開する
                var chars = Enumerable.Range(0, key.Length).Select(i => (char)Marshal.ReadInt16(bstr, i * 2));

                // キー情報を書き込む
                foreach (var c in chars)
                {
                    Key.AppendChar(c);
                }
            }
            finally
            {
                // アンマネージリソース解放
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            if (Key != null)
            {
                Key.Dispose();
            }

            if (_instance != null)
            {
                _instance = null;
            }

            GC.SuppressFinalize(this);
        }

        ///// <summary>
        ///// リソースの解放
        ///// </summary>
        ///// <param name="disposing"></param>
        //private void Dispose(bool disposing)
        //{
        //    if (isDisposed) return;
        //    if (disposing)
        //    {
        //        if (Key != null)
        //        {
        //            Key.Dispose();
        //        }

        //        if (_instance != null)
        //        {
        //            _instance = null;
        //        }
        //    }

        //    isDisposed = true;
        //}
    }
}
