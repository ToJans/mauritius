
namespace Example
{
    namespace Contracts
    {
        namespace Membership
        {
            public interface ICommands
            {
                void RegisterMember(string MemberId);
            }

            public interface IEvents
            {
                void MemberRegistered(string MemberId);
            }

            public interface IState
            {
                bool IsExistingMember(string MemberId);
            }
        }
    }

    public class Membership : Contracts.Membership.ICommands
    {
        MembershipState state = null;
        public Contracts.Membership.IEvents Modifier = null;

        public Membership()
        {
            state = new MembershipState();
            Modifier = new MembershipStateModifier(state);
        }

        public void RegisterMember(string MemberId)
        {
            Guard.AgainstNullOrWhitespace(MemberId, "UserId has to contain at least one non-whitespace character");
            Guard.Against(state.IsExistingMember(MemberId), "Duplicate userid");
            Modifier.MemberRegistered(MemberId);
        }
    }

    public class MembershipState : Contracts.Membership.IState
    {
        public const string UNIQUENESS_KEY = "members";

        public UniquenessState UniquenessState;

        public MembershipState()
        {
            UniquenessState = new UniquenessState();
        }

        public bool IsExistingMember(string MemberId)
        {
            return UniquenessState.HasUniqueValue(UNIQUENESS_KEY, MemberId);
        }
    }

    public class MembershipStateModifier : Contracts.Membership.IEvents
    {
        UniquenessModifier UniquenessModifier=null;
        MembershipState state = null;

        public MembershipStateModifier(MembershipState state)
        {
            this.state = state;
            this.UniquenessModifier = new UniquenessModifier(state.UniquenessState);
        }

        public void MemberRegistered(string MemberId)
        {
            UniquenessModifier.UniqueValueAdded(MembershipState.UNIQUENESS_KEY, MemberId);
        }
    }
}
