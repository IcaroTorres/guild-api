using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Responses
{
    [Serializable, ExcludeFromCodeCoverage]
    public class GuildResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public GuildMemberResponse Leader { get; set; }
        public List<GuildMemberResponse> Members { get; set; } = new List<GuildMemberResponse>();
        public int MembersCount => Members.Count;
    }

    [Serializable, ExcludeFromCodeCoverage]
    public class GuildMemberResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsGuildLeader { get; set; }
    }
}