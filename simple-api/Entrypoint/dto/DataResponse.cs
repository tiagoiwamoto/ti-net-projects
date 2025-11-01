namespace simple_api.Controllers.dto;

public record DataResponse<T>(
    T data,
    string? message = null,
    int? statusCode = null
    );