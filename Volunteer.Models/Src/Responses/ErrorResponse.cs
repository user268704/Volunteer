﻿namespace Volunteer.Models.Responses;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
}