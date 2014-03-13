using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Climie3.model
{
    /// <summary>
    /// 
    /// </summary>
    public class DataCipherModel
    {
        /// <summary>
        /// 暗号化処理メソッド
        /// </summary>
        /// <param name="sPassInf">秘密鍵の生成に使う情報</param>
        /// <param name="iMode">暗号化モード</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] buf, SecureString pass)
        {
            byte[] encData = null;
            byte[] outBt = null;
            byte[] tmpBt = null;
            int readLen = 0;

            try
            {
                if (pass == null)
                {
                    return null;
                }

                using (KeyManagerModel keyMan = new KeyManagerModel())
                {
                    keyMan.GenerateKey(pass);

                    // 暗号化プロバイダの生成
                    RijndaelManaged rijnProvider = new RijndaelManaged();
                    rijnProvider.BlockSize = 128;                       // 暗号化ブロックサイズ
                    rijnProvider.KeySize = 256;                         // 鍵長
                    rijnProvider.Key = keyMan.SecKey;                   // 鍵
                    rijnProvider.IV = keyMan.InitVec;                   // 初期化ベクトル
                    rijnProvider.Padding = PaddingMode.ANSIX923;        // パディング

                    // 暗号化開始
                    using (ICryptoTransform iEncryptor = rijnProvider.CreateEncryptor())
                    {
                        using (MemoryStream inMs = new MemoryStream(buf))
                        {
                            using (MemoryStream outMs = new MemoryStream())
                            {
                                using (CryptoStream cryptStrm = new CryptoStream(outMs, iEncryptor, CryptoStreamMode.Write))
                                {
                                    tmpBt = new byte[1024];  // 一時バッファ

                                    while ((readLen = inMs.Read(tmpBt, 0, tmpBt.Length)) > 0)
                                    {
                                        cryptStrm.Write(tmpBt, 0, readLen);
                                    }
                                }

                                outBt = outMs.ToArray();
                            }
                        }
                    }

                    // 暗号化データとSalt値を連結して返す
                    encData = keyMan.Salt.Concat(outBt).ToArray();
                }
            }
            catch
            {
                return null;
            }

            return encData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="decTgt"></param>
        /// <returns></returns>
        public byte[] Decrypt(SecureString pass, byte[] decTgt)
        {
            byte[] decData = null;
            byte[] tmpBt = null;
            int readLen = 0;

            try
            {
                if (pass == null)
                {
                    return null;
                }

                using (KeyManagerModel keyMan = new KeyManagerModel(decTgt.Take(16).ToArray()))
                {
                    keyMan.GenerateKey(pass);

                    // 暗号化プロバイダの生成
                    RijndaelManaged rijnProvider = new RijndaelManaged();
                    rijnProvider.BlockSize = 128;                       // 暗号化ブロックサイズ
                    rijnProvider.KeySize = 256;                         // 鍵長
                    rijnProvider.Key = keyMan.SecKey;                   // 鍵
                    rijnProvider.IV = keyMan.InitVec;                   // 初期化ベクトル
                    rijnProvider.Padding = PaddingMode.ANSIX923;        // パディング

                    using (ICryptoTransform iDecryptor = rijnProvider.CreateDecryptor())
                    {
                        using (MemoryStream outMs = new MemoryStream())
                        {
                            using (MemoryStream inMs = new MemoryStream(decTgt.Skip(keyMan.Salt.Length).ToArray()))
                            {
                                using (CryptoStream cryptStrm = new CryptoStream(inMs, iDecryptor, CryptoStreamMode.Read))
                                {
                                    tmpBt = new byte[1024];

                                    while ((readLen = cryptStrm.Read(tmpBt, 0, tmpBt.Length)) > 0)
                                    {
                                        outMs.Write(tmpBt, 0, readLen);
                                    }
                                }
                            }

                            // 復号したデータを書込
                            decData = outMs.ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }

            return decData;
        }
    }
}
