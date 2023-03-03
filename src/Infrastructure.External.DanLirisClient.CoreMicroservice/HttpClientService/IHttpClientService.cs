﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.External.DanLirisClient.CoreMicroservice.HttpClientService
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> GetAsync(string url, string token);
        Task<HttpResponseMessage> PostAsync(string url, string token, HttpContent content);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
    }
}
