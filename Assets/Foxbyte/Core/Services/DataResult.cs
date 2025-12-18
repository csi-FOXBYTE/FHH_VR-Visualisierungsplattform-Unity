namespace Foxbyte.Core
{
    /// <summary>
    /// Represents a result of an operation that returns data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct DataResult<T>
    {
        public bool Success { get; }
        public long StatusCode { get; }
        public string Title { get; }
        public string Description { get; }
        public string Error { get; }
        public T Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult{T}"/> struct.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="statusCode"></param>
        /// <param name="data"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="error"></param>
        public DataResult(bool success, long statusCode, T data = default, string title = null, string description = null, string error = null)
        {
            Success = success;
            StatusCode = statusCode;
            Data = data;
            Title = title;
            Description = description;
            Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult{T}"/> struct with default status code.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="data"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="error"></param>
        public DataResult(bool success, T data = default, string title = null, string description = null, string error = null)
            : this(success, 0, data, title, description, error)
        {
        }
    }

    /// <summary>
    /// Represents a result of an operation that does not return data.
    /// </summary>
    public readonly struct DataResult
    {
        public bool Success { get; }
        public long StatusCode { get; }
        public string Title { get; }
        public string Description { get; }
        public string Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult"/> struct.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="statusCode"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="error"></param>
        public DataResult(bool success, long statusCode, string title = null, string description = null, string error = null)
        {
            Success = success;
            StatusCode = statusCode;
            Title = title;
            Description = description;
            Error = error;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DataResult"/> struct with default status code.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="error"></param>
        public DataResult(bool success, string title = null, string description = null, string error = null)
            : this(success, 0, title, description, error)
        {
        }
    }
}