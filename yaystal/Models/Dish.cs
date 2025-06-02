using System;
using System.Collections.Generic;

namespace yaystal.Models
{
    /// <summary>
    /// </summary>
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Composition { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; } 
        public string PhotoPath { get; set; }
        public int CookingTimeMinutes { get; set; }

        public Dish()
        {
        }

        public Dish(int id, string name, string composition, string description, double weight, decimal price, string photoPath, int cookingTimeMinutes)
        {
            Id = id;
            Name = name;
            Composition = composition;
            Description = description;
            Weight = weight;
            Price = price;
            PhotoPath = photoPath;
            CookingTimeMinutes = cookingTimeMinutes;
        }
    }
}
