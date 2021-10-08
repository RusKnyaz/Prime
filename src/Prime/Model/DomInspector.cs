using System.ComponentModel;
using System.Runtime.CompilerServices;
using Knyaz.Optimus.Dom.Elements;

namespace Prime.Model
{
	class DomInspector : INotifyPropertyChanged
	{
		public Node SelectedNode
		{
			get { return _selectedNode; }
			set
			{
				if (Equals(value, _selectedNode)) return;
				_selectedNode = value;
				OnPropertyChanged();
			}
		}

		public bool SelectorActive;
		private Node _selectedNode;
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
