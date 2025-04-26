using System;
using System.Collections.Generic;
using System.Linq;
using ZooManagement.Domain.ValueObject;

namespace ZooManagement.Domain.Entities
{
    public class Animal
    {
        public Guid Id { get; private set; }
        public string Species { get; private set; }
        public string Nickname { get; private set; }
        public DateTime BirthDate { get; private set; }
        public string Gender { get; private set; }
        public Food PreferredFood { get; private set; }
        public string Health { get; private set; } = "Healthy";
        public Guid? CurrentEnclosureId { get; private set; }

        private readonly List<DateTime> _feedLog = new();
        public IReadOnlyList<DateTime> FeedLog => _feedLog;

        public Animal(string species, string nickname, DateTime birthDate, string gender, Food preferredFood)
        {
            Id = Guid.NewGuid();
            Species = species ?? throw new ArgumentNullException(nameof(species));
            Nickname = nickname ?? throw new ArgumentNullException(nameof(nickname));
            BirthDate = birthDate;
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            PreferredFood = preferredFood ?? throw new ArgumentNullException(nameof(preferredFood));
        }

        public void Feed(Food food)
        {
            if (food == null) throw new ArgumentNullException(nameof(food));
            if (food != PreferredFood)
                throw new InvalidOperationException("Provided food is not preferred by the animal.");
            _feedLog.Add(DateTime.UtcNow);
        }

        public void Infect() => Health = "Sick";

        public void Recover() => Health = "Healthy";

        public void MoveTo(Guid enclosureId) => CurrentEnclosureId = enclosureId;

        public int GetAgeInYears()
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public bool IsHungerAlert()
        {
            if (!_feedLog.Any()) return true;
            var lastFed = _feedLog.Max();
            return (DateTime.UtcNow - lastFed) > TimeSpan.FromHours(24);
        }
    }
}
