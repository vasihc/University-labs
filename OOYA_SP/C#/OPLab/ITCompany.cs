using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPLab
{
    public class ITCompany
    {
        private string _name;
        private List<Programmer> _programmers;

        public ITCompany(string name, List<Programmer> programmers)
        {
            _name = name;
            _programmers = programmers;
        }

        ~ITCompany()
        {
            //destructor
        }



        public void Sort()
        {
            // _programmers.Sort(0, _programmers.Count, _programmers.First().CompareProgrammers());
        }

        public void Print()
        {
            Console.WriteLine("IT Company: {0}", _name);
            foreach (Programmer programmer in _programmers)
            {
                programmer.Print();
            }
        }

        Position getPosition(string s)
        {
            if (s == "JUNIOR")
            {
                return Position.JUNIOR;
            }
            else if (s == "MIDDLE")
            {
                return Position.MIDDLE;
            }
            else
            {
                return Position.SENIOR;
            }
        }

        List<string> getWords(string s)
        {
            List<string> result;
            int start = 0;
            int end = s.find(" ");
            while (end != std::string::npos) {
                result.push_back(s.substr(start, end - start));
                start = end + 1;
                end = s.find(" ", start);
            }
            result.push_back(s.substr(start, end));
            return result;
        }

        public ITCompany ReadInfoFromFile()
        {
            string path = "input.txt";
           
                using (StreamReader sr = File.OpenText(path))
                {
                    string nameCompany = sr.ReadLine();
                    int programmersQuant = Convert.ToInt32(sr.ReadLine());
                    sr.ReadLine();
                    var allProgrammers = new List<Programmer>();
                    for (int i = 0; i < programmersQuant; i++)
                    {
                        string type = sr.ReadLine();
                        string name = sr.ReadLine();
                        string surname = sr.ReadLine();
                        Position position = getPosition(sr.ReadLine());
                        List<string> skills = getWords(sr.ReadLine());
                        if (type == "Developer")
                        {
                            string knowledgeArea = sr.ReadLine();
                            string currentProject = sr.ReadLine();
                            allProgrammers.Add(new Developer(name, surname, position, skills, knowledgeArea, currentProject));
                        }
                        else if (type == "Manager")
                        {
                            List<string> projects = getWords(sr.ReadLine());
                            int subordinatesQuantity = Convert.ToInt32(sr.ReadLine());
                            allProgrammers.Add(new Manager(name, surname, position, skills, subordinatesQuantity, projects));
                        }
                        sr.ReadLine();
                    }
                    return new ITCompany(nameCompany, allProgrammers);
                }
        }
    }

}


