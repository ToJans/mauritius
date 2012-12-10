using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Example.Specs
{
    [TestClass]
    public class UniquenessProviderSpecs
    {
        const string some_category = "Some category";
        const string another_category = "Another category";
        const string some_value = "some value";
        const string another_value = "another value";

        Contracts.Uniqueness.IEvents Given;
        UniquenessImpl When;
        Mock<Contracts.Uniqueness.IEvents> Then;

        [TestInitialize]
        public void Init()
        {
            When = new Example.UniquenessImpl();
            Given = When.Modifier;
            Then = new Mock<Contracts.Uniqueness.IEvents>();
            When.Modifier = Then.Object;
        }

        [TestMethod]
        public void Adding_a_unique_value_to_a_category_for_the_first_time_should_succeed()
        {
            When.AddUniqueValue(some_category,some_value);

            Then.Verify(x => x.UniqueValueAdded(some_category, some_value));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException), "This value is not unique")]
        public void Adding_a_unique_value_to_the_same_category_twice_should_fail()
        {
            Given.UniqueValueAdded(some_category, some_value);

            When.AddUniqueValue(some_category, some_value);
        }

        [TestMethod]
        public void Adding_a_different_value_to_the_same_category_should_succeed()
        {
            Given.UniqueValueAdded(some_category, some_value);

            When.AddUniqueValue(some_category, another_value);

            Then.Verify(x=>x.UniqueValueAdded(some_category,another_value));
        }

        [TestMethod]
        public void Adding_a_the_same_value_to_different_category_should_succeed()
        {
            Given.UniqueValueAdded(some_category, some_value);

            When.AddUniqueValue(another_category, some_value);

            Then.Verify(x => x.UniqueValueAdded(another_category, some_value));
        }
    }
}
