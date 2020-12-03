using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DesignPatternCmsInlupp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Parametrar()
        {
            var logger = Logger.GetInstance();
            
            logger.LogAction(Logger.Actions.ParametrarPage, "");

            var model = new Parametrar();
            model.CurrentRiksbankenStibor = InterestService.GetRiksbankensBaseRate();
            return View(model);
        }


        [HttpGet]
        public ActionResult ListCustomers()
        {
            var model = new List<Customer>();
            var logger = Logger.GetInstance();
            logger.LogAction(Logger.Actions.ListCustomersPage, "");

            ICustomerRepository repository = GetRepository();
            model = repository.GetCustomers();
           

            string databas = Server.MapPath("~/customers.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                var customer = new Customer { PersonNummer = parts[0] };
                SetLoansForCustomer(customer);
                SetInvoicesForCustomer(customer);
                SetPaymentsForCustomer(customer);
                model.Add(customer);
            }


            return View(model);
        }

        private ICustomerRepository GetRepository() //ny metod Repository
        {
            return new FileCustomerRepository();
        }

        [HttpGet]
        public ActionResult Customer(string PersonNummer)
        {
            var logger = Logger.GetInstance();
            logger.LogAction(Logger.Actions.ViewCustomerPage, PersonNummer);

            var customer = FindCustomer(PersonNummer);
            return View(customer);
        }



        [HttpGet]
        public ActionResult Ringinstruktioner()
        {
            var logger = Logger.GetInstance();
            logger.LogAction(Logger.Actions.CallReceived, " some more useless info...");
            var model = new CallInstructions();
            return View(model);
        }

        void SaveToFile(Customer c)
        {
            var repository = GetRepository();
            repository.SaveCustomer(c);
            
        }

        void SaveLoanToFile(Customer c, Loan l)
        {
            var repository = GetRepository();
            repository.SaveLoan(c, l);

            
        }



        [HttpPost]
        public ActionResult NewLoan(CallInstructions model)
        {
            var logger = Logger.GetInstance();

            var c = FindCustomer(model.Personnummer);
            if (c == null)
            {
                c = new Customer { PersonNummer = model.Personnummer };
                SaveToFile(c);
                logger.LogAction(Logger.Actions.CreatingCustomer, model.Personnummer);
                SendEmailToBoss("New customer!",model.Personnummer);
            }

            var loan = new Loan
            { 
                LoanNo = DateTime.Now.Ticks.ToString(),
                Belopp = model.HowMuchDoYouNeed,
                FromWhen = DateTime.Now,
                InterestRate = model.RateWeCanOffer
            };

            c.Loans.Add(loan);
            SaveLoanToFile(c,   loan);
            SendEmailToBoss("New loan!", model.Personnummer + " " + loan.LoanNo);
            ReportNewLoanToFinansInspektionen(model.Personnummer, loan);

            logger.LogAction(Logger.Actions.CreatingLoan, $"{model.Personnummer} {loan.LoanNo}  {loan.Belopp}");


            return View(loan);
        }

        void SendEmailToBoss(string subject, string message)
        {
            var mailer = new Mailer();
            mailer.SendMail("harry@hederligeharry.se", subject, message);
        }

        void ReportNewLoanToFinansInspektionen(string personNummer, Loan loan)
        {
            var report = new FinansInspektionsRapportering.Report(FinansInspektionsRapportering.Report.ReportType.Loan,
                personNummer, loan.LoanNo, 0, loan.Belopp, 0);
            report.Send();
        }

        [HttpPost]
        public ActionResult Ringinstruktioner(CallInstructions model)
        {
            var c = FindCustomer(model.Personnummer);
            model.Result = true;
            if (c == null)
                model.Customer = c;

            int age = GetAge(model.Personnummer);
            decimal baseRate = InterestService.GetRiksbankensBaseRate();
            

            if (c == null)
            {
                if (age < 18)
                    model.RateWeCanOffer = 30.22m + baseRate;
                else if (age < 35)
                    model.RateWeCanOffer = 32.18m + baseRate;
                else if (age < 65)
                    model.RateWeCanOffer = 22.30m + baseRate;
                else 
                    model.RateWeCanOffer = 45.30m + baseRate;
            }
            else
            {
                if (age < 18)
                    model.RateWeCanOffer = 29.32m + baseRate;
                else if (age < 35)
                    model.RateWeCanOffer = 31.38m + baseRate;
                else if (age < 65)
                    model.RateWeCanOffer = 21.20m + baseRate;
                else
                    model.RateWeCanOffer = 41.12m + baseRate;


                if(c.HasEverBeenLatePaying)
                {
                    model.RateWeCanOffer += 10.0m;
                }

            }






            return View(model);
        }

        int GetAge(string personnummer)
        {
            if (personnummer.Length == 10) //8101011234
                return DateTime.Now.Year - 1900 - Convert.ToInt32(personnummer.Substring(0,2));

            if (personnummer.Length == 12 &&  !personnummer.Contains("-")) //198101011234
                return DateTime.Now.Year - Convert.ToInt32(personnummer.Substring(0, 4));

            if (personnummer.Length == 11) //810101-1234
                return DateTime.Now.Year - 1900 - Convert.ToInt32(personnummer.Substring(0, 2));

            if (personnummer.Length == 13 ) //19810101-1234
                return DateTime.Now.Year - Convert.ToInt32(personnummer.Substring(0, 4));

            //Fake if not correct
            return 50;
        }

        public void SetLoansForCustomer(Customer c)
        {
            string databas = Server.MapPath("~/loans.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                if (parts[0] == c.PersonNummer)
                {
                    var loan = new Loan
                    {
                        LoanNo = parts[1],
                        Belopp = Convert.ToInt32(parts[2]),
                        FromWhen = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        InterestRate = Convert.ToDecimal(parts[4])
                    };

                    c.Loans.Add(loan);
                }
            }

        }

        public void SetInvoicesForCustomer(Customer customer)
        {
            string databas = Server.MapPath("~/invoices.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                var loan = customer.Loans.FirstOrDefault(r => r.LoanNo == parts[0]);
                if (loan == null) continue;
                var invoice = new Invoice
                {
                    InvoiceNo = Convert.ToInt32(parts[1]),
                    Belopp = Convert.ToInt32(parts[2]),
                    InvoiceDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DueDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                };
                loan.Invoices.Add(invoice);
            }


        }

        public void SetPaymentsForCustomer(Customer customer)
        {
            string databas = Server.MapPath("~/payments.txt");
            foreach (var line in System.IO.File.ReadAllLines(databas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                var invoice = customer.Loans.SelectMany(r => r.Invoices).FirstOrDefault(i => i.InvoiceNo == Convert.ToInt32(parts[0]));
                if (invoice == null) continue;
                var payment = new Payment
                {
                    Belopp = Convert.ToInt32(parts[1]),
                    PaymentDate = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    BankPaymentReference = parts[3],
                };
                invoice.Payments.Add(payment);
            }


        }

        public Customer FindCustomer(string personnummer)
        {
            var repository = GetRepository();
            return repository.FindCustomer(personnummer);

            
        }


        public ActionResult GenerateFakeData(int antal)
        {
            var rnd = new Random();
            for(int i =0;i<antal;i++)
            {
                var persnr = rnd.Next(1934, 1999).ToString() +
                    rnd.Next(1, 12).ToString("00") +
                    rnd.Next(1, 28).ToString("00") +
                    rnd.Next(1000, 9999);

                var c = FindCustomer(persnr);
                if (c != null) continue;
                c = new Customer { PersonNummer = persnr };
                SaveToFile(c);

                for(int l=0;  l <= rnd.Next(1,7);l++ )
                {
                    var loan = new Loan
                    {
                        LoanNo = DateTime.Now.AddDays(-rnd.Next(10,2000)).Ticks.ToString(),
                        Belopp = rnd.Next(3,200) * 100,
                        FromWhen = DateTime.Now.AddDays(-rnd.Next(10, 2000)),
                        InterestRate = Convert.ToDecimal(rnd.NextDouble() * (45 - 20) + 20)
                };
                    SaveLoanToFile(c, loan);

                }



            }
            return Content("Done");
        }


    }
}