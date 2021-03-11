using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.ElasticApm.Domain.Model;

namespace Sample.ElasticApm.Domain.Concrete
{
    public interface ISampleApplication
    {
        void PostActorsSample();
        void PostSampleException();
        void PostDataSql();
        ICollection<IndexActorsModel> GetAll();
        ICollection<IndexActorsModel> GetActorsAllCondition(string term);
        ActorsAggregationModel GetActorsAggregation();
        Task GetGoogle();
    }
}
