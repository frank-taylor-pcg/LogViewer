using Akka.Actor;
using LogViewer.Actors;
using LogViewer.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace LogViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private IActorRef _actor;
		private MainViewModel _viewModel;

		public static RoutedCommand ExitCommand = new();

		public MainWindow()
		{
			InitializeComponent();

			_viewModel = new MainViewModel();

			Globals.InitializeActorSystem();
			_actor = Globals.ActorSystem.ActorOf(Props.Create(() => new MainViewActor(_viewModel)));

			DataContext = _viewModel;
		}

		private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog dialog = new();
			if (dialog.ShowDialog() == true)
			{
				System.Diagnostics.Debug.WriteLine("Opening file dialog");
				_actor.Tell(new OpenFile(dialog.FileName));
			}
		}

		private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
