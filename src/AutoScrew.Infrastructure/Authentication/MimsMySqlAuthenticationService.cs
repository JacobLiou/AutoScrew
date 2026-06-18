using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace AutoScrew.Infrastructure.Authentication;

/// <summary>
/// 只读查询 MIMS MySQL（<c>mims_person</c> + <c>mims_role</c>），不执行任何写操作。
/// </summary>
public sealed class MimsMySqlAuthenticationService(
    IOptions<MimsAuthenticationOptions> options,
    ILogger<MimsMySqlAuthenticationService> logger) : IUserAuthenticationService, IMimsAuthenticationService
{
    private const string SelectSql = """
        SELECT p.id AS PersonId, p.login_name AS LoginName, p.name AS DisplayName,
               r.id AS RoleId, r.type AS RoleType, r.name AS RoleName
        FROM mims_person p
        INNER JOIN mims_role r ON r.id = p.role_id
        WHERE LOWER(TRIM(p.login_name)) = LOWER(@loginName) AND p.password = @passwordHash
        LIMIT 1
        """;

    public Task<MimsSignInOutcome> SignInAsync(string userName, string password, CancellationToken cancellationToken = default) =>
        SignInInternalAsync(userName, password, cancellationToken);

    async Task<LoginResult> IUserAuthenticationService.SignInAsync(
        string userName,
        string password,
        CancellationToken cancellationToken)
    {
        var outcome = await SignInInternalAsync(userName, password, cancellationToken).ConfigureAwait(false);
        return outcome.Result ?? LoginResult.Failed(outcome.ErrorMessage ?? "登录失败。");
    }

    private async Task<MimsSignInOutcome> SignInInternalAsync(
        string userName,
        string password,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userName) || password is null)
            return MimsSignInOutcome.Failed(MimsSignInFailureKind.InvalidInput, "请输入用户名和密码。");

        var opt = options.Value;
        if (string.IsNullOrWhiteSpace(opt.ConnectionString))
        {
            return MimsSignInOutcome.Failed(
                MimsSignInFailureKind.NotConfigured,
                "未配置 MIMS 数据库连接（Authentication:Mims:ConnectionString 或 ConnectionStringDpapiBase64）。");
        }

        var hash = MimsPasswordHasher.Hash(password);

        await using var conn = new MySqlConnection(opt.ConnectionString);

        try
        {
            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "MySQL connection failed for login.");
            return MimsSignInOutcome.Failed(
                MimsSignInFailureKind.ConnectionFailed,
                "无法连接用户数据库，请稍后重试或联系管理员。");
        }

        await using var cmd = new MySqlCommand(SelectSql, conn);
        cmd.CommandTimeout = (int)Math.Clamp(opt.CommandTimeoutSeconds, 1, 120);
        cmd.Parameters.AddWithValue("@loginName", userName.Trim());
        cmd.Parameters.AddWithValue("@passwordHash", hash);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            logger.LogWarning("MIMS login failed for user {User}", userName);
            return MimsSignInOutcome.Failed(MimsSignInFailureKind.InvalidCredentials, "用户名或密码错误。");
        }

        var personId = reader.GetInt32(reader.GetOrdinal("PersonId"));
        var loginName = reader.GetString(reader.GetOrdinal("LoginName"));
        var displayName = reader.IsDBNull(reader.GetOrdinal("DisplayName"))
            ? loginName
            : reader.GetString(reader.GetOrdinal("DisplayName"));
        var roleId = reader.GetInt32(reader.GetOrdinal("RoleId"));
        var roleType = reader.GetInt32(reader.GetOrdinal("RoleType"));
        var roleName = reader.IsDBNull(reader.GetOrdinal("RoleName"))
            ? null
            : reader.GetString(reader.GetOrdinal("RoleName"));

        var userRole = MimsRoleMapper.ToAutoScrewRole(roleName);

        logger.LogInformation(
            "MIMS user {Login} signed in (personId={PersonId}, roleId={RoleId}, roleName={RoleName}, autoScrewRole={AutoScrewRole}).",
            loginName,
            personId,
            roleId,
            roleName,
            userRole);

        return MimsSignInOutcome.Succeeded(
            LoginResult.Ok(loginName, displayName, userRole, personId, roleId, roleType));
    }
}
