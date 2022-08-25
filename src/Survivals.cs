

namespace Optimization
{
    public class NexGenSurvival<T>: ISurvival<T> where T: unmanaged, IChromosome
    {
        public void RunStep(GeneticAlg<T> algorithm)
        {
            var population = algorithm.Population;
            var matingpool = algorithm.Matingpool;
            
            for(int i = 0; i < algorithm.MatingpoolCount; i++)
            {
                population[i] = matingpool[i];
                algorithm.PopulationCount += 1;
            }            
        }
    }
}
