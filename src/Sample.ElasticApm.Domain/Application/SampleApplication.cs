﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nest;
using Sample.ElasticApm.Domain.Interface;
using Sample.ElasticApm.Domain.Model;
using Sample.ElasticApm.Persistence.Context;
using Sample.ElasticApm.Persistence.Entity;

namespace Sample.ElasticApm.Domain.Application;

public class SampleApplication : ISampleApplication
{
    private readonly IElasticClient _elasticClient;
    private readonly IHttpClientFactory _client;
    private readonly SampleDataContext _context;

    public SampleApplication(IElasticClient elasticClient, SampleDataContext context, IHttpClientFactory client)
    {
        _elasticClient = elasticClient;
        _context = context;
        _client = client;
    }

    public void PostSampleElastic()
    {
        var descriptor = new BulkDescriptor();

        if (!_elasticClient.Indices.Exists(nameof(IndexActorsModel).ToLower()).Exists)
            _elasticClient.Indices.Create(nameof(IndexActorsModel).ToLower());

        _elasticClient.IndexMany(IndexActorsModel.GetSampleData());

        //or
        descriptor.UpdateMany<IndexActorsModel>(IndexActorsModel.GetSampleData(), (b, u) => b
            .Index(nameof(IndexActorsModel).ToLower())
            .Doc(u)
            .DocAsUpsert());

        var insert = _elasticClient.Bulk(descriptor);

        if (!insert.IsValid)
            throw new Exception(insert.OriginalException.ToString());
    }

    public void PostDataSql()
    {
        _context.Database.Migrate();
        var pessoa = new Pessoa
        {
            DataNascimento = DateTime.Now,
            Endereco = "Teste teste teste teste teste teste teste teste teste"
        };

        for (var i = 0; i < 200; i++)
        {
            pessoa.Id = Guid.NewGuid();
            pessoa.Nome = $"Pessoa teste {i}";
            _context.Pessoas.Add(pessoa);
        }

        _context.SaveChanges();

        var pessoas = _context.Pessoas.Where(p => p.Nome.Contains("Pessoa")).ToList();
        var enderecos = _context.Pessoas.Where(p => p.Endereco.Contains("teste")).ToList();
    }

    public void PostSampleException()
    {
        Convert.ToInt32("Teste");
    }

    public async Task GetGoogle()
    {
        var httpClient = _client.CreateClient();
        var message = new HttpRequestMessage
        {
            RequestUri = new Uri("https://google.com"),
            Method = HttpMethod.Get,
            Version = new Version(2, 0)
        };

        var response = await httpClient.SendAsync(message);

        response.EnsureSuccessStatusCode();
    }

    public async Task GetApiTest()
    {
        var httpClient = _client.CreateClient();
        var message = new HttpRequestMessage
        {
            RequestUri = new Uri("https://jsonplaceholder.typicode.com/todos"),
            Method = HttpMethod.Get,
            Version = new Version(2, 0)
        };

        var response = await httpClient.SendAsync(message);

        response.EnsureSuccessStatusCode();
    }

    public ICollection<IndexActorsModel> GetAll()
    {
        var result = _elasticClient.Search<IndexActorsModel>(s => s
            .Index(nameof(IndexActorsModel).ToLower())
            .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

        return result.ToList();
    }

    public ICollection<IndexActorsModel> GetActorsAllCondition(string term)
    {
        var query = new QueryContainerDescriptor<IndexActorsModel>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));
        int.TryParse(term, out var numero);

        query = query && new QueryContainerDescriptor<IndexActorsModel>().Wildcard(w => w.Field(f => f.Name).Value($"*{term}*"))
                || new QueryContainerDescriptor<IndexActorsModel>().Wildcard(w => w.Field(f => f.Description).Value($"*{term}*"))
                || new QueryContainerDescriptor<IndexActorsModel>().Term(w => w.Age, numero)
                || new QueryContainerDescriptor<IndexActorsModel>().Term(w => w.TotalMovies, numero);

        var result = _elasticClient.Search<IndexActorsModel>(s => s
            .Index(nameof(IndexActorsModel).ToLower())
            .Query(s => query)
            .Size(10)
            .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

        return result?.ToList();
    }

    public ActorsAggregationModel GetActorsAggregation()
    {
        var query = new QueryContainerDescriptor<IndexActorsModel>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));

        var result = _elasticClient.Search<IndexActorsModel>(s => s
            .Index(nameof(IndexActorsModel).ToLower())
            .Query(s => query)
            .Aggregations(a => a.Sum("TotalAge", sa => sa.Field(o => o.Age))
                .Sum("TotalMovies", sa => sa.Field(p => p.TotalMovies))
                .Average("AvAge", sa => sa.Field(p => p.Age))
            ));

        var totalAge = ObterBucketAggregationDouble(result.Aggregations, "TotalAge");
        var totalMovies = ObterBucketAggregationDouble(result.Aggregations, "TotalMovies");
        var avAge = ObterBucketAggregationDouble(result.Aggregations, "AvAge");

        return new ActorsAggregationModel { TotalAge = totalAge, TotalMovies = totalMovies, AverageAge = avAge };
    }

    public static double ObterBucketAggregationDouble(AggregateDictionary agg, string bucket)
    {
        return agg.BucketScript(bucket).Value.HasValue ? agg.BucketScript(bucket).Value.Value : 0;
    }
}