using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class CallInstructions
    {
        public int HowMuchDoYouNeed { get; set; }

        public string Personnummer { get; set; }

        public bool Result { get; set; }
        public decimal RateWeCanOffer { get; set; }
        public Customer Customer { get; internal set; }
    }
}