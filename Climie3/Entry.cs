using Climie3.config;
using System;
using System.Security;
using System.Windows;

namespace Climie3
{
    /// <summary>
    /// アプリケーションエントリポイント
    /// </summary>
    public class Entry
    {
        [STAThread()]
        public static void Main()
        {  
#if DEBUG
            SecureString s = new SecureString();
            char[] pass = "test".ToCharArray();

            foreach (char c in pass)
            {
                s.AppendChar(c);
            }

            KeyInfo.Instance.SetKey(s);
#endif
            Application app = new Application();
            MainWindow mw = new MainWindow();

            app.Run(mw);
        }
    }
}
