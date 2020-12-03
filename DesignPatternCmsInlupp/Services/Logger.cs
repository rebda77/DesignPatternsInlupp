using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Services
{
    public class Logger
    {
        private static Logger theInstance = null;

        private Logger ()
        {
           
        }
        public static Logger GetInstance()
        {
            if (theInstance == null)
                theInstance = new Logger();
            return theInstance;
        }

        public enum Actions{
            CallReceived,
            ViewCustomerPage,
            ListCustomersPage,
            ParametrarPage,
            CreatingCustomer,
            CreatingLoan

        };
        public void LogAction(Actions action, string message)
        {
            System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/log.txt"),  $"{action.ToString()} - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:SS")}  {message}\n");
        }
    }
}