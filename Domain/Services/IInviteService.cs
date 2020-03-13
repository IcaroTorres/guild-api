﻿using Domain.Entities;
using Domain.DTOs;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    public interface IInviteService
    {
        IInvite GetInvite(Guid id);
        IReadOnlyList<IInvite> List(InviteDto payload);
        IInvite InviteMember(InviteDto payload);
        IInvite Accept(Guid id);
        IInvite Decline(Guid id);
        IInvite Cancel(Guid id);
        IInvite Delete(Guid id);
    }
}