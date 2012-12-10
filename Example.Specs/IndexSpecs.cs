using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Example.Specs
{
    [TestClass]
    public class IndexSpecs
    {
        const string an_index_name = "Some index";
        const string another_index_name = "Another index";
        const string some_value = "some value";
        const string another_value = "another value";

        Contracts.Index.IHandleEvents GivenIndex;
        Index WhenIndex;
        Mock<Contracts.Index.IHandleEvents> ThenIndex;

        [TestInitialize]
        public void Init()
        {
            var sIndex = new IndexState();
            var qIndex = new IndexQueries(sIndex);
            var hIndex = new IndexEventhandlers(sIndex);

            WhenIndex = new Example.Index(qIndex,hIndex);
            GivenIndex = WhenIndex.Handle;
            ThenIndex = new Mock<Contracts.Index.IHandleEvents>();
            WhenIndex.Handle = ThenIndex.Object;
        }

        [TestMethod]
        public void Adding_a_unique_value_to_an_index_for_the_first_time_should_succeed()
        {
            WhenIndex.AddUniqueValue(an_index_name,some_value);

            ThenIndex.Verify(x => x.ValueAdded(an_index_name, some_value));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Adding_a_unique_value_to_the_same_index_twice_should_fail()
        {
            GivenIndex.ValueAdded(an_index_name, some_value);

            WhenIndex.AddUniqueValue(an_index_name, some_value);
        }

        [TestMethod]
        public void Adding_a_different_value_to_the_same_index_should_succeed()
        {
            GivenIndex.ValueAdded(an_index_name, some_value);

            WhenIndex.AddUniqueValue(an_index_name, another_value);

            ThenIndex.Verify(x=>x.ValueAdded(an_index_name,another_value));
        }

        [TestMethod]
        public void Adding_a_the_same_value_to_different_index_should_succeed()
        {
            GivenIndex.ValueAdded(an_index_name, some_value);

            WhenIndex.AddUniqueValue(another_index_name, some_value);

            ThenIndex.Verify(x => x.ValueAdded(another_index_name, some_value));
        }
    }
}
