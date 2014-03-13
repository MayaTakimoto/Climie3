using Climie3.config;
using Climie3.model;
using Livet;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Climie3.viewmodel
{
    /// <summary>
    /// 
    /// </summary>
    public class PasswordViewModel : ViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SecureString ConfirmPassword { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public void UpdateKey()
        {
            IsInputOK();

            KeyInfo.Instance.SetKey(Password);

            using (StreamWriter sw = new StreamWriter("key"))
            {
                using (KeyManagerModel keyMan = new KeyManagerModel())
                {
                    string hash = keyMan.GetHashString(Password);
                    sw.Write(hash);
                }
            }
        }

        private bool IsInputOK()
        {
            if (Password == null)
            {
                return false;
            }

            if (ConfirmPassword == null)
            {
                return false;
            }

            var ptBef = Marshal.SecureStringToBSTR(Password);
            var ptAft = Marshal.SecureStringToBSTR(ConfirmPassword);

            try
            {
                return Enumerable.Range(0, Password.Length).All(i => Marshal.ReadInt16(ptBef, i) == Marshal.ReadInt16(ptAft, i));
            }
            catch
            {
                return false;
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptBef);
                Marshal.ZeroFreeBSTR(ptAft);
            }
        }
    }
}
