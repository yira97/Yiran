namespace Evrane.Core.Exceptions;

public enum EvraneStatusCode
{
    BadRequest = 4000,
    UserNotExist = 4010,
    UserAlreadyExist,
    EmailNotExist = 4020,
    EmailAlreadyExist,
    PasswordNotMatch = 4030,
    PasswordInvalid,
    InvalidToken = 4040,
    TokenExpired,
    RefreshTokenExpired,
    NoToken,
    HashNotEqual = 4050,
    ExceedSizeLimit = 4060,
    UnknownEnum = 4070,
    ResourceNotFound = 4080,
    IdNotFound = 4081,
    InvalidFileName = 4090,
    InvalidOperation = 4100,
    RequiredFieldsAreEmpty = 4110,
    FieldLengthTooShort = 4111,
    FieldLengthTooLong = 4112,
    ServerError = 5000,
    ConfigError = 5010,
    AuthFailed = 5020,
}

public class EvraneException : ApplicationException
{
    public EvraneException(EvraneStatusCode code, string? message, string? publicMessage = null) : base(message)
    {
        this.Code = code;
        this.PublicMessage = publicMessage;
    }

    public EvraneStatusCode Code { get; set; }
    public string? PublicMessage { get; set; }
}