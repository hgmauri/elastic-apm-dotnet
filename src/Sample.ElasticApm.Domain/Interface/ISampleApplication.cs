using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.ElasticApm.Domain.Model;

namespace Sample.ElasticApm.Domain.Interface;

public interface ISampleApplication
{
    Task PostSampleElastic();
    void PostSampleException();
    Task PostDataSql();
    ICollection<IndexActorsModel> GetAll();
    ICollection<IndexActorsModel> GetActorsAllCondition(string term);
    ActorsAggregationModel GetActorsAggregation();
    Task GetGoogle();
    Task GetApiTest();
}