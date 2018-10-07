using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Android.App;
using Android.Widget;

namespace MobiDigits
{
    static class CurrencyGetter
    {
        static Dictionary<string, dynamic> webRates = new Dictionary<string, dynamic>();

        const string urlBase = "http://apilayer.net/api/live?access_key=";
        public static string urlKey = "";
        const string urlCurrencies = "&currencies=";
        const string urlToCurrenciesFromUSD = "EUR,GBP,RUB";
        const string urlToCurrenciesFromEUR = "USD,GBP,RUB";
        const string urlToCurrenciesFromGPB = "EUR,USD,RUB";
        const string urlToCurrenciesFromRUB = "EUR,GBP,USD";
        const string urlSource = "&source=";
        const string urlFromCurrencyUSD = "USD";
        const string urlFromCurrencyEUR = "EUR";
        const string urlFromCurrencyGBP = "GBP";
        const string urlFromCurrencyRUB = "RUB";
        const string urlFormat = "&format=1";

        static string GetWebCurrencyRate(string from)
        {
            var urlToCurrencies = "";

            switch (from)
            {
                case urlFromCurrencyUSD: urlToCurrencies = urlToCurrenciesFromUSD; break;
                case urlFromCurrencyEUR: urlToCurrencies = urlToCurrenciesFromEUR; break;
                case urlFromCurrencyGBP: urlToCurrencies = urlToCurrenciesFromGPB; break;
                case urlFromCurrencyRUB: urlToCurrencies = urlToCurrenciesFromRUB; break;
            }

            var urlBuilder = new StringBuilder();
            urlBuilder.Append(urlBase).Append(urlKey).Append(urlCurrencies).Append(urlToCurrencies)
                      .Append(urlSource).Append(from).Append(urlFormat);
            var web = new WebClient();
            
            return web.DownloadString(urlBuilder.ToString());
        }

        public static decimal GetRate(string from, string to)
        {
            if (string.IsNullOrEmpty(urlKey) || from == to) return 1.00M;

            if (webRates.ContainsKey(from)
                && webRates[from].timestamp <= DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 7200)
                return GetStoredRate(from, to);
            try
            {
                var responce = GetWebCurrencyRate(from);

                var rates = CurrencyParser.ParseCurrencyJSON(responce);

                if (rates.success != "true") return FallBackRateCalculator();

                webRates[from] = rates;
                return GetStoredRate(from, to);
            }

            catch
            {
                Toast.MakeText(Application.Context, "Some error occured!",
                    ToastLength.Short).Show();
                return 1.00M;
            }

            decimal FallBackRateCalculator()
            {
                Toast.MakeText(Application.Context, $"Cannot get rate, trying to make a {from} to USD to {to} conversion!",
                    ToastLength.Short).Show();

                decimal from_From_ToUsd = GetRate(urlFromCurrencyUSD, from);
                decimal fromUSDTo_To_ = GetRate(urlFromCurrencyUSD, to);
                return fromUSDTo_To_ / from_From_ToUsd;
            }
        }

        static decimal GetStoredRate(string from, string to)
        {
            var result = webRates[from];

            var obj = result["quotes"];
            var val = (string) obj[from + to];
                
            return CurrencyParser.ParseCurrencyString(val);
        }
    }
}