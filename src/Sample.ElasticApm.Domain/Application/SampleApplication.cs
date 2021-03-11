using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Nest;
using Sample.ElasticApm.Domain.Concrete;
using Sample.ElasticApm.Domain.Indices;
using Sample.ElasticApm.Domain.Model;
using static System.Int32;

namespace Sample.ElasticApm.Domain.Application
{
    public class SampleApplication : ISampleApplication
    {
        private readonly IElasticClient _elasticClient;

        public SampleApplication(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public void PostActorsSample()
        {
            var descriptor = new BulkDescriptor();

            if (!_elasticClient.Indices.Exists(nameof(IndexActors).ToLower()).Exists)
                _elasticClient.Indices.Create(nameof(IndexActors).ToLower());

            _elasticClient.IndexMany<IndexActors>(IndexActors.GetSampleData());

            //or
            descriptor.UpdateMany<IndexActors>(IndexActors.GetSampleData(), (b, u) => b
                .Index(nameof(IndexActors).ToLower())
                .Doc(u)
                .DocAsUpsert());

            var insert = _elasticClient.Bulk(descriptor);

            if (!insert.IsValid)
                throw new Exception(insert.OriginalException.ToString());
        }

        public void PostSampleException()
        {
            Convert.ToInt32("Teste");
        }

        public async Task GetGoogle()
        {
            var httpClient = new HttpClient();
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri("https://google.com"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0)
            };

            var response = await httpClient.SendAsync(message);

            response.EnsureSuccessStatusCode();
        }

        public ICollection<IndexActors> GetAll()
        {
            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result.ToList();
        }

        public ICollection<IndexActors> GetActorsAllCondition(string term)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));
            TryParse(term, out int numero);

            query = query && new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Name).Value($"*{term}*")) //performance ruim, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Description).Value($"*{term}*")) //performance ruim, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexActors>().Term(w => w.Age, numero)
                    || new QueryContainerDescriptor<IndexActors>().Term(w => w.TotalMovies, numero);

            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => query)
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ActorsAggregationModel GetActorsAggregation()
        {
            var query = new QueryContainerDescriptor<IndexActors>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));

            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
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
}
