using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Climie3.model
{
    /// <summary>
    /// キー情報生成・保持
    /// </summary>
    public class KeyManagerModel : IDisposable
    {
        /// <summary>
        /// salt
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// 初期化ベクトル
        /// </summary>
        public byte[] InitVec { get; set; }

        /// <summary>
        /// 秘密鍵
        /// </summary>
        public byte[] SecKey { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KeyManagerModel() { }

        /// <summary>
        /// コンストラクタ（復号用）
        /// </summary>
        /// <param name="salt"></param>
        public KeyManagerModel(byte[] salt)
        {
            // saltの値をセット
            Salt = salt;
        }

        /// <summary>
        /// 秘密鍵を生成する（パスワード）
        /// </summary>
        /// <returns></returns>
        public void GenerateKey(SecureString pass)
        {
            // アンマネージリソース割り当て
            var bstr = Marshal.SecureStringToBSTR(pass);

            try
            {
                // SecureStringを展開する
                var key = Enumerable.Range(0, pass.Length).Select(i => Marshal.ReadByte(bstr, i));

                // Saltがない場合は生成
                if (Salt == null || Salt.Length == 0)
                {
                    Salt = new byte[16];

                    // RNGCryptoServiceProviderを利用してSaltを生成する
                    RNGCryptoServiceProvider rngProv = new RNGCryptoServiceProvider();
                    rngProv.GetBytes(Salt);
                }

                // 秘密鍵と初期化ベクトルを生成する
                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(key.ToArray(), Salt, 1024);
                SecKey = deriveBytes.GetBytes(256 / 8);
                InitVec = deriveBytes.GetBytes(128 / 8);
            }
            catch
            {
                // 例外発生時は初期化
                Salt = null;
                InitVec = null;
                SecKey = null;
            }
            finally
            {
                // アンマネージリソースの解放
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        /// <summary>
        /// パスワードのハッシュ化
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public string GetHashString(SecureString pass)
        {
            // 変数初期化
            byte[] hash = null;

            // アンマネージリソース割り当て
            var bstr = Marshal.SecureStringToBSTR(pass);
            
            try
            {
                // SecureStringの展開  
                var key = Enumerable.Range(0, pass.Length).Select(i => Marshal.ReadByte(bstr, i));

                // ハッシュ値を計算
                SHA256 sha256 = SHA256.Create();
                hash = sha256.ComputeHash(key.ToArray());
            }
            catch
            {
                return null;
            }
            finally
            {
                // アンマネージリソースの解放
                Marshal.ZeroFreeBSTR(bstr);
            }

            // 文字列に変換して返す
            return Encoding.UTF8.GetString(hash);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Salt = null;
            InitVec = null;
            SecKey = null;

            GC.SuppressFinalize(this);
        }
    }
}
