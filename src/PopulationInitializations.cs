


namespace Optimization
{
    public class RandomInitialization : IPopulationInitialization<T> where T: IChromosome
    {
        public void InitPopulation(GeneticAlg<T> algorithm)
        {
            var population = algorithm.Population;
            Random rand = new Random();

            for(int i = 0; i < population.Length; i++)
            {
                population[i].RandomInit();
            }
        }

        [Test]
        static void TestRandomInitialization()
        {
            bool result = true;
            var population = new Population<Chromosome>(200).Init(new RandomInitialization());

            foreach(var chromosome in (Chromosome[])population)
            {
                result = chromosome.Constrained();
                if(!result) return (result, chromosome.ToString());
            }

            return (result, "");
        }     
    }  

    public class HeuristicInitialization : IPopulationInitialization<T> where T: IChromosome
    {
        public void Init(Population<Chromosome> population)
        {
            throw new NotImplementedException();
        }
    }    
}
