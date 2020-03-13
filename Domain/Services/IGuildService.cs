using Domain.Entities;
using Domain.DTOs;
using System;
using System.Collections.Generic;
using Domain.Validations;

namespace Domain.Services
{
    public interface IGuildService
    {
        IGuild GetGuild(Guid id);
        IGuild Create(GuildDto payload);
        IGuild Update(GuildDto payload, Guid id);
        IGuild Delete(Guid id);
        IReadOnlyList<IGuild> List(int count = 20);
    }
}