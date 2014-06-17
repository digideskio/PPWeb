#if(!__XAMARIN)
using Microsoft.WindowsAzure.Mobile.Service;
#endif
using System;
using System.Collections.Generic;
using System.Linq;

namespace POPpicService.DataObjects
{
    public class POPpicGameMove
	#if(!__XAMARIN)
	: EntityData
	#endif
    {
        public String GameId { get; set; }
        public String UserId { get; set; }
        public DateTimeOffset MoveDuration { get; set; }
        public Int32 NumberOfTaps { get; set; }
    }
}