#if(!__XAMARIN)
using Microsoft.WindowsAzure.Mobile.Service;
#endif
using System;
using System.Collections.Generic;
using System.Linq;

namespace POPpicService.DataObjects
{
    public class POPpicUser
	#if(!__XAMARIN)
	: EntityData
	#endif
    {
        public String FacebookId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String UserName { get; set; }
        public String ProfilePictureUrl { get; set; }
        public DateTimeOffset LastSignInTimeStamp { get; set; }
        public Double LastLatitude { get; set; }
        public Double LastLongitude { get; set; }
        public DateTimeOffset LastLocationTimeStamp { get; set; }
        public Int32 Wins { get; set; }
        public Int32 Loses { get; set; }
        public DateTimeOffset TimePressed { get; set; }
        public DateTimeOffset OpponentTimePressed { get; set; }
        public String ExtraProperty { get; set; }
    }
}