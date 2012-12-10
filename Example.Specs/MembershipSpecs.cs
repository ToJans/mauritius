using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Example.Specs
{
    [TestClass]
    public class MembershipSpecs
    {
        public const string a_member = "members/1";
        public const string another_member = "members/2";
        public const string yet_another_member = "members/3";

        Membership When;
        Contracts.Membership.IHandleEvents Given;
        Mock<Contracts.Membership.IHandleEvents> Then;

        [TestInitialize]
        public void Init()
        {
            var sIndex = new IndexState();
            var qIndex = new IndexQueries(sIndex);
            var hIndex = new IndexEventhandlers(sIndex);
            var qMembership = new MembershipQueries(qIndex);
            var hMembership = new MembershipEventhandlers(hIndex);
            When = new Membership(qMembership,hMembership);
            Given = When.Handle;
            Then = new Mock<Contracts.Membership.IHandleEvents>();
            When.Handle = Then.Object;
        }

        [TestMethod]
        public void Registering_a_new_member_should_succeed()
        {
            When.RegisterMember(a_member);

            Then.Verify(x => x.MemberRegistered(a_member));
        }

        [TestMethod]
        public void Registering_a_few_members_should_succeed()
        {
            Given.MemberRegistered(a_member);
            Given.MemberRegistered(another_member);

            When.RegisterMember(yet_another_member);
            
            Then.Verify(x => x.MemberRegistered(yet_another_member));
        }

        [TestMethod,ExpectedException(typeof(InvalidOperationException))]
        public void Registering_a_member_with_a_whitespace_id_should_fail()
        {
            When.RegisterMember("   \t");

        }


        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Registering_a_member_twice_should_fail()
        {
            Given.MemberRegistered(a_member);

            When.RegisterMember(a_member);
        }

    }
}
