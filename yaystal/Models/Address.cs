using System;

namespace yaystal.Models
{
    /// <summary>
    /// </summary>
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public double X { get; set; } // X
        public double Y { get; set; } // Y 
        public bool IsMain { get; set; } // Флаг

        public Address()
        {
        }

        public Address(int id, string street, string houseNumber, string apartmentNumber, bool isMain = false)
        {
            Id = id;
            Street = street;
            HouseNumber = houseNumber;
            ApartmentNumber = apartmentNumber;
            IsMain = isMain;
            
            GenerateRandomCoordinates();
        }

        private void GenerateRandomCoordinates()
        {
            Random random = new Random();
            X = random.NextDouble() * 100; 
            Y = random.NextDouble() * 100; 
        }

        public override string ToString()
        {
            return $"{Street}, {HouseNumber}, кв. {ApartmentNumber}";
        }
    }
}
