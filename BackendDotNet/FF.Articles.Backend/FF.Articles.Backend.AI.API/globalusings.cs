global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using System.Net.Http.Headers;
global using System.Text.Json;

global using FF.AI.Common;
global using FF.AI.Common.Constants;
global using FF.AI.Common.Interfaces;
global using FF.AI.Common.Models;

global using FF.Articles.Backend.AI.API.Infrastructure;
global using FF.Articles.Backend.AI.API.Interfaces.Repositories;
global using FF.Articles.Backend.AI.API.Interfaces.Services;
global using FF.Articles.Backend.AI.API.Interfaces.Services.RemoteServices;
global using FF.Articles.Backend.AI.API.MapperExtensions;
global using FF.Articles.Backend.AI.API.Models.Dtos;
global using FF.Articles.Backend.AI.API.Models.Entities;
global using FF.Articles.Backend.AI.API.Models.Requests.ArticleGenerations;
global using FF.Articles.Backend.AI.API.Models.Requests.ChatRounds;
global using FF.Articles.Backend.AI.API.Models.Requests.Sessions;
global using FF.Articles.Backend.AI.API.Repositories;
global using FF.Articles.Backend.AI.API.Services;
global using FF.Articles.Backend.AI.API.Services.Consumers;
global using FF.Articles.Backend.AI.API.Services.RemoteServices;
global using FF.Articles.Backend.AI.API.UnitOfWork;

global using FF.Articles.Backend.Common.ApiDtos;
global using FF.Articles.Backend.Common.Bases;
global using FF.Articles.Backend.Common.Bases.Interfaces;
global using FF.Articles.Backend.Common.Constants;
global using FF.Articles.Backend.Common.Exceptions;
global using FF.Articles.Backend.Common.Responses;
global using FF.Articles.Backend.Common.Utils;

global using FF.Articles.Backend.Common.RabbitMQ;
