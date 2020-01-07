using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class Invoice
    {
        public Invoice()
        {
            Payments = new List<Payment>();
            InvoiceNo = Convert.ToInt32( DateTime.Now.ToString("yyyyMMdd") + Convert.ToInt32(DateTime.Now.Ticks) );
        }
        public DateTime InvoiceDate { get; set; }

        public int InvoiceNo { get; set; }

        public DateTime DueDate { get; set; }

        public int Belopp { get; set; }

        public List<Payment> Payments { get; set; }

        public int LatePayment()
        {
            return Payments.Where(r => r.PaymentDate.Date > DueDate.Date).Sum(r => r.Belopp);

        }
    }
}