using Domain.Models;
using Domain.Nulls;
using Domain.States.Members;
using FluentAssertions;
using Tests.Domain.Models.Fakes;
using Tests.Domain.Models.Proxies;
using Tests.Helpers;
using Xunit;

namespace Tests.Domain.States.Members
{
    [Trait("Domain", "Model-State")]
    public class GuildLeaderStateTests
    {
        [Fact]
        public void Constructor_Should_CreateWith_GivenStatus()
        {
            // arrange
            var leader = (MemberTestProxy)MemberFake.GuildLeader().Generate();

            // act
            var sut = new GuildLeaderState(leader, leader.GetGuild());

            // assert
            sut.Guild.Should().BeOfType<GuildTestProxy>().And.Be(leader.GetGuild());
            sut.IsGuildLeader.Should().BeTrue().And.Be(leader.IsGuildLeader);
            sut.Guild.Members.Should().Contain(leader);
        }

        [Fact]
        public void Join_Should_Change_Guild_And_Memberships()
        {
            // arrange
            var member = (MemberTestProxy)MemberFake.GuildMember().Generate();
            var guild = (GuildTestProxy)GuildFake.Complete().Generate();
            var monitor = member.Monitor();
            var sut = member.GetState();
            var factory = TestModelFactoryHelper.Factory;

            // act
            sut.Join(guild, factory);

            // assert
            sut.Guild.Should().NotBeNull().And.BeOfType<GuildTestProxy>().And.NotBe(member.GetGuild());
            sut.IsGuildLeader.Should().BeFalse().And.Be(member.IsGuildLeader);
            sut.Guild.Members.Should().Contain(member);
            guild.Members.Should().Contain(member);
            member.GuildId.Should().NotBeNull();

            monitor.AssertCollectionChanged(member.Memberships);
            monitor.AssertPropertyChanged(
                nameof(Guild),
                nameof(Member.GuildId));

            monitor.AssertPropertyNotChanged(
                nameof(Member.Id),
                nameof(Member.Name),
                nameof(Member.IsGuildLeader));
        }

        [Fact]
        public void BePromoted_Should_Change_IsGuildLeader()
        {
            // arrange
            var leader = (MemberTestProxy)MemberFake.GuildLeader().Generate();
            var monitor = leader.Monitor();
            var sut = leader.GetState();

            // act
            sut.BePromoted();

            // assert
            sut.Guild.Should().NotBeNull().And.BeOfType<GuildTestProxy>().And.Be(leader.GetGuild());
            sut.Guild.Members.Should().Contain(leader);
            sut.IsGuildLeader.Should().BeTrue().And.Be(leader.IsGuildLeader);
            leader.GuildId.Should().NotBeNull();

            monitor.AssertPropertyNotChanged(
                nameof(Member.Id),
                nameof(Member.Name),
                nameof(Guild),
                nameof(Member.GuildId),
                nameof(Member.IsGuildLeader),
                nameof(Member.Memberships));
        }

        [Fact]
        public void BeDemoted_Should_Change_Nothing()
        {
            // arrange
            var leader = (MemberTestProxy)MemberFake.GuildLeader().Generate();
            var monitor = leader.Monitor();
            var sut = leader.GetState();

            // act
            sut.BeDemoted();

            // assert
            sut.Guild.Should().NotBeNull().And.BeOfType<GuildTestProxy>().And.Be(leader.GetGuild());
            sut.IsGuildLeader.Should().BeTrue().And.Be(!leader.IsGuildLeader);
            sut.Guild.Members.Should().Contain(leader);

            monitor.AssertPropertyChanged(nameof(Member.IsGuildLeader));
            monitor.AssertPropertyNotChanged(
                nameof(Member.Id),
                nameof(Member.Name),
                nameof(Guild),
                nameof(Member.GuildId),
                nameof(Member.Memberships));
        }

        [Fact]
        public void Leave_Should_Change_Guild()
        {
            // arrange
            var leader = (MemberTestProxy)MemberFake.GuildLeader().Generate();
            var monitor = leader.Monitor();
            var membership = (MembershipTestProxy)leader.GetActiveMembership();
            var membershipMonitor = membership.Monitor();
            var sut = leader.GetState();

            // act
            sut.Leave();

            // assert
            sut.Guild.Should().BeOfType<GuildTestProxy>().And.NotBe(leader.GetGuild());
            sut.Guild.Members.Should().Contain(leader);
            sut.IsGuildLeader.Should().BeTrue().And.Be(!leader.IsGuildLeader);
            leader.GetGuild().Should().BeOfType<NullGuild>();
            leader.GuildId.Should().BeNull();
            leader.Memberships.Should().Contain(membership);

            monitor.AssertPropertyChanged(
                nameof(Guild),
                nameof(Member.GuildId),
                nameof(Member.IsGuildLeader));

            monitor.AssertPropertyNotChanged(
                nameof(Member.Id),
                nameof(Member.Name));
            monitor.AssertCollectionNotChanged(leader.Memberships);

            membershipMonitor.AssertPropertyChanged(nameof(Membership.ModifiedDate));
        }
    }
}