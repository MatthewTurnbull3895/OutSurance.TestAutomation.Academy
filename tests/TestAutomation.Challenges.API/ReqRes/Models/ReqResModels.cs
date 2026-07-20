using System.Text.Json.Serialization;

namespace TestAutomation.Challenges.API.ReqRes.Models;

// -------------------------------------------------------------------------
// User list
// -------------------------------------------------------------------------

public sealed class UserListResponse
{
    [JsonPropertyName("page")]       public int Page       { get; set; }
    [JsonPropertyName("per_page")]   public int PerPage    { get; set; }
    [JsonPropertyName("total")]      public int Total      { get; set; }
    [JsonPropertyName("total_pages")]public int TotalPages { get; set; }
    [JsonPropertyName("data")]       public List<UserDto> Data { get; set; } = [];
}

public sealed class SingleUserResponse
{
    [JsonPropertyName("data")] public UserDto Data { get; set; } = null!;
}

public sealed class UserDto
{
    [JsonPropertyName("id")]         public int    Id        { get; set; }
    [JsonPropertyName("email")]      public string Email     { get; set; } = string.Empty;
    [JsonPropertyName("first_name")] public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("last_name")]  public string LastName  { get; set; } = string.Empty;
}

// -------------------------------------------------------------------------
// Create / update
// -------------------------------------------------------------------------

public sealed class CreateUserResponse
{
    [JsonPropertyName("name")]      public string Name      { get; set; } = string.Empty;
    [JsonPropertyName("job")]       public string Job       { get; set; } = string.Empty;
    [JsonPropertyName("id")]        public string Id        { get; set; } = string.Empty;
    [JsonPropertyName("createdAt")] public string CreatedAt { get; set; } = string.Empty;
}

public sealed class UpdateUserResponse
{
    [JsonPropertyName("name")]      public string Name      { get; set; } = string.Empty;
    [JsonPropertyName("job")]       public string Job       { get; set; } = string.Empty;
    [JsonPropertyName("updatedAt")] public string UpdatedAt { get; set; } = string.Empty;
}

// -------------------------------------------------------------------------
// Auth
// -------------------------------------------------------------------------

public sealed class TokenResponse
{
    [JsonPropertyName("token")] public string Token { get; set; } = string.Empty;
}

public sealed class RegisterResponse
{
    [JsonPropertyName("id")]    public int    Id    { get; set; }
    [JsonPropertyName("token")] public string Token { get; set; } = string.Empty;
}

public sealed class ErrorResponse
{
    [JsonPropertyName("error")] public string Error { get; set; } = string.Empty;
}
