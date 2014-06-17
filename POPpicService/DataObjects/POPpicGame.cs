#if(!__XAMARIN)
using Microsoft.WindowsAzure.Mobile.Service;
#endif
using System;
using System.Collections.Generic;
using System.Linq;

namespace POPpicService.DataObjects
{
	public class POPpicGame
	#if(!__XAMARIN)
		: EntityData
	#endif
	{
		public String RequesterId { get; set; }
		public String ResponderId { get; set; }
		public String BackgroundId { get; set; }
		public String BalloonId { get; set; }
		public DateTimeOffset PopTime { get; set; }
	}

	public class POPpicGameModel : POPpicGame
	{
		public POPpicGameModel()
		{
			this.GameMoves = new List<POPpicGameMove>();
		}

		public POPpicGameModel(POPpicGame game)
		{
			this.GameMoves = new List<POPpicGameMove>();

			CrossPlatformUtilities.CopyProperties(game, this);
		}

		public IList<POPpicGameMove> GameMoves { get; set; }
		public POPpicGameResults GameResult { get; set; }
	}
}