﻿using Application.Common.Abstractions;
using Application.Common.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Members.Commands.LeaveGuild
{
    public class LeaveGuildHandler : IRequestHandler<LeaveGuildCommand, IApiResult>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipRepository _membershipRepository;

        public LeaveGuildHandler(IMemberRepository memberRepository, IMembershipRepository membershipRepository)
        {
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<IApiResult> Handle(LeaveGuildCommand command, CancellationToken cancellationToken)
        {
            var leavingMember = await _memberRepository.GetForGuildOperationsAsync(command.Id, cancellationToken);
            var promotedNewLeader = leavingMember.Guild.GetVice();
            leavingMember.Guild.RemoveMember(leavingMember);

            leavingMember = _memberRepository.Update(leavingMember);
            _memberRepository.Update(promotedNewLeader);
            _membershipRepository.Update(leavingMember.GetLastFinishedMembership());

            return new SuccessResult(leavingMember);
        }
    }
}