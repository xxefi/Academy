﻿namespace ApiFirst.Exceptions;

public enum AuthErrorTypes
{
    InvalidToken,
    InvalidRefreshToken,
    InvalidCredentials,
    UserNotFound,
    InvalidRequest,
    PasswordMismatch,
}
