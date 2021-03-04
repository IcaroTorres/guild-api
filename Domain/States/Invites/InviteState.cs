﻿using Domain.Common;
using Domain.Enums;
using Domain.Models;

namespace Domain.States.Invites
{
    public abstract class InviteState
    {
        protected InviteState(Invite context)
        {
            Context = context;
        }

        internal Invite Context { get; set; }
        internal InviteStatuses Status { get; set; }
        internal abstract Membership BeAccepted(IModelFactory factory);
        internal abstract Invite BeDenied();
        internal abstract Invite BeCanceled();

        internal static InviteState NewState(Invite invite, Guild guild, Member member, InviteStatuses status)
        {
            if (guild is INullObject || member is INullObject) return new ClosedInviteState(invite, status);

            return status == InviteStatuses.Pending
                ? new OpenInviteState(invite) : new ClosedInviteState(invite, status) as InviteState;
        }
    }
}
