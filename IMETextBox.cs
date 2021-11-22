using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hash_textarea
{
    public class IMETextBox : TextBox
    {
        static IMETextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IMETextBox), new FrameworkPropertyMetadata(typeof(IMETextBox)));
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            GetBindingExpression(TextBox.TextProperty).UpdateSource();
            base.OnTextChanged(e);
        }

    }
}
