using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZPayroll.MVC.Tools
{
    public static class TotalCalculator
    {
       public static decimal GetCaptureTotal(decimal _rate,int hours)
        {
            decimal _total = 0;
            try
            {
                _total = _rate* hours;
            }
            catch (Exception)
            {

             //log
            }

            return _total;
        }
    }
}
