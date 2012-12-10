using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
        Contracts.Membership.IEvents Given;
        Mock<Contracts.Membership.IEvents> Then;

        [TestInitialize]
        public void Init()
        {
            When = new Membership();
            Given = When.Modifier;
            Then = new Mock<Contracts.Membership.IEvents>();
            When.Modifier = Then.Object;
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

        [TestMethod, ExpectedException(typeof(InvalidOperationException), "Duplicate memberid")]
        public void Registering_a_member_twice_should_fail()
        {
            Given.MemberRegistered(a_member);

            When.RegisterMember(a_member);
        }

    }
}
