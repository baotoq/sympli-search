namespace SearchService.Application.Common.Exceptions;

[Serializable]
public class ServiceUnavailableException(string message = "Service unavailable.") : Exception(message);
