using Newtonsoft.Json;

namespace MobiDigits
{
    static class CurrencyParser
    {
        public static dynamic ParseCurrencyJSON(string responce)
        {
            return JsonConvert.DeserializeObject(responce);
        }

        public static decimal ParseCurrencyString(string rate)
        {
            return decimal.Parse(rate);
        }
    }
}