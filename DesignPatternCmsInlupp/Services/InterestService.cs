using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Services
{
    public class InterestService
    {
        public static decimal GetRiksbankensBaseRate()
        {
            using (var c = new SweaWebService.SweaWebServicePortTypeClient())
            {
                var r = c.getLatestInterestAndExchangeRates(SweaWebService.LanguageType.sv, new[] { "SEDP2MSTIBOR" });
                return Convert.ToDecimal(r.groups[0].series[0].resultrows[0].value);
            }
        }
    }
}