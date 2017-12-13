using System;
using System.Reflection;
using AutoFixture;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace FelineSorter.UnitTests.FixtureHelpers
{
    public class FixtureAssertions : ObjectAssertions
    {
        private readonly Fixture _value;

        public FixtureAssertions(Fixture value) : base(value)
        {
            _value = value;
        }

        public FixtureAssertions ThrowExceptionWhileCreating<TException, TItem>(string message, string reason = "", params object[] reasonArgs) where TException : Exception
        {
            try
            {
                _value.Create<TItem>();
            }
            catch (ObjectCreationException oce)
            {
                Execute.Assertion
                    .ForCondition(oce.InnerException != null)
                    .FailWith($"Inner exception was null but we were expecting it to be of type <{typeof(TargetInvocationException).Name}>")
                    .Then
                    .ForCondition(oce.InnerException is TargetInvocationException)
                    .FailWith(
                        $"Inner exception was of type <{oce.InnerException.GetType().Name}> but we were expecting it to be of type <{typeof(TargetInvocationException).Name}>")
                    .Then
                    .ForCondition(oce.InnerException.InnerException != null)
                    .FailWith($"Inner.Inner exception was null but we were expecting it to be of type <{typeof(TException).Name}>")
                    .Then
                    .ForCondition(oce.InnerException.InnerException is TException)
                    .FailWith(
                        $"Inner.Inner exception was of type <{oce.InnerException.InnerException.GetType().Name}> but we were expecting it to be of type <{typeof(TException).Name}>")
                    .Then
                    .ForCondition(oce.InnerException.InnerException.Message == message)
                    .BecauseOf(reason, reasonArgs)
                    .FailWith($"Inner most exception message was  <{oce.InnerException.InnerException.Message}> but we were expecting <{message}>");
                return this;
            }
            Execute.Assertion.ForCondition(false)
                .FailWith($"No exception thrown while creating <{nameof(TItem)}>");
            return this;
        }
    }
}