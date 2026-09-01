using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.CreateRole;

public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<CreateRoleResponse>>
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async ValueTask<Result<CreateRoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var nameEn = request.NameEn.Trim();

        var exists = await _roleRepository.NameExistsAsync(nameEn, null, cancellationToken);

        if (exists)
        {
            return Result<CreateRoleResponse>.Failure("role_name_exists", $"A role named '{nameEn}' already exists");
        }

        var validKeys = Permissions.All.Select(p => p.Key).ToHashSet();
        var invalid = request.Permissions.Where(p => !validKeys.Contains(p)).ToList();

        if (invalid.Count > 0)
        {
            return Result<CreateRoleResponse>.Failure(
                "invalid_permissions",
                $"Unknown permission keys: {string.Join(", ", invalid)}");
        }

        var role = Role.Create(nameEn, request.NameAr, request.Description);
        role.SetPermissions(request.Permissions);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateRoleResponse>.Success(
            new CreateRoleResponse(role.Id, role.NameEn, role.NameAr, role.Permissions.Count));
    }
}