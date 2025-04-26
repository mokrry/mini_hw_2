using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.ValueObject;
using ZooManagement.Infrastructure.Repositories;
using ZooManagement.Application.Services;

namespace ZooManagement.Tests
{
    public class FeedHistoryTests
    {
        [Fact]
        public void Feeding_WithPreferredFood_DoesNotThrow()
        {
            var rhino = new Animal("Rhinoceros", "Rocky", DateTime.UtcNow.AddYears(-5), "M", new Food("Leaves"));
            var ex = Record.Exception(() => rhino.Feed(new Food("Leaves")));
            Assert.Null(ex);
        }

        [Fact]
        public void Feeding_WithWrongFood_ThrowsInvalidOperation()
        {
            var rhino = new Animal("Rhinoceros", "Rocky", DateTime.UtcNow.AddYears(-5), "M", new Food("Leaves"));
            Assert.Throws<InvalidOperationException>(() => rhino.Feed(new Food("Meat")));
        }

        [Fact]
        public void NewAnimal_HasEmptyFeedLog()
        {
            var hippo = new Animal("Hippopotamus", "HappyHippo", DateTime.UtcNow.AddYears(-4), "F", new Food("AquaticPlants"));
            Assert.Empty(hippo.FeedLog);
        }

        [Fact]
        public void Feeding_CorrectFood_RecordsFeedLog()
        {
            var before = DateTime.UtcNow;
            var rhino = new Animal("Rhinoceros", "Rocky", before.AddYears(-5), "M", new Food("Leaves"));
            rhino.Feed(new Food("Leaves"));
            var after = DateTime.UtcNow;
            Assert.Single(rhino.FeedLog);
            var ts = rhino.FeedLog.First();
            Assert.InRange(ts, before, after);
        }

        [Fact]
        public void MultipleFeedings_AppendMultipleEntries()
        {
            var koala = new Animal("Koala", "Koopie", DateTime.UtcNow.AddYears(-3), "F", new Food("Eucalyptus"));
            koala.Feed(new Food("Eucalyptus"));
            Thread.Sleep(10);
            koala.Feed(new Food("Eucalyptus"));
            Assert.Equal(2, koala.FeedLog.Count);
            Assert.True(koala.FeedLog[1] > koala.FeedLog[0]);
        }
    }

    public class AnimalHealthTests
    {
        [Fact]
        public void NewBear_IsHealthyByDefault()
        {
            var bear = new Animal("Bear", "Brownie", DateTime.UtcNow.AddYears(-4), "F", new Food("Berries"));
            Assert.Equal("Healthy", bear.Health);
        }

        [Fact]
        public void Infect_ChangesHealthToSick()
        {
            var elephant = new Animal("Elephant", "Elly", DateTime.UtcNow.AddYears(-6), "F", new Food("Grass"));
            elephant.Infect();
            Assert.Equal("Sick", elephant.Health);
        }

        [Fact]
        public void Recover_AfterInfect_ChangesHealthToHealthy()
        {
            var elephant = new Animal("Elephant", "Elly", DateTime.UtcNow.AddYears(-6), "F", new Food("Grass"));
            elephant.Infect();
            elephant.Recover();
            Assert.Equal("Healthy", elephant.Health);
        }
    }

    public class EnclosureBehaviorTests
    {
        [Fact]
        public void AddAnimal_UnderCapacity_AllowsAddition()
        {
            var pen = new Enclosure("Arctic", 300, 3);
            var polarBear = new Animal("PolarBear", "Polly", DateTime.UtcNow.AddYears(-5), "F", new Food("Fish"));
            pen.AddAnimal(polarBear);
            Assert.Contains(polarBear, pen.Residents);
            Assert.Equal(1, pen.AnimalsCount);
        }

        [Fact]
        public void AddAnimal_AtCapacity_ThrowsInvalidOperation()
        {
            var pen = new Enclosure("Arctic", 300, 1);
            var pb1 = new Animal("PolarBear", "Polly", DateTime.UtcNow.AddYears(-5), "F", new Food("Fish"));
            var pb2 = new Animal("PolarBear", "Pete", DateTime.UtcNow.AddYears(-4), "M", new Food("Fish"));
            pen.AddAnimal(pb1);
            Assert.Throws<InvalidOperationException>(() => pen.AddAnimal(pb2));
        }
    }

    public class ScheduleTests
    {
        [Fact]
        public void Reschedule_ChangesScheduleTime()
        {
            var zebra = new Animal("Zebra", "Ziggy", DateTime.UtcNow.AddYears(-3), "M", new Food("Grass"));
            var original = DateTime.UtcNow.AddHours(1);
            var schedule = new FeedingSchedule(zebra, original, new Food("Grass"));
            var updated = original.AddMinutes(15);
            schedule.Reschedule(updated);
            Assert.Equal(updated, schedule.ScheduleTime);
        }

        [Fact]
        public void CompleteMark_SetsCompletedTrue()
        {
            var zebra = new Animal("Zebra", "Ziggy", DateTime.UtcNow.AddYears(-3), "M", new Food("Grass"));
            var schedule = new FeedingSchedule(zebra, DateTime.UtcNow.AddHours(1), new Food("Grass"));
            schedule.Complete();
            Assert.True(schedule.Completed);
        }
    }

    public class TransferServiceTests
    {
        [Fact]
        public void TransferAnimal_MovesAnimalBetweenPens()
        {
            var aRepo = new InMemoryAnimalRepository();
            var eRepo = new InMemoryEnclosureRepository();
            var penA = new Enclosure("Mountain", 120, 2);
            var penB = new Enclosure("Valley", 140, 2);
            eRepo.Add(penA);
            eRepo.Add(penB);

            var goat = new Animal("Goat", "Gerty", DateTime.UtcNow.AddYears(-2), "F", new Food("Hay"));
            aRepo.Add(goat);
            penA.AddAnimal(goat);
            goat.MoveTo(penA.Id);

            var service = new AnimalTransferService(aRepo, eRepo);
            service.TransferAnimal(goat.Id, penB.Id);

            Assert.DoesNotContain(goat, penA.Residents);
            Assert.Contains(goat, penB.Residents);
            Assert.Equal(penB.Id, goat.CurrentEnclosureId);
        }

        [Fact]
        public void TransferAnimal_MissingAnimal_ThrowsKeyNotFound()
        {
            var service = new AnimalTransferService(new InMemoryAnimalRepository(), new InMemoryEnclosureRepository());
            Assert.Throws<KeyNotFoundException>(() => service.TransferAnimal(Guid.NewGuid(), Guid.NewGuid()));
        }
    }

    public class StatisticsServiceTests
    {
        [Fact]
        public void Totals_And_FreeCounts_AreCorrect()
        {
            var aRepo = new InMemoryAnimalRepository();
            var eRepo = new InMemoryEnclosureRepository();
            aRepo.Add(new Animal("Wolf", "Winston", DateTime.UtcNow.AddYears(-3), "M", new Food("Meat")));
            aRepo.Add(new Animal("Fox", "Fiona", DateTime.UtcNow.AddYears(-2), "F", new Food("Berries")));
            eRepo.Add(new Enclosure("Forest", 500, 5));

            var stats = new ZooStatisticsService(aRepo, eRepo);
            Assert.Equal(2, stats.GetTotalAnimals());
            Assert.Equal(1, stats.GetFreeEnclosuresCount());
        }
    }

    public class RepositoryTests
    {
        [Fact]
        public void AnimalRepo_AddGetRemove_WorksProperly()
        {
            var repo = new InMemoryAnimalRepository();
            var deer = new Animal("Deer", "Daisy", DateTime.UtcNow.AddYears(-1), "F", new Food("Leaves"));
            repo.Add(deer);
            Assert.Equal(deer, repo.GetById(deer.Id));
            repo.Remove(deer.Id);
            Assert.Null(repo.GetById(deer.Id));
        }

        [Fact]
        public void EnclosureRepo_AddGetRemove_WorksProperly()
        {
            var repo = new InMemoryEnclosureRepository();
            var enclosure = new Enclosure("Test", 100, 2);
            repo.Add(enclosure);
            Assert.Equal(enclosure, repo.GetById(enclosure.Id));
            repo.Remove(enclosure.Id);
            Assert.Null(repo.GetById(enclosure.Id));
        }

        [Fact]
        public void FeedingScheduleRepo_AddGetRemove_WorksProperly()
        {
            var aRepo = new InMemoryAnimalRepository();
            var repo = new InMemoryFeedingScheduleRepository();
            var fox = new Animal("Fox", "Fiona", DateTime.UtcNow.AddYears(-2), "F", new Food("Berries"));
            aRepo.Add(fox);
            var sched = new FeedingSchedule(fox, DateTime.UtcNow, new Food("Berries"));
            repo.Add(sched);
            Assert.Equal(sched, repo.GetById(sched.Id));
            repo.Remove(sched.Id);
            Assert.Null(repo.GetById(sched.Id));
        }
    }

    public class AnimalExtensionsTests
    {
        [Fact]
        public void GetAgeInYears_CalculatesCorrectly()
        {
            var birthDate = DateTime.Today.AddYears(-10).AddDays(1);
            var animal = new Animal("Test", "Teddy", birthDate, "M", new Food("TestFood"));
            Assert.Equal(9, animal.GetAgeInYears());
        }

        [Fact]
        public void IsHungerAlert_True_WhenNeverFed()
        {
            var animal = new Animal("Test", "Teddy", DateTime.UtcNow.AddYears(-1), "M", new Food("TestFood"));
            Assert.True(animal.IsHungerAlert());
        }

        [Fact]
        public void IsHungerAlert_False_IfFedRecently()
        {
            var animal = new Animal("Test", "Teddy", DateTime.UtcNow.AddYears(-1), "M", new Food("TestFood"));
            animal.Feed(new Food("TestFood"));
            Assert.False(animal.IsHungerAlert());
        }
    }

    public class EnclosureExtensionsTests
    {
        [Fact]
        public void IsFull_ReturnsTrue_WhenAtCapacity()
        {
            var enclosure = new Enclosure("Test", 100, 1);
            var animal = new Animal("Test", "A", DateTime.UtcNow, "M", new Food("F"));
            enclosure.AddAnimal(animal);
            Assert.True(enclosure.IsFull());
        }

        [Fact]
        public void GetOccupancyRate_ReturnsCorrectFraction()
        {
            var enclosure = new Enclosure("Test", 100, 4);
            for (int i = 0; i < 2; i++)
            {
                var animal = new Animal("Test", i.ToString(), DateTime.UtcNow, "M", new Food("F"));
                enclosure.AddAnimal(animal);
            }
            Assert.Equal(0.5, enclosure.GetOccupancyRate(), 3);
        }
    }

    public class TransferServiceBatchTests
    {
        [Fact]
        public void TransferAnimals_MovesAllSpecifiedAnimals()
        {
            var aRepo = new InMemoryAnimalRepository();
            var eRepo = new InMemoryEnclosureRepository();
            var source = new Enclosure("Src", 100, 5);
            var dest = new Enclosure("Dst", 100, 5);
            eRepo.Add(source);
            eRepo.Add(dest);

            var ids = new List<Guid>();
            for (int i = 0; i < 3; i++)
            {
                var animal = new Animal("Test", i.ToString(), DateTime.UtcNow, "M", new Food("F"));
                aRepo.Add(animal);
                source.AddAnimal(animal);
                animal.MoveTo(source.Id);
                ids.Add(animal.Id);
            }

            var service = new AnimalTransferService(aRepo, eRepo);
            service.TransferAnimals(ids, dest.Id);

            Assert.Equal(3, dest.Residents.Count);
            Assert.Equal(0, source.Residents.Count);
        }

        [Fact]
        public void TransferAnimals_ThrowsOnOverCapacity()
        {
            var aRepo = new InMemoryAnimalRepository();
            var eRepo = new InMemoryEnclosureRepository();
            var source = new Enclosure("Src", 100, 2);
            var dest = new Enclosure("Dst", 100, 1);
            eRepo.Add(source);
            eRepo.Add(dest);

            var animal1 = new Animal("Test", "A1", DateTime.UtcNow, "M", new Food("F"));
            var animal2 = new Animal("Test", "A2", DateTime.UtcNow, "M", new Food("F"));
            aRepo.Add(animal1);
            aRepo.Add(animal2);
            source.AddAnimal(animal1);
            source.AddAnimal(animal2);
            animal1.MoveTo(source.Id);
            animal2.MoveTo(source.Id);

            var service = new AnimalTransferService(aRepo, eRepo);
            service.TransferAnimal(animal1.Id, dest.Id);

            Assert.Throws<InvalidOperationException>(() => service.TransferAnimals(new[] { animal2.Id }, dest.Id));
        }
    }
}
