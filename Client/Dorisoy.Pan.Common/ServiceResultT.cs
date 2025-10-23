using System;

namespace Dorisoy.Pan.Common;

public class ServiceResult<T> : ServiceResult
{
    public T Data { get; } = default;

    public ServiceResult(T data) : base()
    {
        Data = data;
    }

    public ServiceResult(string errorMessage) : base(errorMessage)
    {

    }

    public ServiceResult(Exception ex) : base(ex)
    {

    }

    public ServiceResult(string errorMessage, Exception ex) : base(errorMessage, ex)
    {

    }

    public ServiceResult(params ServiceError[] errors) : base(errors)
    {

    }

    public static new ServiceResult<T> Fail(string errorMessage) => new ServiceResult<T>(errorMessage);

    public static new ServiceResult<T> Fail(Exception ex) => new ServiceResult<T>(ex);

    public static new ServiceResult<T> Fail(string errorMessage, Exception ex) => new ServiceResult<T>(errorMessage, ex);

    public static new ServiceResult<T> Fail(params ServiceError[] errors) => new ServiceResult<T>(errors);
}
