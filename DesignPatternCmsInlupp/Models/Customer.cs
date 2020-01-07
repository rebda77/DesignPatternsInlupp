using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class Customer
    {
        public Customer()
        {
            Loans = new List<Loan>();
        }
        public string PersonNummer { get; set; }
        public List<Loan> Loans { get; set; }

        public int Total()
        {
            return Loans.Sum(l => l.Belopp);
        }

        public bool HasEverBeenLatePaying { get {
                foreach (var loan in Loans)
                    foreach (var i in loan.Invoices)
                        if (i.LatePayment() > 0) return true;
                return false;
            } }

    }
}