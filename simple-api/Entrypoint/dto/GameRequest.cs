namespace simple_api.Controllers.dto;

public record GameRequest(
    string? nome,
    string? image,
    string? consoleType,
    double? preco,
    string? descricao
    );