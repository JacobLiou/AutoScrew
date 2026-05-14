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
    ILogger<MimsMySqlAuthenticationService> logger) : IUserAuthenticationService
{
    private const string SelectSql = """
        SELECT p.id AS PersonId, p.login_name AS LoginName, p.name AS DisplayName,
               r.id AS RoleId, r.type AS RoleType, r.name AS RoleName
        FROM mims_person p
        INNER JOIN mims_role r ON r.id = p.role_id
        WHERE LOWER(TRIM(p.login_name)) = LOWER(@loginName) AND p.password = @passwordHash
        LIMIT 1
        """;

    public async Task<LoginResult> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName) || password is null)
            return LoginResult.Failed("请输入用户名和密码。");

        var opt = options.Value;
        if (string.IsNullOrWhiteSpace(opt.ConnectionString))
            return LoginResult.Failed("未配置 MIMS 数据库连接（Authentication:Mims:ConnectionString 或 ConnectionStringDpapiBase64）。");

        var hash = MimsPasswordHasher.Hash(password);

        await using var conn = new MySqlConnection(opt.ConnectionString);

        try
        {
            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "MySQL connection failed for login.");
            return LoginResult.Failed("无法连接用户数据库，请稍后重试或联系管理员。");
        }

        await using var cmd = new MySqlCommand(SelectSql, conn);
        cmd.CommandTimeout = (int)Math.Clamp(opt.CommandTimeoutSeconds, 1, 120);
        cmd.Parameters.AddWithValue("@loginName", userName.Trim());
        cmd.Parameters.AddWithValue("@passwordHash", hash);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            logger.LogWarning("MIMS login failed for user {User}", userName);
            return LoginResult.Failed("用户名或密码错误。");
        }

        var personId = reader.GetInt32(reader.GetOrdinal("PersonId"));
        var loginName = reader.GetString(reader.GetOrdinal("LoginName"));
        var displayName = reader.IsDBNull(reader.GetOrdinal("DisplayName"))
            ? loginName
            : reader.GetString(reader.GetOrdinal("DisplayName"));
        var roleId = reader.GetInt32(reader.GetOrdinal("RoleId"));
        var roleType = reader.GetInt32(reader.GetOrdinal("RoleType"));
        _ = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"));

        if (!TryMapRole(roleType, opt, out var userRole, out var failMessage))
            return LoginResult.Failed(failMessage ?? "角色映射失败。");

        logger.LogInformation("MIMS user {Login} signed in (personId={PersonId}, roleType={RoleType}).", loginName, personId, roleType);
        return LoginResult.Ok(loginName, displayName, userRole, personId, roleId, roleType);
    }

    private static bool TryMapRole(int roleType, MimsAuthenticationOptions opt, out UserRole userRole, out string? failMessage)
    {
        failMessage = null;
        userRole = UserRole.Operator;
        var key = roleType.ToString(System.Globalization.CultureInfo.InvariantCulture);

        if (opt.RoleMap.TryGetValue(key, out var roleName) && !string.IsNullOrWhiteSpace(roleName))
        {
            if (Enum.TryParse<UserRole>(roleName.Trim(), true, out var mapped))
            {
                userRole = mapped;
                return true;
            }

            failMessage = "角色配置无效（Authentication:Mims:RoleMap）。";
            return false;
        }

        if (string.Equals(opt.UnmappedRoleBehavior, "Deny", StringComparison.OrdinalIgnoreCase))
        {
            failMessage = "当前角色无权登录本系统。";
            return false;
        }

        userRole = UserRole.Operator;
        return true;
    }
}
