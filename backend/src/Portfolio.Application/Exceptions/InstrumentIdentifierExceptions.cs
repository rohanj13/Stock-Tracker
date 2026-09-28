namespace Portfolio.Application.Exceptions;

/// <summary>
/// Base exception for instrument identifier resolution failures.
/// </summary>
public class InstrumentIdentifierException : Exception
{
    public InstrumentIdentifierException(string message) : base(message) { }
    public InstrumentIdentifierException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when an instrument cannot be resolved through OpenFIGI (no match found).
/// </summary>
public class InstrumentNotResolvedException : InstrumentIdentifierException
{
    public string? Identifier { get; }

    public InstrumentNotResolvedException(string identifier)
        : base($"No instrument found for identifier: {identifier}")
    {
        Identifier = identifier;
    }
}

/// <summary>
/// Thrown when an identifier resolves to multiple instruments (ambiguous match).
/// </summary>
public class AmbiguousInstrumentException : InstrumentIdentifierException
{
    public string? Identifier { get; }
    public int MatchCount { get; }

    public AmbiguousInstrumentException(string identifier, int matchCount)
        : base($"Multiple instruments found for identifier '{identifier}': {matchCount} matches. Please use a more specific identifier.")
    {
        Identifier = identifier;
        MatchCount = matchCount;
    }
}

/// <summary>
/// Thrown when a FIGI already exists in the database (uniqueness violation).
/// </summary>
public class DuplicateFigiException : InstrumentIdentifierException
{
    public string Figi { get; }

    public DuplicateFigiException(string figi)
        : base($"An instrument with FIGI '{figi}' already exists.")
    {
        Figi = figi;
    }
}

/// <summary>
/// Thrown when the OpenFIGI service is unavailable, rate-limited, or returns an error.
/// </summary>
public class InstrumentIdentifierServiceException : InstrumentIdentifierException
{
    public string? ServiceError { get; }
    public int? HttpStatusCode { get; }

    public InstrumentIdentifierServiceException(string message, string? serviceError = null, int? httpStatusCode = null, Exception? innerException = null)
        : base(message, innerException!)
    {
        ServiceError = serviceError;
        HttpStatusCode = httpStatusCode;
    }
}

/// <summary>
/// Thrown when an instrument lookup request is invalid (missing or conflicting identifiers).
/// </summary>
public class InvalidInstrumentLookupException : InstrumentIdentifierException
{
    public InvalidInstrumentLookupException(string message) : base(message) { }
}
