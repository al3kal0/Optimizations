


namespace Optimization
{
    public class FitnessStep<T> : IFitness<T> where T: IChromosome
    {
        public void RunStep(GeneticAlg<T> algorithm)
        {
            var population = algorithm.Population;
            var fitness = algorithm.PopulationFitness;

            for(int i = 0; i < population.Length; i++)
            {
                fitness[i] = population[i].Fitness();
            }
        }
    }
}
