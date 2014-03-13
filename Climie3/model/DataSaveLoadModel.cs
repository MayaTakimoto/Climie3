using Climie3.config;
using ProtoBuf;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Climie3.model
{
    /// <summary>
    /// ファイル読書クラス
    /// </summary>
    public class DataSaveLoadModel
    {
        /// <summary>
        /// ファイル書込処理
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="oc">コレクション</param>
        public void Save<T>(ObservableCollection<T> oc, string filePath)
        {
            byte[] data = null;
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true);

            if (oc == null || oc.Count == 0)
            {
                return;
            }

            if (!File.Exists(filePath))
            {
                return;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, oc);

                if (KeyInfo.Instance.Key == null || KeyInfo.Instance.Key.Length == 0)
                {
                    // パスワード未設定時はそのまま
                    data = ms.ToArray();
                }
                else
                {
                    ms.Position = 0;

                    // パスワード設定時は暗号化処理を行う
                    DataCipherModel cipher = new DataCipherModel();
                    data = cipher.Encrypt(ms.ToArray(), KeyInfo.Instance.Key);
                }
            }

            fs.BeginWrite(data, 0, data.Length, new AsyncCallback(SaveCallback), fs);
        }

        /// <summary>
        /// ファイル読込処理
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <returns>コレクション</returns>
        public ObservableCollection<T> Load<T>(string filePath)
        {
            ObservableCollection<T> oc = null;
            byte[] data = null;
            byte[] dec = null;

            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (KeyInfo.Instance.Key == null || KeyInfo.Instance.Key.Length == 0)
                    {
                        // パスワード未設定時はそのまま
                        oc = Serializer.Deserialize<ObservableCollection<T>>(fs);
                    }
                    else
                    {
                        // パスワード設定時は復号処理を行う
                        data = new byte[fs.Length];
                        fs.Read(data, 0, data.Length);

                        DataCipherModel cipher = new DataCipherModel();
                        dec = cipher.Decrypt(KeyInfo.Instance.Key, data);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            stream.Write(dec, 0, dec.Length);
                            oc = Serializer.Deserialize<ObservableCollection<T>>(stream);
                        }
                    }
                }
            }
            else
            {
                oc = new ObservableCollection<T>();
            }

            return oc;
        }

        /// <summary>
        /// 非同期書込処理のコールバックメソッド
        /// </summary>
        /// <param name="ar"></param>
        private static void SaveCallback(IAsyncResult ar)
        {
            // 当該ストリームの取得
            FileStream fs = ar.AsyncState as FileStream;

            fs.EndWrite(ar);
            fs.Close();
        }
    }
}
