using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.ElasticApm.Domain.Indices;
using Sample.ElasticApm.Domain.Model;

namespace Sample.ElasticApm.Domain.Concrete
{
    public interface ISampleApplication
    {
        void PostActorsSample();
        void PostSampleException();
        ICollection<IndexActors> GetAll();
        ICollection<IndexActors> GetActorsAllCondition(string term);
        ActorsAggregationModel GetActorsAggregation();
        Task GetGoogle();
    }
}
