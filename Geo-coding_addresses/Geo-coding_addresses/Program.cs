using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Geo_coding_addresses
{
   public  class Program
    {
        static void Main(string[] args)
        {
            //serialize
            List<Location> locations = new List<Location>();

            using (System.IO.StreamReader reader = new StreamReader(@"Locations.json"))
            {
                string jsonString = reader.ReadToEnd();

                locations = JsonConvert.DeserializeObject<List<Location>>(jsonString);

                // call the api using the locations
            }

            //call api
            for (int i = 100; i < 110; i++)
            {
                // Create a request for the URL.   
                WebRequest request = WebRequest.Create("https://www.latlong.net/_spm4.php");
                request.Method = "POST";
                // If required by the server, set the credentials.  
                request.Credentials = CredentialCache.DefaultCredentials;
                var headers = new WebHeaderCollection();
                request.Headers["authority"] = "www.latlong.net";
                request.Headers["origin"] = "https://www.latlong.net";
                request.Headers["x-requested-with"] = "XMLHttpRequest";
                //request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.Headers.Add("accept", "*/*");


                request.Headers["sec-fetch-site"] = "same-origin";
                request.Headers["sec-fetch-mode"] = "cors";
                // request.Headers.Add("referer", "https://www.latlong.net/convert-address-to-lat-long.html");
                request.Headers["accept-encoding"] = "utf-8"; //gzip, deflate, br, 
                request.Headers["accept-language"] = "en-US,en;q=0.9";
                request.Headers["cookie"] = "_ga=GA1.2.756030845.1575787276; _gid=GA1.2.1106784365.1575787276; PHPSESSID=naolen6i7fm46o0ahupiqtb1v1";

                string postData = string.Format("c1={0}&action=gpcm&cp=", (locations[i].Address));
                Console.WriteLine(postData);
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byte1 = encoding.GetBytes(postData);

                // Set the content type of the data being posted.
                request.ContentType = "application/x-www-form-urlencoded";

                // Set the content length of the string being posted.
                request.ContentLength = byte1.Length;

                Stream newStream = request.GetRequestStream();

                newStream.Write(byte1, 0, byte1.Length);

                // Get the response.  
                WebResponse response = request.GetResponse();
                // Display the status.  
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                // Get the stream containing content returned by the server. 
                // The using block ensures the stream is automatically closed. 
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.  
                    StreamReader reader1 = new StreamReader(dataStream, Encoding.UTF8, true);

                    // Read the content.  
                    string responseFromServer = reader1.ReadLine();

                    if (responseFromServer != null) {
                        var tokens = responseFromServer.Split(',');
                        locations[i].Lat = (tokens.Count() > 0) ? tokens[0] : "NaN";
                        locations[i].Long = (tokens.Count() > 1) ? tokens[1] : "NaN";
                    }
                    else
                    {
                        locations[i].Lat = "NaN";
                        locations[i].Long = "NaN";
                    }

                    // Display the content.  
                    Console.WriteLine(responseFromServer);
                }

                // Close the response.  
                response.Close();
            }

            string jsonOutput = JsonConvert.SerializeObject(locations);

            using (StreamWriter stream = new StreamWriter("LocationOutput.json"))
            {
                stream.WriteLine(jsonOutput);
            }
        }
    }
}


//RAW REQUEST


/*
//curl 'https://www.latlong.net/_spm4.php' -H 'authority: www.latlong.net' -H 'origin: https://www.latlong.net' -H 
//'x-requested-with: XMLHttpRequest' 
//-H 'user-agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36'
//-H 'content-type: application/x-www-form-urlencoded' 
///-H 'accept: *//*' 
//-H 'sec-fetch-site: same-origin' //
//-H 'sec-fetch-mode: cors' 
//-H 'referer: https://www.latlong.net/convert-address-to-lat-long.html' 
//-H 'accept-encoding: gzip, deflate, br'
//-H 'accept-language: en-US,en;q=0.9' 
//-H 'cookie: _ga=GA1.2.756030845.1575787276; _gid=GA1.2.1106784365.1575787276; PHPSESSID=naolen6i7fm46o0ahupiqtb1v1' 
//--data 'c1=942%2C%2021st%20Main%20Road%2C%202nd%20Stage%2C%20Banashankari%2C%20Bangalore&action=gpcm&cp=' --compressed
//*/