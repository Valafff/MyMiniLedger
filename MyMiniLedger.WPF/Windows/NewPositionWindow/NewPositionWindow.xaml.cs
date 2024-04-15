using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.WindowsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyMiniLedger.WPF.Windows.NewPositionWindow
{
    /// <summary>
    /// Interaction logic for NewPositionWindow.xaml
    /// </summary>
    public partial class NewPositionWindow : Window
    {
        public NewPositionWindow()
        {
            InitializeComponent();

            dp_OpenDate.SelectedDate = DateTime.Now;



		}

		

		private void ComboBox_Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).PositionConstruct.Kind.Category.Category = ((CategoryUIModel)cb_Category.SelectedItem).Category;
        }

		private void ComboBox_Kind_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).PositionConstruct.Kind.Kind = ((KindUIModel)cb_Kind.SelectedItem).Kind;
		}

		private void TextBox_Income_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			//Замена точки на запятую для ввода с разных раскладок
			if (e.Key == Key.OemPeriod)
			{
				e.Source = Key.OemComma;
			}

			//if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.') && (textBox_Income.Text.IndexOf(".") == -1) && (textBox_Income.Text.Length != 0)))
			//{
			//	if (e.KeyChar != (char)Keys.Back)
			//	{
			//		e.Handled = true;
			//	}
			//}

			if (!(Char.IsDigit((char)e.Key)) && !((e.Key == (Key)',') && (tb_Income.Text.IndexOf(",") == -1) && (tb_Income.Text.Length != 0)))
			{
				if ((char)e.Key != (char)Keys.Back)
				{
					e.Handled = true;
				}
			}


			////Замена точки на запятую для ввода с разных раскладок
			//KeyboardChar = e.KeyChar;
			//if (e.KeyChar == '.')
			//{
			//	e.KeyChar = ',';
			//}

			////if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.') && (textBox_Income.Text.IndexOf(".") == -1) && (textBox_Income.Text.Length != 0)))
			////{
			////	if (e.KeyChar != (char)Keys.Back)
			////	{
			////		e.Handled = true;
			////	}
			////}

			//if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == ',') && (textBox_Income.Text.IndexOf(",") == -1) && (textBox_Income.Text.Length != 0)))
			//{
			//	if (e.KeyChar != (char)Keys.Back)
			//	{
			//		e.Handled = true;
			//	}
			//}

		}

		private void tb_Income_SizeChanged(object sender, SizeChangedEventArgs e)
		{

		}
	}
}
