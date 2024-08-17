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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyMiniLedger.WPF.Windows.KindWindow
{
	/// <summary>
	/// Interaction logic for KindWindow.xaml
	/// </summary>
	public partial class KindWindow : Window
	{
		MainWindowModel MainWindow;
		public KindWindow(MainWindowModel _mainWindowModel)
		{
			InitializeComponent();
			MainWindow = _mainWindowModel;
			(DataContext as KindWindowModel).UpdateKindEvent += MainWindow.UpdateKinds;
			(DataContext as KindWindowModel).UpdateKindEvent += MainWindow.UpdatePositionsCollection;
		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void KindsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (((KindUIModel)KindsList.SelectedItem) != null)
			{
				//Выбор индекса для комбобокса
				for (int i = 0; i < (DataContext as KindWindowModel).Categories.Count - 1; i++)
				{
					if (((KindUIModel)KindsList.SelectedItem).Category.Category == (DataContext as KindWindowModel).Categories[i].Category)
					{
						cb_CategoriesForChange.SelectedIndex = i;
					}
				}
				tb_EditKind.Text = ((KindUIModel)KindsList.SelectedItem).Kind;

				//Присваиваю id и количество ссылок выбранному виду
				(DataContext as KindWindowModel).SelectedKind.Id = ((KindUIModel)KindsList.SelectedItem).Id;
				(DataContext as KindWindowModel).SelectedKind.Kind = ((KindUIModel)KindsList.SelectedItem).Kind;
				(DataContext as KindWindowModel).SelectedKind.RefNumber = ((KindUIModel)KindsList.SelectedItem).RefNumber;
				//Передача выбранной категории
				(DataContext as KindWindowModel).SelectedKind.Category = ((KindUIModel)KindsList.SelectedItem).Category;
			}
			
		}

		private void cb_CategoriesForChange_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			(DataContext as KindWindowModel).SelectedCategory = (CategoryUIModel)cb_CategoriesForChange.SelectedItem;
		}
	}
}
