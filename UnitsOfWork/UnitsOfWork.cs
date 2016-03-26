namespace UnitsOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Unit : IComparable<Unit>
    {
        private const string ToStringTemplate = "{0}[{1}]({2})";

        public Unit(string name, string type, ushort attack)
        {
            this.Name = name;
            this.Type = type;
            this.Attack = attack;
        }

        public string Name { get; private set; }

        public string Type { get; private set; }

        public ushort Attack { get; private set; }

        public int CompareTo(Unit other)
        {
            var compareResult = -this.Attack.CompareTo(other.Attack);
            if (compareResult == 0)
            {
                compareResult = this.Name.CompareTo(other.Name);
                if (compareResult == 0)
                {
                    compareResult = this.Type.CompareTo(other.Type);
                }
            }

            return compareResult;
        }

        private bool Equals(Unit other)
        {
            return string.Equals(this.Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Unit) obj);
        }

        public override int GetHashCode()
        {
            return (this.Name != null ? this.Name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format(ToStringTemplate, this.Name, this.Type, this.Attack);
        }
    }

    public class UnitVault
    {
        private const string AddSuccess = "SUCCESS: {0} added!";
        private const string AddFailure = "FAIL: {0} already exists!";
        private const string RemoveSuccess = "SUCCESS: {0} removed!";
        private const string RemoveFailure = "FAIL: {0} could not be found!";
        private const string PrintResultsTemplate = "RESULT: {0}";

        private readonly Dictionary<string, Unit> allUnits; 
        private readonly Dictionary<string, SortedSet<Unit>> unitsByType;
        private readonly SortedSet<Unit> sortdUnits;

        public UnitVault()
        {
            this.allUnits = new Dictionary<string, Unit>();
            this.sortdUnits = new SortedSet<Unit>();
            this.unitsByType = new Dictionary<string, SortedSet<Unit>>();
        }

        public void AddUnit(string name, string type, ushort attack)
        {
            if (this.allUnits.ContainsKey(name))
            {
                Console.WriteLine(AddFailure, name);
                return;
            }

            var unit = new Unit(name, type, attack);

            this.allUnits.Add(name, unit);

            this.sortdUnits.Add(unit);

            if (!this.unitsByType.ContainsKey(unit.Type))
            {
                this.unitsByType[unit.Type] = new SortedSet<Unit>();
            }

            this.unitsByType[unit.Type].Add(unit);
            Console.WriteLine(AddSuccess, unit.Name);
        }

        public void RemoveUnit(string name)
        {
            if (!this.allUnits.ContainsKey(name))
            {
                Console.WriteLine(RemoveFailure, name);
                return;
            }

            var unitToRemove = this.allUnits[name];
            this.unitsByType[unitToRemove.Type].Remove(unitToRemove);
            this.sortdUnits.Remove(unitToRemove);
            this.allUnits.Remove(name);

            Console.WriteLine(RemoveSuccess, name);
        }

        public void FindUnit(string type)
        {
            if (this.unitsByType.ContainsKey(type))
            {
                var res = this.unitsByType[type].Take(10);
                Console.WriteLine(PrintResultsTemplate, string.Join(", ", res));
                return;
            }

            Console.WriteLine(PrintResultsTemplate, string.Empty);
        }

        public void FindUnit(int topCount)
        {
            var res = this.sortdUnits.Take(topCount);

            Console.WriteLine(PrintResultsTemplate, string.Join(", ", res));
        }
    }

    public static class UnitsOfWork
    {
        private static readonly UnitVault Units = new UnitVault();

        public static void Main()
        {
            var command = Console.ReadLine().Split();
            while (command[0] != "end")
            {
                var commandName = command[0];
                var commandArgs = command.Skip(1).ToArray();

                ExecuteCommand(commandName, commandArgs);

                command = Console.ReadLine().Split();
            }
        }

        private static void ExecuteCommand(string commandName, string[] commandArgs)
        {
            switch (commandName)
            {
                case "add":
                    Units.AddUnit(commandArgs[0], commandArgs[1], ushort.Parse(commandArgs[2]));
                    break;
                case "remove":
                    Units.RemoveUnit(commandArgs[0]);
                    break;
                case "find":
                    Units.FindUnit(commandArgs[0]);
                    break;
                case "power":
                    Units.FindUnit(ushort.Parse(commandArgs[0]));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
