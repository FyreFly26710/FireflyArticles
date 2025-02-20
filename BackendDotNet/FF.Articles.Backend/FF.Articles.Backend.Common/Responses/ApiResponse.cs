﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Responses;
/// <summary>
/// All api should return this response
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// All api should return 200 OK status code, indicate the request was received
    /// Return custom status code & Message in ApiResponse
    /// </summary>
    public int Code { get; set; }

    public string Message { get; set; }

    public T? Data { get; set; }

    public ApiResponse(int code, T data, string message)
    {
        Code = code;
        Data = data;
        Message = message;
    }

}