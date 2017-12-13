using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FelineSorter.Components;
using FelineSorter.UnitTests.FixtureHelpers;
using FelineSorter.WebserviceContract;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FelineSorter.UnitTests.Components
{
    public class feline_owner_sorter
    {
        private readonly Fixture _fixture;

        private readonly MockRepository _mockRepository;
        private readonly Mock<ILogger<FelineOwnerSorter>> _loggerMock;

        public feline_owner_sorter()
        {
            _fixture = new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Default);
            _loggerMock = _mockRepository.Create<ILogger<FelineOwnerSorter>>();
        }

        private void InjectParameters(ILogger<FelineOwnerSorter> logger)
        {
            _fixture.Inject(logger);
        }

        [Fact]
        public void ctor_should_throw_if_no_logger_provided()
        {
            InjectParameters(null);
            _fixture.Should().ThrowExceptionWhileCreating<ArgumentNullException, FelineOwnerSorter>($"Value cannot be null.{Environment.NewLine}Parameter name: logger");
        }

        [Fact]
        public void sort_should_group_owners_and_sort_its_cats_correctly()
        {
            var ownerOne = new Owner
            {
                Gender = "Male",
                Pets = new []
                {
                    new Pet
                    {
                        Name = "Zulu",
                        Type = "Cat"
                    },
                    new Pet
                    {
                        Name = "Jim",
                        Type = "Frog"
                    }
                }
            };

            var ownerTwo = new Owner
            {
                Gender = "male", // small "m"
                Pets = new[]
                {
                    new Pet
                    {
                        Name = "Garfield",
                        Type = "Cat"
                    },
                    new Pet
                    {
                        Name = "Tom",
                        Type = "Cat"
                    }
                }
            };

            var ownerThree = new Owner
            {
                Gender = "Male",
                Pets = null
            };

            var ownerFour = new Owner
            {
                Gender = "Female",
                Pets = new []
                {
                    new Pet
                    {
                        Name = "Will",
                        Type = "Cat"
                    },
                    new Pet
                    {
                        Name = null,
                        Type = "Cat"
                    }

                }
            };

            var ownerFive = new Owner
            {
                Gender = "Female",
                Pets = new[]
                {
                    new Pet
                    {
                        Name = "",
                        Type = "Cat"
                    }
                }
            };

            InjectParameters(_loggerMock.Object);
            var felineOwnerSorter = _fixture.Create<FelineOwnerSorter>();

            var owners = new[] {ownerOne, ownerTwo, ownerThree, ownerFour, ownerFive};
            IEnumerable<OwnerAndCats> sortedOwnerAndCats = felineOwnerSorter.Sort(owners);
            sortedOwnerAndCats.Should().NotBeNull();
            sortedOwnerAndCats.Should().HaveCount(2);

            var maleOwner = sortedOwnerAndCats.SingleOrDefault(o => string.Compare(o.Gender, "male", StringComparison.InvariantCultureIgnoreCase) == 0);
            maleOwner.Should().NotBeNull();
            maleOwner.Cats.Should().HaveCount(3);
        }
    }
}