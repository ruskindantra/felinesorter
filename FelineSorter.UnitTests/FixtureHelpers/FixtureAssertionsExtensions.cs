using AutoFixture;

namespace FelineSorter.UnitTests.FixtureHelpers
{
    public static class FixtureAssertionsExtensions
    {
        public static FixtureAssertions Should(this Fixture actual)
        {
            return new FixtureAssertions(actual);
        }
    }
}