using System;

namespace yaystal.Models
{
    /// <summary>
    /// Перечисление типов сотрудников
    /// </summary>
    public enum EmployeeType
    {
        Administrator,
        Cook,
        Courier
    }

    /// <summary>
    /// Базовый класс для всех сотрудников
    /// </summary>
    public abstract class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EmployeeType Type { get; protected set; }
        public double EfficiencyMetric { get; set; } // метрика эффективности

        protected Employee()
        {
        }

        protected Employee(int id, string name)
        {
            Id = id;
            Name = name;
            EfficiencyMetric = 0;
        }

        // Абстрактный метод для расчета метрики эффективности
        public abstract void CalculateEfficiencyMetric();
    }

    /// <summary>
    /// </summary>
    public class Administrator : Employee
    {
        public Administrator() : base()
        {
            Type = EmployeeType.Administrator;
        }

        public Administrator(int id, string name) : base(id, name)
        {
            Type = EmployeeType.Administrator;
        }

        public override void CalculateEfficiencyMetric()
        {
            //пока не знаю как..
        }
    }

    /// <summary>
    /// </summary>
    public class Cook : Employee
    {
        public Cook() : base()
        {
            Type = EmployeeType.Cook;
        }

        public Cook(int id, string name) : base(id, name)
        {
            Type = EmployeeType.Cook;
        }

        public override void CalculateEfficiencyMetric()
        {
            //пока не знаю как..
        }
    }

    /// <summary>
    /// </summary>
    public class Courier : Employee
    {
        public Courier() : base()
        {
            Type = EmployeeType.Courier;
        }

        public Courier(int id, string name) : base(id, name)
        {
            Type = EmployeeType.Courier;
        }

        public override void CalculateEfficiencyMetric()
        {
            //пока не знаю как..
        }
    }
}
