using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.FinansInspektionsRapportering
{
    public class Report
    {
        public enum ReportType
        {
            Loan,
            LatePayment,
            AverageRta
        }


        public ReportType Type { get; set; }
        public decimal LoanBelopp { get; }
        public string PersonNummer { get; set; }
        public string LoanNumber { get; set; }
        public decimal AvgRta { get; set; }
        public decimal LatePaymentBelopp { get; set; }

        public Report(ReportType reportType, string personNummer, string loanNumber, decimal avgRta, decimal loanBelopp, decimal latePaymentBelopp)
        {
            Type = reportType;
            LoanBelopp = loanBelopp;
            if (Type == ReportType.LatePayment || Type == ReportType.Loan)
            {
                PersonNummer = personNummer;
                LoanNumber = loanNumber;
            }
            if(Type == ReportType.LatePayment)
            {
                LatePaymentBelopp = latePaymentBelopp;
            }
            if (Type == ReportType.Loan)
            {
                LoanBelopp = loanBelopp;
            }
            if (Type == ReportType.AverageRta)
                AvgRta = avgRta;
        }
        public void Send()
        {
            string transaction = "";
            if(Type == ReportType.AverageRta)
            {
                transaction = $"AR;{AvgRta}";
            }
            if (Type == ReportType.LatePayment)
            {
                transaction = $"AR;{PersonNummer};{LoanNumber};{LatePaymentBelopp}";
            }
            if (Type == ReportType.Loan)
            {
                transaction = $"AR;{PersonNummer};{LoanNumber};{LoanBelopp}";
            }
            //Send transaction
            //Dummy --- nothing happens here...
        }
    }
}