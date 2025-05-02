namespace Application.Models;

public sealed record ResponseModel<T>(string Message, T? Data);