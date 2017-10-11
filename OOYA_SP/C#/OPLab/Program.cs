using System;

namespace OPLab
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ITCompany company = new ITCompany();

           company =  company.ReadInfoFromFile();

           company.Sort();
           company.Print();

           company = null;
           Console.ReadLine();

        }
    }
}