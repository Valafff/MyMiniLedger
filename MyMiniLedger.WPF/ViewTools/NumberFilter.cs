using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyMiniLedger.WPF.ViewTools
{
	internal class NumberFilter
	{
		public void textBoxPreviewTextInputFilter(object sender, TextCompositionEventArgs e)
		{
			string str = ((TextBox)sender).Text + e.Text;

			if ((str.IndexOf('0') == 0 && str.IndexOf(',') != 1 && Char.IsDigit(e.Text, 0) && str != "0")
				|| (((TextBox)sender).SelectionStart == 0 && e.Text == "0" && ((TextBox)sender).Text.Length != 0)
				|| (((TextBox)sender).SelectionStart == 0 && !Char.IsDigit(e.Text, 0)))
			{
				if (((TextBox)sender).Text == "0" && e.Text != "0" && Char.IsDigit(e.Text, 0))
				{
					((TextBox)sender).Text = e.Text;
					((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
				}
				e.Handled = true;
				return;
			}
			e.Handled = !double.TryParse(str, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.CurrentUICulture, out double i);
		}
		public void textBoxTextChangedFilter(object sender, TextChangedEventArgs e)
		{
			if (((TextBox)sender).Text == string.Empty)
			{
				((TextBox)sender).Text = "0";
				((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
			}
		}
	}
}
