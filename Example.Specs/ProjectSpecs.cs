using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Example.Specs
{
    [TestClass]
    public class ProjectSpecs
    {
        static readonly string a_project_id = "projects/1";
        static readonly string another_project_id = "projects/2";
        static readonly string a_project_title = "Some project";
        static readonly string another_project_title = "Another project";
        static readonly string a_member_id = "members/1";

        Project WhenProject;
        Contracts.Projects.IHandleEvents GivenProject;
        Contracts.Membership.IHandleEvents GivenMembership;
        Mock<Contracts.Projects.IHandleEvents> ThenProject;

        [TestInitialize]
        public void Init()
        {
            var sIndex = new IndexState();
            var qIndex = new IndexQueries(sIndex);
            var hIndex = new IndexEventhandlers(sIndex);

            GivenMembership = new MembershipEventhandlers(hIndex);

            var sProject = new ProjectState();
            var qProject = new ProjectQueries(qIndex);
            var hProject = new ProjectEventhandlers(sProject, hIndex);

            WhenProject = new Project(qProject, hProject);
            GivenProject = WhenProject.Handle;
            ThenProject = new Mock<Contracts.Projects.IHandleEvents>();
            WhenProject.Handle = ThenProject.Object;
        }

        [TestMethod]
        public void Registering_a_project_should_succeed()
        {
            GivenMembership.MemberRegistered(a_member_id);
            WhenProject.Register(a_project_id, a_project_title, a_member_id);

            ThenProject.Verify(x => x.Registered(a_project_id, a_project_title, a_member_id));
        }

        [TestMethod,ExpectedException(typeof(InvalidOperationException))]
        public void Registering_a_project_with_an_unknown_member_should_fail()
        {
            WhenProject.Register(a_project_id, a_project_title, a_member_id);

            ThenProject.Verify(x => x.Registered(a_project_id, a_project_title, a_member_id));
        }

        [TestMethod]
        public void Registering_two_projects_should_succeed()
        {
            GivenMembership.MemberRegistered(a_member_id);
            GivenProject.Registered(a_project_id, a_project_title, a_member_id);
            WhenProject.Register(another_project_id, another_project_title, a_member_id);

            ThenProject.Verify(x => x.Registered(another_project_id, another_project_title, a_member_id));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Registering_a_project_with_the_same_id_should_fail()
        {
            GivenMembership.MemberRegistered(a_member_id);
            GivenProject.Registered(a_project_id, a_project_title, a_member_id);

            WhenProject.Register(a_project_id, another_project_title, a_member_id);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Registering_a_project_with_the_same_title_should_fail()
        {
            GivenMembership.MemberRegistered(a_member_id);
            GivenProject.Registered(a_project_id, a_project_title, a_member_id);

            WhenProject.Register(another_project_id, a_project_title, a_member_id);
        }
    }
}
