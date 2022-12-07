using NXP3_GetPublicationDetails.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NXP3_GetPublicationDetails
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            const string userName = "SDL_Soumava";
            const string passWord = "Qwerty1@345";

            Uri serviceUrl = new Uri(@"https://nxp001.sdlproducts.com/ISHWS/"); // requires ending '/' character
            Console.WriteLine("Starting Console application for user: " + userName.ToString());
            Console.WriteLine("Autenticating the user on the specified environment...");
            InfoShareWSHelper infoShareWSHelper = new InfoShareWSHelper(serviceUrl)
            {
                Username = userName,
                Password = passWord
            };

            infoShareWSHelper.Resolve();
            //Issue a token. In other words authenticate
            infoShareWSHelper.IssueToken();

            Console.WriteLine("User " + userName.ToString() + " successfully autenticated on the specified environment.");

            try
            {
                Console.WriteLine("Starting Publication class...");
                List<XmlElement> PubSpesificDetails=await getPubDetails.Run(infoShareWSHelper);
                Console.WriteLine("Ended Publication details...");
                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                await exportPublicationSpesificDetails.Run(PubSpesificDetails, infoShareWSHelper);
                Console.WriteLine("Ended Publication details...");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
