using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ValidareDate.Helpers
{
    public class CNPHelper
    {
        public static bool CheckCnp(string cnp)
        {
            return checkValidDateForCNP(cnp.Substring(1, 6)) && checkValidCnp(cnp);
        }

        private static bool checkValidCnp(string cnp)
        {
            var validation = "279146358279";
            int sum = 0;
            for (int i = 0; i < validation.Length; i++)
            {
                sum += ConvertStr(validation[i]) * ConvertStr(cnp[i]);
            }

            int rest = sum % 11;

            if (rest == 10)
            {
                rest = 1;
            }

            return (ConvertStr(cnp.Last()) == rest);
        }

        private static int ConvertStr(char c)
        {
            return c - '0';
        }

        private static bool checkValidDateForCNP(string date)
        {
            try
            {
                DateTime.ParseExact(date, "yyMMdd", CultureInfo.CurrentCulture);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}