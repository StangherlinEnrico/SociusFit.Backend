using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.Register;

/// <summary>
/// Command to register a new user
/// </summary>
public record RegisterUserCommand : IRequest<Result<AuthResponseDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
