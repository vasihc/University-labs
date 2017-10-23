using System;
using System.IO;
using System.Collections.Generic;

namespace OPLab
{
    public class Developer : Programmer // Класс-наследник
	{
		// Приватные свойства класса наследника
		private String _knowledgeArea;
        private String _currentProject;


		public Developer(String name, String surname, Position position, List<String> skills,
                         String knowledgeArea, String currentProject) : base(name, surname, position, skills)
		{
			KnowledgeArea = knowledgeArea;
			CurrentProject = currentProject;
		}

        public string KnowledgeArea { get => _knowledgeArea; set => _knowledgeArea = value; } // Get- и Set-методы
        public string CurrentProject { get => _currentProject; set => _currentProject = value; }

         public override void  Print() // Переопределенный метод базового класса
		{
            Console.WriteLine("Developer: {0} {1}",this.Surname, this.Name );
            Console.WriteLine("Position: {0}", this.Position);
            Console.WriteLine("Skills: {0}", GetSkillsAsString());
            Console.WriteLine("Knowledge area: {0}", KnowledgeArea);
            Console.WriteLine("Current project: {0}", CurrentProject);
		    Console.WriteLine();
        }
    }
}