
namespace Example
{
    namespace Contracts
    {
        namespace Membership
        {
            public interface IHandleCommands
            {
                void RegisterMember(string MemberId);
            }

            public interface IHandleEvents
            {
                void MemberRegistered(string MemberId);
            }

            public interface IQuery
            {
                bool IsExistingMember(string MemberId);
            }
        }
    }

    public class Membership : Contracts.Membership.IHandleCommands
    {
        public Contracts.Membership.IQuery Query = null;
        public Contracts.Membership.IHandleEvents Handle = null;

        public Membership(Contracts.Membership.IQuery Queries,Contracts.Membership.IHandleEvents EventHandlers)
        {
            this.Handle = EventHandlers;
            this.Query = Queries;
        }

        public void RegisterMember(string MemberId)
        {
            Guard.AgainstNullOrWhitespace(MemberId, "The member id has to contain at least one non-whitespace character");
            Guard.Against(Query.IsExistingMember(MemberId), "This member id has already been registered");
            Handle.MemberRegistered(MemberId);
        }
    }

    public class MembershipState
    {
        public static readonly string IndexName = "members";
    }

    public class MembershipQueries : Contracts.Membership.IQuery
    {
        Contracts.Index.IQuery qIndex = null;

        public MembershipQueries(Contracts.Index.IQuery qIndex)
        {
            this.qIndex = qIndex;
        }

        public bool IsExistingMember(string MemberId)
        {
            return qIndex.ContainsValue(MembershipState.IndexName, MemberId);
        }
    }

    public class MembershipEventhandlers : Contracts.Membership.IHandleEvents
    {
        Contracts.Index.IHandleEvents hIndex=null;

        public MembershipEventhandlers(Contracts.Index.IHandleEvents hIndex)
        {
            this.hIndex = hIndex;
        }

        public void MemberRegistered(string MemberId)
        {
            hIndex.ValueAdded(MembershipState.IndexName, MemberId);
        }
    }
}
