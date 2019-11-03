using System.ComponentModel;
using System.Runtime.CompilerServices;
using Knyaz.Optimus.Dom.Interfaces;

namespace Prime.Model
{
	class DomInspector : INotifyPropertyChanged
	{
		public INode SelectedNode
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
		private INode _selectedNode;
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
