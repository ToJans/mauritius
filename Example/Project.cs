using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Example
{
    namespace Contracts
    {
        namespace Projects
        {
            public interface IHandleCommands
            {
                void Register(string ProjectId, string Title, string MemberId);
            }

            public interface IHandleEvents
            {
                void Registered(string ProjectId, string Title, string MemberId);
            }

            public interface IQuery
            {
                bool ContainsProjectId(string ProjectId);
                bool ContainsTitle(string Title);
                bool IsAMember(string MemberId);
            }
        }
    }

    public class Project : Contracts.Projects.IHandleCommands
    {
        public Contracts.Projects.IHandleEvents Handle = null;
        public Contracts.Projects.IQuery Query = null;

        public Project(Contracts.Projects.IQuery Query, Contracts.Projects.IHandleEvents Handle)
        {
            this.Query = Query;
            this.Handle = Handle;
        }

        public void Register(string ProjectId, string Title, string MemberId)
        {
            Guard.Against(Query.ContainsProjectId(ProjectId),"Duplicate project id");
            Guard.Against(Query.ContainsTitle(Title),"Duplicate project title");
            Guard.That(Query.IsAMember(MemberId), "Unknown member");
            Handle.Registered(ProjectId, Title, MemberId);
        }
    }

    public class ProjectState
    {
        public static readonly string IdIndexName = "Project ids";
        public static readonly string TitleIndexName = "Project title";
    }

    public class ProjectQueries : Contracts.Projects.IQuery
    {
        Contracts.Index.IQuery qIndex;

        public ProjectQueries(Contracts.Index.IQuery qIndex)
        {
            this.qIndex = qIndex;
        }

        public bool ContainsProjectId(string ProjectId)
        {
            return qIndex.ContainsValue(ProjectState.IdIndexName, ProjectId);
        }

        public bool ContainsTitle(string Title)
        {
            return qIndex.ContainsValue(ProjectState.TitleIndexName, Title);
        }


        public bool IsAMember(string MemberId)
        {
            return qIndex.ContainsValue(MembershipState.IndexName, MemberId);
        }
    }

    public class ProjectEventhandlers : Contracts.Projects.IHandleEvents
    {
        IndexEventhandlers hIndex;
        ProjectState state;

        public ProjectEventhandlers(ProjectState state, IndexEventhandlers hIndex)
        {
            this.state = state;
            this.hIndex = hIndex;
        }

        public void Registered(string ProjectId, string Title, string MemberId)
        {
            hIndex.ValueAdded(ProjectState.IdIndexName, ProjectId);
            hIndex.ValueAdded(ProjectState.TitleIndexName, Title);
        }
    }
}
