using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Knyaz.Optimus;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.WinForms;
using Knyaz.Optimus.WinForms.Annotations;

namespace Prime.Model
{
	public class Browser : INotifyPropertyChanged
	{
		public Engine Engine = new Engine() { ComputedStylesEnabled = true};
		private Exception _exception;

		public BrowserStates State
		{
			get { return _state; }
			set
			{
				if (_state == value)
					return;

				_state = value;
				OnPropertyChanged();
			}
		}

		string Login;
		string Password;
		private BrowserStates _state;

		public async Task OpenUrl(string value)
		{
			_exception = null;

			try
			{
				State = BrowserStates.Loading;
				var page = await Engine.OpenUrl(value);
				State = BrowserStates.Ready;
				if (page is HttpPage httpPage && httpPage.HttpStatusCode == HttpStatusCode.Unauthorized)
				{
					Authorize();
					if (!string.IsNullOrEmpty(Login))
					{
						Engine = new Engine(new ResourceProviderBuilder().Http(x => x.Basic(Login, Password)).UsePrediction().Build())  { ComputedStylesEnabled = true};
						State = BrowserStates.Loading;
						await OpenUrl(value);
						State = BrowserStates.Ready;
					}
					//pass login|password to the engine.
				}
			}
			catch (Exception e)
			{
				_exception = e;
				State = BrowserStates.Error;
			}
		}
		
		public event Func<Tuple<string, string>> OnAuthorize;

		private void Authorize()
		{
			var result = OnAuthorize?.Invoke();
			Login = result?.Item1;
			Password = result?.Item2;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}