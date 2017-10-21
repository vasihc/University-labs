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

        public ITCompany()
        { }

        public ITCompany(string name, List<Programmer> programmers)
        {
            _name = name;
            _programmers = programmers;
        }

        ~ITCompany()
        {
            //destructor
            Console.WriteLine("Company was deleted.");
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public List<Programmer> Programmers
        {
            get => _programmers;
            set => _programmers = value;
        }

        public void Sort()
        {
            _programmers =  _programmers.OrderBy(p => p.Position ).ThenBy(p => p.Name).ThenBy(p => p.Surname).ToList();
        }

        public void Print()
        {
            Console.WriteLine("IT Company: {0}", _name);
            Console.WriteLine();
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
            List<string> result = new List<string>();
            int start = 0;
            int end = s.IndexOf(" ");
            while (end > 0)
            {
                var a = s.Substring(start, end - start);
                result.Add(a);
                start = end + 1;
                end = s.IndexOf(" ", start);
            }
            //result.Add(s.Substring(start, end));
            return result;
        }

        public ITCompany ReadInfoFromFile()
        {
            string path = "input.txt";
           
                using (StreamReader sr = File.OpenText(path))
                {
                    string nameCompany = sr.ReadLine();
                    string tmp = sr.ReadLine();
                    int programmersQuant = Convert.ToInt32(tmp);
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
                    // sr.ReadLine();
                    }
                    var result = new ITCompany(nameCompany, allProgrammers);
                    return result;
                }
        }

        public void DeleteNotOdd()
        {
            List<Programmer> programmers = new List<Programmer>();
            for (int i = 0; i < this._programmers.Count(); i++)
            {
                if (this._programmers[i].GetType() == typeof(Developer))
                {
                    var developer = (Developer)Convert.ChangeType(this.Programmers[i], typeof(Developer));
                    if (Convert.ToInt32(developer.CurrentProject[developer.CurrentProject.Length - 2])%2 != 0)
                    {
                        programmers.Add(developer);
                    }
                }
                else
                {
                    programmers.Add(this._programmers[i]);
                }
            }
			this._programmers = null;
			this._programmers = programmers;
        }
    }

}


