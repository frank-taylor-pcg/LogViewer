using Akka.Actor;
using LogViewer.Actors;

namespace LogViewer
{
	public static class Globals
	{
		public static ActorSystem ActorSystem { get; set; }
		public static IActorRef FileHandler { get; set; }

		public static void InitializeActorSystem()
		{
			ActorSystem = ActorSystem.Create(nameof(LogViewer));

			FileHandler = ActorSystem.ActorOf<FileHandlerActor>();
		}
	}
}
