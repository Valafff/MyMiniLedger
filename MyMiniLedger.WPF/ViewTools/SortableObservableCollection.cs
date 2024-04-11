using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.ViewTools
{
	public class SortableObservableCollection<T>: ObservableCollection<T>
	{

		public void Sort(Comparison<T> comparison)
		{
			// Преобразуем Items в List<T> для использования метода Sort.
			var items = this.Items.ToList();
			items.Sort(comparison);

			// Обновляем коллекцию после сортировки.
			this.Items.Clear();
			foreach (var item in items)
			{
				this.Items.Add(item);
			}

			// Уведомляем об изменениях.
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

	}
}
