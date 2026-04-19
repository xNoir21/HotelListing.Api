namespace HotelListing.Api.Results;

public readonly record struct Error(string Code, string Description)
{
    public static readonly Error None = new("", "");
    public bool IsNone => string.IsNullOrWhiteSpace(Code);
}

public readonly record struct Result
{
    public bool IsSuccess { get; }
    public Error[] Errors { get; }

    private Result(bool isSuccess, Error[] errors)
    {
        (IsSuccess, Errors) = (isSuccess, errors);
    }

    public static Result Success()
    {
        return new Result(true, Array.Empty<Error>());
    }

    public static Result Failure(params Error[] errors)
    {
        return new Result(false, errors);
    }

    public static Result NotFound(params Error[] errors)
    {
        return new Result(false, errors);
    }
    
    public static Result BadRequest(params Error[] errors)
    {
        return new Result(false, errors);
    }

    public static Result Combine(params Result[] results)
    {
        return results.Any(result => !result.IsSuccess)
            ? Failure(results.Where(result => !result.IsSuccess).SelectMany(result => result.Errors).ToArray())
            : Success();
    }
}

public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Error[] Errors { get; }

    private Result(bool isSuccess, T value, Error[] errors)
    {
        (IsSuccess, Value, Errors) = (isSuccess, value, errors);
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, Array.Empty<Error>());
    }

    public static Result<T> Failure(params Error[] errors)
    {
        return new Result<T>(false, default!, errors);
    }

    public static Result<T> NotFound(params Error[] errors)
    {
        return new Result<T>(false, default!, errors);
    }
    
    public static Result<T> BadRequest(params Error[] errors)
    {
        return new Result<T>(false, default!, errors);
    }

    // Functional Helpers
    public Result<K> Map<K>(Func<T, K> map)
    {
        return IsSuccess ? Result<K>.Success(map(Value!)) : Result<K>.Failure(Errors);
    }

    public Result<K> Bind<K>(Func<T, Result<K>> next)
    {
        return IsSuccess ? next(Value!) : Result<K>.Failure(Errors);
    }

    public Result<T> Ensure(Func<T, bool> predicate, Error error)
    {
        return IsSuccess && !predicate(Value!) ? Failure(error) : this;
    }
}