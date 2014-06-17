#if(!__XAMARIN)
using Microsoft.WindowsAzure.Mobile.Service;
#endif
using System;
using System.Collections.Generic;
using System.Linq;

namespace POPpicService.DataObjects
{
    public class POPpicGameResults
	#if(!__XAMARIN)
	: EntityData
	#endif
    {
        public String GameId { get; set; }
        public String WinnerId { get; set; }
        public String LoserId { get; set; }
        public DateTimeOffset WinnerDuration { get; set; }
        public DateTimeOffset LoserDuration { get; set; }
        public Int32 WinnerNumberOfMoves { get; set; }
        public Int32 LoserNumberOfMoves { get; set; }
        public String ImageUrl { get; set; }
    }
}