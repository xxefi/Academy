namespace Academy.Application.Exceptions;

public enum ExceptionType
{
    InvalidToken, 
    InvalidRefreshToken, 
    InvalidCredentials,
    UserNotFound, 
    NullCredentials,
    InvalidRequest,
    PasswordMismatch,
    EmailAlreadyConfirmed,
    EmailNotConfirmed, 
    EmailAlreadyExists,
    CredentialsAlreadyExists,
    NotFound,
    UnauthorizedAccess,
    Forbidden,
    BadRequest,
    Conflict,
    InternalServerError, 
    ServiceUnavailable,
    OperationFailed,
    DatabaseError,
    Critical,
}
