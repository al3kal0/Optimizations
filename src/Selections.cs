

namespace Optimization
{
    /// <summary> Selects Different Parents, no repeats... </summary>
    public class ProportionalSelection<T> : ISelection<T> where T: IChromosome
    {
        int[] sparseSet;
        Random rand = new Random();

        public void RunStep(GeneticAlg<T> algorithm)
        {
            var fitness = algorithm.PopulationFitness;
            var population = algorithm.Population;
            var matingpool = algorithm.MatingPool;
            var parents = new Span<T>(matingpool, 0, matingpool.Length / 2);
            sparseSet ??= new int[population.MaxSize];
            int count = 0;

            // select randomly n chromosomes to store half of matingpool
            do
            {
                for(int i = 0; i < population.Length; i++)
                {
                    // if map (sparseSet) has null (0) in selected index then store that value
                    if(fitness[i] > rand.NextDouble() && sparseSet[i] == 0)
                    {
                        // offset +1 to index so ( > 0) comparison can happen
                        sparseSet[i] = i + 1;
                        count++;
                    }
                }

            } while(count < parents.Length);  // repeat until enough parents are selected
           
            // copy selected parents(from sparseSet) to matingpool and zero value in sparseSet
            for(int i  = 0, n = 0; i < sparseSet.Length && n < parents.Length; i++)
            {
                if(sparseSet[i] > 0) 
                {
                    parents[n++] = population[sparseSet[i] - 1];
                    sparseSet[i] = default;
                }
            }
        }

        [Test]
        static (bool, string) TestProportionalSelection()
        {
            var algorithm = new GeneticAlg<Chromosome>()
            {
                Population = new Population<Chromosome>(200).Init(new RandomInitialization()),
                MatingPool = new Population<Chromosome>(200)
            };
            var fitness = new WSGAFitness();
            var randomselection = new ProportionalSelection<Chromosome>();
            var population = algorithm.Population;
            var matingpool = algorithm.MatingPool;
            var  eq = true;
            var chromose = new Chromosome();
            var msg = new System.Text.StringBuilder(1000);

            fitness.RunStep(algorithm);
            randomselection.RunStep(algorithm);
            unsafe
            {
                fixed(Chromosome* ptr1 = population)
                fixed(Chromosome* ptr2 = matingpool)
                {
                    for(int i = 0; i < population.Length / 2; i++)
                    {
                        eq &= !IChromosome.CheckEqualityUnsafe<Chromosome>(&ptr1[i], &chromose, Chromosome.Genes);
                        eq &= !IChromosome.CheckEqualityUnsafe<Chromosome>(&ptr2[i], &chromose, Chromosome.Genes);                        
                    }
                    for(int i = population.Length / 2; i < population.Length; i++)
                    {
                        eq &= IChromosome.CheckEqualityUnsafe<Chromosome>(&ptr2[i], &chromose, Chromosome.Genes);
                    }
                }
            }     

            return (eq, msg.ToString());       
        }
    }

    /// <summary> Selects Parents proportional to fitness, repeats might happen... </summary>

    public class RouletteWheelSelection<T> : ISelection<T> where T: IChromosome
    {
        Random rand = new Random();

        public void RunStep(GeneticAlg<T> algorithm)
        {
            var fitness = algorithm.PopulationFitness;
            var population = algorithm.Population;
            var  matingpool = algorithm.MatingPool;
            var parents = new Span<T>(matingpool, 0, matingpool.Length / 2);
            int count = 0;

            // select randomly n chromosomes to store half of matingpool
            do
            {
                for(int i = 0; i < population.Length; i++)
                {
                    if(fitness[i] > rand.NextDouble())
                    {
                        count++;
                    }
                }

            } while(count < parents.Length);  // repeat until enough parents are selected
        }
    }

    public class StochasticUniversalSampling<T> : ISelection<T> where T: IChromosome
    {
        public void RunStep(GeneticAlg<T> algorithm)
        {
            throw new NotImplementedException();
        }
    }

    public class TournamentSelection<T> : ISelection<T> where T: IChromosome
    {
        readonly static int[] participants_max = { 2, 4, 8, 16, 32, 64, 128, 256 };
        int maxParticipants = 16;
        int[] sparseSet;
        Random rand = new Random();
        public int TournamentParticipants 
        {
            get => maxParticipants;
            set
            {
                bool accept = false;
                foreach(int i in participants_max)
                {
                    if(value == i) 
                    {
                        accept = true; break;
                    }
                }
                if(!accept) throw new ArgumentException("TournamentParticipants must be 2^n");

                maxParticipants = value;
            }
        }

        public void RunStep(GeneticAlg<T> algorithm)
        {
            double[] fitness = algorithm.Population.Fitness;
            T[] population = (T[])algorithm.Population;
            T[] matingpool = (T[])algorithm.MatingPool;
            Span<T> parents = new Span<T>(matingpool, 0, matingpool.Length / 2);
            Span<int> tournament = stackalloc int[maxParticipants];
            sparseSet ??= new int[population.Length];
            int count = 0;            

            do
            {
                int _participants = 0;

                // select DIFFERENT participants for the tournament
                do
                {
                    int selected = rand.Next(population.Length);
                    if(sparseSet[selected] == 0)
                    {
                        sparseSet[selected] = selected + 1;
                        _participants++;
                    }

                }while(_participants < maxParticipants);

                // enlists selected participants for the tournament
                for(int i = 0, n = 0; i < sparseSet.Length; i++)
                {
                    if(sparseSet[i] > 0)
                    {
                        tournament[n++] = sparseSet[i] - 1;
                        sparseSet[i] = default;
                    } 
                }

                // begin tournament 
                // until only one remains
                do
                {
                    int rounds = tournament.Length;
                    for(int i = 0, j = 0; i < rounds; i += 2)
                    {
                        int pA = tournament[i];
                        int pB = tournament[i + 1];
                        
                        if(fitness[pA] > fitness[pB]) tournament[j++] = pA;
                        else tournament[j++] = pB;    

                        _participants--;                    
                    }
                    rounds /= 2;

                }while (_participants > 1);                

                parents[count++] = population[tournament[0] - 1];

            }while(count < parents.Length);
        }
    }

    public class RankSelection<T> : ISelection<T> where T: IChromosome
    {
        public void RunStep(GeneticAlg<T> algorithm)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary> Selects Different Parents, no repeats... </summary>
    public class RandomSelection<T> : ISelection<T> where T: IChromosome
    {
        int[] sparseSet;
        Random rand = new Random();

        public void RunStep(GeneticAlg<T> algorithm)
        {
            T[] population = (T[])algorithm.Population;
            T[] matingpool = (T[])algorithm.MatingPool;
            Span<T> parents = new Span<T>(matingpool, 0, matingpool.Length / 2);
            sparseSet ??= new int[population.Length];
            int count = 0;

            // select randomly n chromosomes to store half of matingpool
            while(count < parents.Length)
            {
                int selected = rand.Next(population.Length);

                // if map (sparseSet) has null (0) in selected index then store that value
                if(sparseSet[selected] == 0)
                {
                    // offset +1 to index so ( > 0) comparison can happen
                    sparseSet[selected] = selected + 1;
                    count++;
                }
            }

            for(int i  = 0, n = 0; i < sparseSet.Length; i++)
            {
                if(sparseSet[i] > 0) 
                {
                    parents[n++] = population[sparseSet[i] - 1];
                    sparseSet[i] = default;
                }
            }
        }

        [Test]
        internal static bool TestRandomSelection()
        {
            var algorithm = new GeneticAlg<Chromosome>()
            {
                Population = new Population<Chromosome>(200).Init(new RandomInitialization()),
                MatingPool = new Population<Chromosome>(200)
            };
            var fitness = new WSGAFitness();
            var randomselection = new RandomSelection<Chromosome>();
            var population = (Chromosome[])algorithm.Population;
            var matingpool = (Chromosome[])algorithm.MatingPool;
            var  eq = true;
            var chromose = new Chromosome();

            fitness.RunStep(algorithm);
            randomselection.RunStep(algorithm);
            unsafe
            {
                fixed(Chromosome* ptr1 = population)
                fixed(Chromosome* ptr2 = matingpool)
                {
                    for(int i = 0; i < population.Length / 2; i++)
                    {
                        eq &= !IChromosome.CheckEqualityUnsafe<Chromosome>(&ptr1[i], &chromose, Chromosome.Genes);
                        eq &= !IChromosome.CheckEqualityUnsafe<Chromosome>(&ptr2[i], &chromose, Chromosome.Genes);
                    }
                    for(int i = population.Length / 2; i < population.Length; i++)
                    {
                        eq &= IChromosome.CheckEqualityUnsafe<Chromosome>(&ptr2[i], &chromose, Chromosome.Genes);
                    }
                }
            }     

            return eq;       
        }
    }    
}
