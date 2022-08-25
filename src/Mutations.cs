using System;

namespace Optimization
{
    public class BitflipMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        Random rand = new();
        public double MutationFactor {get; set;} = 0.1;

        public BitflipMutation()
        {
            unsafe
            {
                size = sizeof(T);
                genes = size / sizeof(double);
            }
        }

        public void RunStep(GeneticAlg<T> algorithm)
        {
            var population = algorithm.Population;
            
            for(int i = 0; i < population.Length; i++)
            {
                if(MutationFactor > rand.NextDouble())
                {
                    ref var chromosome = ref population[i];
                    var temp = chromosome;
                    byte mutation = rand.Next(7) switch
                    {
                        0 => 0b000_0001,
                        1 => 0b000_0010,
                        2 => 0b000_0100,
                        3 => 0b000_1000,
                        4 => 0b001_0000,
                        5 => 0b010_0000,
                        6 => 0b100_0000,
                        _ => 0b000_0000
                    };
    
                    int offset = rand.Next(size - sizeof(byte));
                    unsafe
                    {
                        byte* ptr = (byte*)temp;
                        ptr[offset] ^= mutation;                            
                    }                    
                    chromosome = temp;
                    chromosome.Repair();
                }
            }
        }   
    }

    public class RandomMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        Random rand = new();
        public double MutationFactor {get; set;} = 0.1;

        public RandomMutation()
        {
            unsafe
            {
                size = sizeof(T);
                genes = size / sizeof(double);
            }
        }

        public void RunStep(GeneticAlg<T> algorithm)
        {
            var population = algorithm.Population;

            for(int i = 0; i < population.Length; i++)
            {
                if(MutationFactor > rand.NextDouble())
                {
                    int _gene = rand.Next(genes);
                    population[i].RandomMutation(_gene, rand);

                    population[i].Repair();
                }
            }
        }      
    }

    public class ScrambleMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        const int MUTATION_SIZE = 12;
        Random rand = new Random();
        public double MutationFactor {get; set;} = 0.5;

        public ScrambleMutation()
        {
            unsafe
            {
                size = sizeof(T);
                genes = size / sizeof(double);
                // mutationSize = 12; // sizeof(Mutation);
            }
        }

        public void RunStep(GeneticAlg<T> algorithm)
        {   
            var population = algorithm.Population;

            for(int i = 0; i < population.Length; i++)
            {
                if(MutationFactor > rand.NextDouble())
                {
                    ref var chromosome = ref population[i];
                    var temp = chromosome;
                    int offset = rand.Next(size - MUTATION_SIZE);
                    unsafe
                    {
                        byte* ptr = (byte*)temp + offset;
                        int* ptr_as_int = (int*)ptr;
                        ptr_as_int[0] ^= rand.Next();
                        ptr_as_int[1] ^= rand.Next();
                        ptr_as_int[2] ^= rand.Next();
                    }
                    chromosome = temp;                    
                    chromosome.Repair();                        
                }
            }
        }
    }

    public class SwapMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        public void RunStep(GeneticAlg<T> algorithm)
        {   
            throw new NotImplementedException();
        }
    }

    public class InversionMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        public void RunStep(GeneticAlg<T> algorithm)
        {   
            throw new NotImplementedException();
        }
    }
}
