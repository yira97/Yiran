namespace Evrane.Core.Exceptions;

public enum EvraneStatusCode
{
    BadRequest = 40000,
    UserAlreadyExist,
    EmailAlreadyExist,
    PasswordNotMatch,
    HashNotEqual,
    ExceedSizeLimit,
    UnknownEnum,
    FieldLengthTooShort,
    FieldLengthTooLong,
    InvalidOperation,
    InvalidFileName,
    RequiredFieldsAreEmpty,
    Unauthorized = 40100,
    InvalidToken,
    TokenExpired,
    NoToken,
    Forbidden = 40300,
    NotFound = 40400,
    ResourceNotFound,
    IdNotFound,
    UserNotExist,
    EmailNotExist,
    MethodNotAllowed = 40500,
    ServerError = 50000,
    ConfigError,
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