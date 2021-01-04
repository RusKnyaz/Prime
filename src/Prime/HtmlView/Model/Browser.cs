using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Scripting.Jurassic;
using Prime.Annotations;
using Prime.Styles;

namespace Prime.Model
{
	/// <summary> Browser model </summary>
	public class Browser : INotifyPropertyChanged
	{
		private readonly IConsole _console;
		public Engine Engine;

		public Browser(IConsole console)
		{
			_console = console;
			Engine = BuildEngine();
			if (console != null)
				Engine.ScriptExecutor.OnException += ex => _console.Error(ex.Message);
		}

		private EngineBuilder ConfigureBuilder()
		{
			var builder = EngineBuilder.New()
				.EnableCss(css => css.UserAgentStyleSheet = StyleSheetFactory.Instance.GetBrowserDefault());
			switch (Properties.Settings.Default.JsEngine)
			{
				case "JINT": builder.UseJint();break;
				case "JURASSIC" : builder.UseJurassic();break;
			}

			builder.Window(w => w.SetConsole(_console));

			return builder;
		}
		

		private Engine BuildEngine() => ConfigureBuilder()
			.ConfigureResourceProvider(r => r.Http().UsePrediction().Notify(OnRequest, OnResponse))
			.Build();

		private void OnResponse(ReceivedEventArguments arg)
		{
			if (_console != null &&
				arg.Resource is HttpResponse http && http.StatusCode == HttpStatusCode.NotFound)
			{
				_console.Log("Error loading: " + http.Uri);
			}
		}

		private void OnRequest(Request obj)
		{
		}

		private Engine BuildEngine(string login, string password) =>
			ConfigureBuilder()
				.ConfigureResourceProvider(r => 
					r.Http(x => x.Basic(login, password)).UsePrediction().Notify(OnRequest, OnResponse))
				.Build();
		
		public Exception Exception { get; private set; }

		public BrowserStates State
		{
			get => _state;
			set
			{
				if (_state == value)
					return;

				_state = value;
				OnPropertyChanged();
			}
		}

		private BrowserStates _state;

		public async Task OpenUrl(string value)
		{
			var page = await OpenUrlInternal(value);
			if (page is HttpPage httpPage && httpPage.HttpStatusCode == HttpStatusCode.Unauthorized)
			{
				// Ask for username and password
				var credentials = OnAuthorize?.Invoke();
				var login = credentials?.Item1;
				var password = credentials?.Item2;
				
				if (!string.IsNullOrEmpty(login))
				{
					//Create new engine with specified authorization headers.
					Engine = BuildEngine(login, password);
					
					await OpenUrl(value);
				}
			}
		}

		private async Task<Page> OpenUrlInternal(string value)
		{
			State = BrowserStates.Loading;
			try
			{
				var page = await Engine.OpenUrl(value);
				State = BrowserStates.Ready;
				return page;
			}
			catch (Exception e)
			{
				Exception = e;
				State = BrowserStates.Error;
				return null;
			}
		}

		public event Func<Tuple<string, string>> OnAuthorize;

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	
	public enum BrowserStates
	{
		/// <summary>
		/// No document available
		/// </summary>
		None,
		/// <summary>
		/// Document is ready
		/// </summary>
		Ready,
		/// <summary>
		/// There are resources to load.
		/// </summary>
		Loading,
		Error
	}
}