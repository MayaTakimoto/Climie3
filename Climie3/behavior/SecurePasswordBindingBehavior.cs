using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Climie3.behavior
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurePasswordBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// SecurePassword依存関係プロパティ
        /// </summary>
        public SecureString SecurePassword
        {
            get
            {
                return (SecureString)GetValue(SecurePasswordProperty);
            }
            set
            {
                SetValue(SecurePasswordProperty, value);
            }
        }

        /// <summary>
        /// SecurePassword依存関係プロパティ識別子
        /// </summary>
        public static readonly DependencyProperty SecurePasswordProperty =
            DependencyProperty.Register("SecurePassword",
            typeof(SecureString),
            typeof(SecurePasswordBindingBehavior),
            new PropertyMetadata(new SecureString(), SecurePasswordPropertyChanged));

        /// <summary>
        /// SecurePasswordの値が変更された際の処理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void SecurePasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as SecurePasswordBindingBehavior;
            if (d == null) return;

            var passwordBox = behavior.AssociatedObject as PasswordBox;
            if (passwordBox == null) return;

            var newPassword = e.NewValue as SecureString;
            if (newPassword == null) return;

            var oldPassword = e.OldValue as SecureString;
            if (newPassword.Equals(oldPassword)) return;

            var bstr = Marshal.SecureStringToBSTR(newPassword);
            try
            {
                passwordBox.SecurePassword.Clear();
                var chars = Enumerable.Range(0, newPassword.Length).Select(i => (char)Marshal.ReadInt16(bstr, i * 2));
                foreach (var c in chars)
                {
                    passwordBox.SecurePassword.AppendChar(c);
                }
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += PasswordBox_PasswordChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= PasswordBox_PasswordChanged;
            base.OnDetaching();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SecurePassword = AssociatedObject.SecurePassword.Copy();
        }
    }
}
