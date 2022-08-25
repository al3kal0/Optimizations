


namespace Optimization
{
    public class RandomInitialization<T> : IPopulationInitialization<T> where T: IChromosome
    {
        public void InitPopulation(GeneticAlg<T> algorithm)
        {
            var population = algorithm.Population;
            var rand = new Random();

            for(int i = 0; i < population.Length; i++)
            {
                population[i].RandomInit();
            }
        }
    }  

    public class HeuristicInitialization<T> : IPopulationInitialization<T> where T: IChromosome
    {
        public void InitPopulation(GeneticAlg<T> algorithm)
        {
            throw new NotImplementedException();
        }
    }    
}
