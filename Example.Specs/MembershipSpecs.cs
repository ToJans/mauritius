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
        public const string MemberId = "members/1";

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
        public void Registering_a_new_user_should_succeed()
        {
            When.RegisterMember(MemberId);

            Then.Verify(x => x.MemberRegistered(MemberId));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException), "Duplicate memberid")]
        public void Registering_a_user_twice_should_fail()
        {
            Given.MemberRegistered(MemberId);

            When.RegisterMember(MemberId);
        }

    }
}
