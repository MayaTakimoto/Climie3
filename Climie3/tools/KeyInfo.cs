using System;
using System.Security;

namespace Climie3.tools
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
        /// 状態保持フラグ
        /// </summary>
        private bool isDisposed;

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
            isDisposed = false;
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (disposing)
            {
                if (Key != null)
                {
                    Key.Dispose();
                }

                if (_instance != null)
                {
                    _instance = null;
                }
            }

            isDisposed = true;
        }
    }
}
