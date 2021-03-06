﻿using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Knyaz.Optimus;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Prime.Annotations;
using Prime.HtmlView;

namespace Prime.Model
{
	public class Browser : INotifyPropertyChanged
	{
		public Engine Engine ;

		public Browser()
		{
			Engine = BuildEngine();
			Engine.ComputedStylesEnabled = true;
		}

		private static Engine BuildEngine() => EngineBuilder.New().UseJint().Build();

		private static Engine BuildEngine(string login, string password) =>
			EngineBuilder.New()
				.UseJint()
				.SetResourceProvider(new ResourceProviderBuilder().Http(x => x.Basic(login, password)).UsePrediction().Build())
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
					Engine.ComputedStylesEnabled = true;
					
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
}