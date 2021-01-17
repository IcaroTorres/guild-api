﻿using Domain.Repositories;
using FluentValidation;
using System;
using System.Net;

namespace Business.Usecases.Members.ChangeMemberName
{
    public class ChangeMemberNameValidator : AbstractValidator<ChangeMemberNameCommand>
    {
        public ChangeMemberNameValidator(IMemberRepository memberRepository)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();

            When(x => x.Id != Guid.Empty && x.Name.Length != 0, () =>
            {
                RuleFor(x => x)
                    .MustAsync((x, ct) => memberRepository.ExistsWithIdAsync(x.Id, ct))
                    .WithMessage(x => $"Record not found for member with given id {x.Id}.")
                    .WithName(x => nameof(x.Id))
                    .WithErrorCode(nameof(HttpStatusCode.NotFound))

                    .MustAsync((x, ct) => memberRepository.CanChangeNameAsync(x.Id, x.Name, ct))
                    .WithMessage(x => $"Record already exists for member with given name {x.Name}.")
                    .WithName(x => nameof(x.Name))
                    .WithErrorCode(nameof(HttpStatusCode.Conflict));
            });
        }
    }
}