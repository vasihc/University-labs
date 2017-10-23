using System;
using System.Collections.Generic;
using System.IO;

namespace OPLab
{
    abstract public class Programmer // Базовый класс
	{
		// Приватные свойства базового класса
		private string _name;
        private string _surname;
        private Position _position;
        private List<String> _skills;


        public Programmer(string name, string surname, Position position, List<String> skills)
		{
            Name = name;
            Surname = surname;
            Position = position;
            Skills = skills;
		}

        public string Name { get => _name; set => _name = value; } // Get- и Set-методы
		public string Surname { get => _surname; set => _surname = value; }
        public Position Position { get => _position; set => _position = value; }
        public List<String> Skills { get => _skills; set => _skills = value;  }

        public virtual void Print() // Абстрактный метод базового класса
		{
            Console.WriteLine("Programmer {0} {1} is {2}. Have skills: {3}", 
                              Name, Surname, Position, GetSkillsAsString());
        }

        public String GetSkillsAsString() // Метод в базовом классе, вызываемый классами-наследниками
		{
            string skills = "";
            foreach(var skill in Skills) skills += skill + ", ";

            return skills;
        }
    }
}