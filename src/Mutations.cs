using System;

namespace Optimization
{
    public class BitflipMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        Random rand = new Random();
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

            unsafe
            {
                fixed(T* ptr = population)
                {
                    for(int i = 0; i < population.Length; i++)
                    {
                        if(MutationFactor > rand.NextDouble()) Mutate(&ptr[i]);
                    }
                }
            }
        }

        unsafe void Mutate(T* chromosome)
        {
            T copy = *chromosome;
            do
            {
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

                byte* ptr = (byte*)chromosome;
                ptr[offset] ^= mutation;

            }while(IChromosome.CheckEqualityUnsafe<T>(chromosome, &copy, genes));

            chromosome->Repair();
        }

        [Test]
        static void TestBitflipMutation()
        {            
            const int length = 10;
            var population = new Population<Chromosome>(length).Init(new RandomInitialization());
            var copy = new Population<Chromosome>(length);
            Array.Copy((Chromosome[])population, 0, (Chromosome[])copy, 0, length);
            var algorithm = new GeneticAlg<Chromosome>(){ Population = population };
            var mutation = new BitflipMutation<Chromosome>(){ MutationFactor = 1.1 };

            mutation.RunStep(algorithm);
            bool eq = true;
            StringBuilder msg = new StringBuilder(1000);
            var _population = (Chromosome[])population;
            var _copy = (Chromosome[])copy;
            unsafe
            {
                fixed(Chromosome* ptr1 = _population)
                fixed(Chromosome* ptr2 = _copy)
                {                
                    for(int i = 0; i < length; i++)
                    {                        
                        if(Chromosome.CheckEqualityUnsafe(&ptr1[i], &ptr2[i]))
                        {
                            eq = false;
                            msg.AppendLine(_population[i].ToString());
                            msg.AppendLine(_copy[i].ToString() + '\n');
                        }
                    }
                }
            }
        }
    }

    public class RandomMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        Random rand = new Random();
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

        [Test]
        static void TestRandomMutation()
        {            
            const int length = 10;
            var population = new Population<Chromosome>(length).Init(new RandomInitialization());
            var copy = new Population<Chromosome>(length);
            Array.Copy((Chromosome[])population, 0, (Chromosome[])copy, 0, length);
            var algorithm = new GeneticAlg<Chromosome>(){ Population = population };
            var mutation = new RandomMutation<Chromosome>(){ MutationFactor = 1.1 };

            mutation.RunStep(algorithm);
            bool eq = true;
            StringBuilder msg = new StringBuilder(1000);
            var _population = (Chromosome[])population;
            var _copy = (Chromosome[])copy;
            unsafe
            {
                fixed(Chromosome* ptr1 = _population)
                fixed(Chromosome* ptr2 = _copy)
                {                
                    for(int i = 0; i < length; i++)
                    {
                        if(Chromosome.CheckEqualityUnsafe(&ptr1[i], &ptr2[i]))
                        {
                            eq = false;
                            msg.AppendLine(_population[i].ToString());
                            msg.AppendLine(_copy[i].ToString() + '\n');
                        }
                    }
                }
            }

            return (eq, msg.ToString());
        }
    }

    public class ScrambleMutation<T> : IMutation<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        int mutationSize;
        Random rand = new Random();
        public double MutationFactor {get; set;} = 0.5;

        public ScrambleMutation()
        {
            unsafe
            {
                size = sizeof(T);
                genes = size / sizeof(double);
                mutationSize = 12; // sizeof(Mutation);
            }
        }

        public void RunStep(GeneticAlg<T> algorithm)
        {   
            T[] population = (T[])algorithm.Population;

            unsafe
            {
                fixed(T* chromosome = population)
                {
                    for(int i = 0; i < population.Length; i++)
                    {
                        if(MutationFactor > rand.NextDouble())
                        {                            
                            int offset = rand.Next(size - mutationSize);
                            Mutate(&chromosome[i], offset, rand);
                        }
                    }
                }
            }
        }

        unsafe void Mutate(T* chromosome, int offset, Random rand)
        {
            byte* ptr = (byte*)chromosome + offset;
            int* ptr_as_int = (int*)ptr;
            ptr_as_int[0] ^= rand.Next();
            ptr_as_int[1] ^= rand.Next();
            ptr_as_int[2] ^= rand.Next();

            chromosome->Repair();
        }

        [Test]
        static void TestScrambleMutation()
        {            
            const int length = 10;
            var population = new Population<Chromosome>(length).Init(new RandomInitialization());
            var copy = new Population<Chromosome>(length);
            Array.Copy((Chromosome[])population, 0, (Chromosome[])copy, 0, length);
            var algorithm = new GeneticAlg<Chromosome>(){ Population = population };
            var mutation = new ScrambleMutation<Chromosome>(){ MutationFactor = 1.1 };

            mutation.RunStep(algorithm);
            bool eq = true;
            StringBuilder msg = new StringBuilder(1000);
            var _population = (Chromosome[])population;
            var _copy = (Chromosome[])copy;
            unsafe
            {
                fixed(Chromosome* ptr1 = _population)
                fixed(Chromosome* ptr2 = _copy)
                {                
                    for(int i = 0; i < length; i++)
                    {
                        if(Chromosome.CheckEqualityUnsafe(&ptr1[i], &ptr2[i])) 
                        {
                            eq = false;
                            msg.AppendLine(_population[i].ToString());
                            msg.AppendLine(_copy[i].ToString() + '\n');
                        }
                    }                    
                    
                }
            }

            return (eq, msg.ToString());
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
