using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class Loan
    {
        public Loan()
        {
            Invoices = new List<Invoice>();
        }
        public string LoanNo { get; set; }
        public int Belopp { get; set; }
        public DateTime FromWhen { get; set; }
        public decimal InterestRate { get; set; }

        public List<Invoice> Invoices { get; set; }
    }
}