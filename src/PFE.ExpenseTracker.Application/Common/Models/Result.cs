namespace PFE.ExpenseTracker.Application.Common.Models
{
    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public T Data { get; set; }
        public string[] Errors { get; set; }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static Result<T> Failure(params string[] errors)
        {
            return new Result<T> { Succeeded = false, Errors = errors };
        }
    }

    public class Result : Result<object>
    {
        public static Result Success()
        {
            return new Result { Succeeded = true };
        }

        public new static Result Failure(params string[] errors)
        {
            return new Result { Succeeded = false, Errors = errors };
        }
    }
}
