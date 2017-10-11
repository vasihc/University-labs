using System;
using System.Collections.Generic;
using System.IO;

namespace OPLab
{
    public class Manager : Programmer
    {
        private int _subordinatesQuantity;
        private List<String> _projects;

        public Manager(String name, String surname, Position position, List<String> skills,
                         int subordinatesQuantity, List<String> projects) : base(name, surname, position, skills)
        {
            SubordinatesQuantity = subordinatesQuantity;
            Projects = projects;
        }

        public int SubordinatesQuantity { get => _subordinatesQuantity; set => _subordinatesQuantity = value; }
        public List<String> Projects { get => _projects; set => _projects = value; }

		public string GetProjectsAsString()
		{
			string result = Projects[0] + " ";
            foreach (string s in Projects)
            {
                result += s + " ";
            }
			return result;
		}

        public override void Print() //
        {
            Console.WriteLine("Manager: {0} {1}", Surname, Name);
            Console.WriteLine("Position: {0}", Position);
            Console.WriteLine("Skills: {0}", GetSkillsAsString());
            Console.WriteLine("Projects: {0}", GetProjectsAsString());
            Console.WriteLine("Subordinates quantity: {0}",SubordinatesQuantity); 
            Console.WriteLine();
        }
    }
}
