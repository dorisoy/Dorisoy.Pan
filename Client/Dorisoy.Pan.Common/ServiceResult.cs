using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.Common;

public class ServiceResult
{
    public bool Succeeded { get; } = false;

    public IEnumerable<ServiceError> Errors { get; } = new ServiceError[0];

    public ServiceResult()
    {
        Succeeded = true;
    }

    public ServiceResult(string errorMessage)
    {
        Errors = new[] { new ServiceError(errorMessage) };
    }

    public ServiceResult(Exception ex)
    {
        Errors = new[] { new ServiceError(ex) };
    }

    public ServiceResult(string errorMessage, Exception ex)
    {
        Errors = new[] { new ServiceError(errorMessage, ex) };
    }

    public ServiceResult(params ServiceError[] errors)
    {
        Errors = errors;
    }

    public static ServiceResult Ok()
    {
        return new ServiceResult();
    }

    public static ServiceResult Fail(string errorMessage) => new ServiceResult(errorMessage);

    public static ServiceResult Fail(Exception ex) => new ServiceResult(ex);

    public static ServiceResult Fail(string errorMessage, Exception ex) => new ServiceResult(errorMessage, ex);

    public static ServiceResult Fail(params ServiceError[] errors) => new ServiceResult(errors);

    public static ServiceResult<T> Ok<T>(T data)
    {
        return new ServiceResult<T>(data);
    }


    public static ServiceResult<T> Fail<T>(string errorMessage)
    {
        return new ServiceResult<T>(errorMessage);
    }
}
