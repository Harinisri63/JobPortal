using Microsoft.Data.SqlClient;
using System.Data;
using JobPortal.Shared.Interfaces;
using JobPortal.Features.User;
using JobPortal.Database;
using JobPortal.Features.User.Interface;

namespace JobPortal.Features.User.Repository;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _db;

    public UserRepository(DatabaseConnection db)
    {
        _db = db;
    }
    public async Task<User1?> GetByIdAsync(int id)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand("JPNS.usp_GetUserById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@UserId", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return MapUser(reader);

        return null;
    }

    public async Task<IEnumerable<User1>> GetAllAsync()
    {
        var users = new List<User1>();

        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "SELECT UserId, FullName, Email, Username, Role, IsActive, IsEmailVerified, " +
            "IsLocked, LockedUntil, FailedLoginAttempts, LastLoginAt, CreatedAt, UpdatedAt " +
            "FROM JPNS.Users ORDER BY CreatedAt DESC", connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            users.Add(MapUser(reader));

        return users;
    }

    public async Task<int> InsertAsync(User1 entity)
    {
        return await RegisterUserAsync(
            entity.FullName, entity.Email, entity.Username,
            entity.PasswordHash, entity.Salt, entity.MobileNumber, entity.Role);
    }

    public async Task<bool> UpdateAsync(User1 entity)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "UPDATE JPNS.Users SET FullName = @FullName, MobileNumber = @MobileNumber, " +
            "UpdatedAt = GETDATE() WHERE UserId = @UserId", connection);

        command.Parameters.AddWithValue("@UserId", entity.UserId);
        command.Parameters.AddWithValue("@FullName", entity.FullName);
        command.Parameters.AddWithValue("@MobileNumber", (object?)entity.MobileNumber ?? DBNull.Value);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "DELETE FROM JPNS.Users WHERE UserId = @UserId", connection);

        command.Parameters.AddWithValue("@UserId", id);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }


    public async Task<int> RegisterUserAsync(string fullName, string email, string username,string passwordHash, string salt,string? mobileNumber = null, string role = "jobseeker")
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand("JPNS.usp_RegisterUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@FullName", fullName);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
        command.Parameters.AddWithValue("@Salt", salt);
        command.Parameters.AddWithValue("@MobileNumber", (object?)mobileNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@Role", role);

        SqlParameter userIdParam = new SqlParameter("@UserId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(userIdParam);

        try
        {
            await command.ExecuteNonQueryAsync();
            return (int)userIdParam.Value;
        }
        catch (SqlException ex)
        {
            Console.WriteLine("SQL Error during RegisterUser:");
            foreach (SqlError error in ex.Errors)
                Console.WriteLine(error.Message);
            throw;
        }
    }

    public async Task<User1?> LoginUserAsync(string emailOrMobile, string passwordHash)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand("JPNS.usp_LoginUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@EmailOrMobile", emailOrMobile);
        command.Parameters.AddWithValue("@PasswordHash", passwordHash);

        try
        {
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User1
                {
                    UserId           = (int)reader["UserId"],
                    FullName         = reader["FullName"].ToString()!,
                    Role             = reader["Role"].ToString()!,
                    IsEmailVerified  = (bool)reader["IsEmailVerified"],
                    IsActive         = (bool)reader["IsActive"]
                };
            }

            return null;
        }
        catch (SqlException ex)
        {
            Console.WriteLine("SQL Error during LoginUser:");
            foreach (SqlError error in ex.Errors)
                Console.WriteLine(error.Message);
            throw;
        }
    }


    public async Task<User1?> GetByEmailAsync(string email)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "SELECT UserId, FullName, Email, Username, Role, IsActive, IsEmailVerified, " +
            "IsLocked, LockedUntil, FailedLoginAttempts, LastLoginAt, CreatedAt, UpdatedAt " +
            "FROM JPNS.Users WHERE Email = @Email", connection);

        command.Parameters.AddWithValue("@Email", email.ToLower());

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return MapUser(reader);

        return null;
    }

    public async Task<User1?> GetByUsernameAsync(string username)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "SELECT UserId, FullName, Email, Username, Role, IsActive, IsEmailVerified, " +
            "IsLocked, LockedUntil, FailedLoginAttempts, LastLoginAt, CreatedAt, UpdatedAt " +
            "FROM JPNS.Users WHERE Username = @Username", connection);

        command.Parameters.AddWithValue("@Username", username.ToLower());

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return MapUser(reader);

        return null;
    }

    public async Task<IEnumerable<User1>> SearchAsync(string keyword)
    {
        var users = new List<User1>();

        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "SELECT UserId, FullName, Email, Username, Role, IsActive, IsEmailVerified, " +
            "IsLocked, LockedUntil, FailedLoginAttempts, LastLoginAt, CreatedAt, UpdatedAt " +
            "FROM JPNS.Users " +
            "WHERE FullName LIKE @Keyword OR Email LIKE @Keyword OR Username LIKE @Keyword " +
            "ORDER BY FullName", connection);

        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            users.Add(MapUser(reader));

        return users;
    }


    public async Task<bool> SetActiveAsync(int userId, bool isActive)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand("JPNS.usp_SetUserActive", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@IsActive", isActive);

        int rows = await command.ExecuteNonQueryAsync();
        return rows >= 0;
    }

    public async Task<bool> SetEmailVerifiedAsync(int userId, bool verified)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "UPDATE JPNS.Users SET IsEmailVerified = @Verified, UpdatedAt = GETDATE() " +
            "WHERE UserId = @UserId", connection);

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@Verified", verified);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash, string newSalt)
    {
        await using var connection = await _db.OpenAsync();
        await using var command = new SqlCommand(
            "UPDATE JPNS.Users SET PasswordHash = @PasswordHash, Salt = @Salt, " +
            "UpdatedAt = GETDATE() WHERE UserId = @UserId", connection);

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
        command.Parameters.AddWithValue("@Salt", newSalt);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }


    private static User1 MapUser(SqlDataReader reader)
    {
        return new User1
        {
            UserId               = (int)reader["UserId"],
            FullName             = reader["FullName"].ToString()!,
            Email                = reader["Email"].ToString()!,
            Username             = reader["Username"].ToString()!,
            Role                 = reader["Role"].ToString()!,
            IsActive             = (bool)reader["IsActive"],
            IsEmailVerified      = (bool)reader["IsEmailVerified"],
            IsLocked             = reader["IsLocked"] != DBNull.Value && (bool)reader["IsLocked"],
            LockedUntil          = reader["LockedUntil"] == DBNull.Value ? null : (DateTime?)reader["LockedUntil"],
            FailedLoginAttempts  = reader["FailedLoginAttempts"] == DBNull.Value ? 0 : (int)reader["FailedLoginAttempts"],
            LastLoginAt          = reader["LastLoginAt"] == DBNull.Value ? null : (DateTime?)reader["LastLoginAt"],
            CreatedAt            = (DateTime)reader["CreatedAt"],
            UpdatedAt            = (DateTime)reader["UpdatedAt"]
        };
    }
}
