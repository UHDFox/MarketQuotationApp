using System.Text;
using System.Xml;

namespace MarketQuotationApp;

class Program
{
    static async Task Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Регистрируем кодировки

        string requestUrl = "http://www.cbr.ru/scripts/XML_daily.asp";
        
        while (true)
        {
            Console.WriteLine("Enter currency code (e.g., 'USD', 'EUR', 'GBP') or 'exit' to quit:");
            var currencyCode = Console.ReadLine().ToUpper();

            if (currencyCode.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Exiting the program.");
                break;
            }

            try
            {
                var rates = await GetCurrencyRatesAsync(requestUrl);

                var rate = FindQuotation(rates, currencyCode.ToUpper());

                if (rate == null)
                {
                    Console.WriteLine($"Currency {currencyCode.ToUpper()} not found. Try again.");

                }
                Console.WriteLine($"Quotation - {currencyCode.ToUpper()}: {rate} RUB");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
    
    
    static async Task<XmlDocument> GetCurrencyRatesAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetByteArrayAsync(url);
            
            string responseString = Encoding.GetEncoding("windows-1251").GetString(response);
            
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseString);

            return xmlDoc;
        }
    }
    
    static string FindQuotation(XmlDocument rates, string currencyCode)
    {
        XmlNode currencyNode = rates.SelectSingleNode($"//Valute[CharCode='{currencyCode}']");
        
        return currencyNode?.SelectSingleNode("Value")?.InnerText;
    }
}