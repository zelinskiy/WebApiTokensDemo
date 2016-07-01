using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TokenCalcTester
{
    class Program
    {
        

        static void Main(string[] args)
        {
            var tester = new CalculatorTester();

            tester.BaseUrl = "http://localhost:7000/";
            tester.Username = "TEST";
            tester.Password = "TEST123";

            
            


            tester.RunMainTest();
            

            Console.ReadLine();
        }       


    }

    public class CalculatorTester
    {
        Random rand = new Random();

        public string Username { get; set; }

        public string Password { get; set; }

        public string BaseUrl { get; set; } = "http://localhost:5000/";

        public string Token { get; set; }


        public async void RunMainTest()
        {
            Console.Beep(500, 100);
            Console.WriteLine("Wait for token...");
            await getToken();            
            await testMultiplication(30, 100, false);
            Console.Beep(500, 100);



            /*
            Console.WriteLine("Wait 1 minute...");
            Thread.Sleep(TimeSpan.FromMinutes(1));
            await testMultiplication(30, 100, false);
            */

            
            Console.WriteLine("Wait 2 minute...");
            for (int i = 2 * 60; i > 0; i--)
            {
                Console.WriteLine($"Wait {i} seconds...");
                Thread.Sleep(1000);
            }
            await testMultiplication(30, 100, true);
            
            
            

            

            /*
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    Console.Beep(500 + i * 10, 100);
                }
            }
            */
        }

        public async Task<bool> testAddition(int n, int max, bool verbose)
        {
            HttpResponseMessage[] responces = new HttpResponseMessage[n];

            using (HttpClient client = new HttpClient())
            {
                for(int i = 0; i < n; i++)
                {
                    int a = rand.Next(max);
                    int b = rand.Next(max);

                    var responce = await client.GetAsync($"{BaseUrl}api/calculator/add/{a}/{b}");
                    if (verbose)
                    {
                        Console.WriteLine($"GET Request #{i} finished with code {responce.StatusCode} => {a} + {b} = {await responce.Content.ReadAsStringAsync()}");
                    }
                    
                    responces[i] = responce;
                }
            }

            var allSucceed = responces.All(r => r.IsSuccessStatusCode);
            Console.WriteLine("All addition requests successfull: " + allSucceed);

            return allSucceed;
        }


        public async Task<IEnumerable<HttpResponseMessage>> testMultiplication(int n, int max, bool verbose)
        {
            /*
            if (string.IsNullOrEmpty(Token))
            {
                throw new Exception("Token isnt set");
            }        
            */

            HttpResponseMessage[] responces = new HttpResponseMessage[n];

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < n; i++)
                {
                    int a = rand.Next(max);
                    int b = rand.Next(max);

                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri($"{BaseUrl}api/calculator/mult/{a}/{b}"),
                        Method = HttpMethod.Get,
                    };
                    request.Headers.Add("Authorization", "Bearer " + Token);

                    var responce = await client.SendAsync(request);
                    if (verbose)
                    {
                        Console.WriteLine($"GET Request #{i} finished with code {responce.StatusCode} => {a} * {b} = {await responce.Content.ReadAsStringAsync()}");
                    }

                    responces[i] = responce;
                }
            }

            Console.WriteLine("All multiplication requests successfull: " + responces.All(r => r.IsSuccessStatusCode));

            return responces;
        }



        public async Task<string> getToken()
        {
            if(Username == null || Password == null)
            {
                throw new Exception("Cant username and/or password not set");
            }

            string token;

            using (HttpClient client = new HttpClient())
            {
                
                var content = new StringContent($"username={Username}&password={Password}");
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
                var responce = await client.PostAsync(BaseUrl+"token", content);
                Console.WriteLine("Requested token with code " + responce.StatusCode);
                dynamic data = JsonConvert.DeserializeObject(await responce.Content.ReadAsStringAsync());
                token = data.access_token;
            }
            Console.WriteLine(token);
            Token = token;
            return token;
        }
        

    }

}
