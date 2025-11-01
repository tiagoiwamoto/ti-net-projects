namespace simple_api.Controllers.dto;

public record GameResponse(
    string? nome,
    string? image,
    string? consoleType,
    double? preco,
    string? descricao
    );