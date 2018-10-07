namespace MobiDigits
{
    static class CurrencyConverter
    {
        public static decimal rate;

        public static string ConvertCurrency(string money)
        {
            var currency = decimal.Parse(money);
            return (currency * rate).ToString("0.00");    
        }
    }
}