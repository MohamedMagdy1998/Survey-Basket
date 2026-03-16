namespace SurveyBasketAPI.Result_Pattern;

public class Result
{
   

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

   

    protected Result(bool isSucces,Error error)
    {
        if ((isSucces && error != Error.None) || (!isSucces && error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSucces;
        Error = error;
        
    }

    public static Result Success() => new Result( true, Error.None);

    public static Result Failure(Error error) => new Result( false,error);

    public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value, true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default!, false, error);


}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("We don't have value for Failure Error."); 

    public Result(TValue value, bool isSucces, Error error) : base(isSucces,error) 
    {
        _value = value; 
    }

    // implicit cast from value -> Result<T>
    public static implicit operator Result<TValue>(TValue value)
    {
        return Success<TValue>(value);
    }

    // implicit cast from Error -> Result<T>
    public static implicit operator Result<TValue>(Error error)
    {
        return Failure<TValue>(error);
    }



}