using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class Payment
    {
        public DateTime PaymentDate { get; set; }

        public int Belopp { get; set; }

        public string BankPaymentReference { get; set; }
    }
}