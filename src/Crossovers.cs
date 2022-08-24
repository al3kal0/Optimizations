


namespace Optimization
{
    public class OnePointCrossover<T> : ICrossover<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        Random rand = new();

        public OnePointCrossover()
        {
            unsafe
            {
                size = sizeof(T);   
                genes = size / sizeof(double);                 
            }
        }

        public void RunStep(GeneticAlg<T> algorithm)
        {
            // one half of mating pool stores parents
            // while the other half stores offsprings; 
            var matingpool = algorithm.Matingpool;
            int mean = matingpool.Length / 2;
            var parents = matingpool[0..mean];
            var offsprings = matingpool[mean..];

            if(parents.Length != offsprings.Length) throw new Exception("mating pool should split in half");

            for(int i = 0; i < parents.Length; i += 2)
            {    
                var point = rand.Next(1, genes - 1);
                ref var parentA = ref parents[i + 0];
                ref var parentB = ref parents[i + 1];
                ref var offspringA = ref offsprings[i + 0];
                ref var offspringB = ref offsprings[i + 1];
                
                for(int j = point; j < genes; j++)
                {
                    offspringA[j] = parentB[j];
                    offspringB[j] = parentA[j];
                }
            }
        }
    }

      

    public class MultiPointCrossover<T> : ICrossover<T> where T: unmanaged, IChromosome
    {
        public void RunStep(GeneticAlg<T> algorith)
        {
            // one half of mating pool stores parents
            // while the other half stores offsprings; 
            var matingpool = algorithm.Matingpool;
            int mean = matingpool.Length / 2;
            var parents = matingpool[0..mean];
            var offsprings = matingpool[mean..];

            if(parents.Length != offsprings.Length) throw new Exception("mating pool should split in half");

            for(int i = 0; i < parents.Length; i += 2)
            {    
                var point = rand.Next(1, genes - 1);
                ref var parentA = ref parents[i + 0];
                ref var parentB = ref parents[i + 1];
                ref var offspringA = ref offsprings[i + 0];
                ref var offspringB = ref offsprings[i + 1];

                var points = stackalloc int[2];
                points[0] = rand.Next(1, genes - 2);
                points[1] = rand.Next(points[0], genes - 1);
                
                for(int j = points[0]; j < points[1]; j++)
                {
                    offspringA[j] = parentB[j];
                    offspringB[j] = parentA[j];
                }
                
                // for(int j = points[1]; j < genes; j++)
                // {
                    // offspringA[j] = parentB[j];
                    // offspringB[j] = parentA[j];
                // }
            }
        }       
    }

    public class UniformCrossover<T> : ICrossover<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        double crossoverFactor = 0.5;
        Random rand = new Random();
        public double CrossoverFactor
        {
            get => crossoverFactor;
            set
            {
                if(value <= 0 || value >= 1) throw new ArgumentException("value must be (0, 1)");
                crossoverFactor = value;
            }
        }

        public UniformCrossover()
        {
            unsafe
            {
                size = sizeof(T);   
                genes = size / sizeof(double);                 
            }
        }


        public void RunStep(GeneticAlg<T> algorithm)
        {
            // one half of mating pool stores parents
            // while the other half stores offsprings; 
            T[] matingpool = (T[])algorithm.MatingPool;
            int mean = matingpool.Length / 2;
            Span<T> parents = new Span<T>(matingpool, 0, mean);
            Span<T> offsprings = new Span<T>(matingpool, mean, mean);

            for(int i = 0; i < parents.Length; i += 2)
            {
                int crossoverPoint = rand.Next(genes);
                (T offspringA, T offspringB) = Crossover(parents[i], parents[i + 1]);
                offsprings[i] = offspringA;
                offsprings[i + 1] = offspringB;
            }
        }

        private unsafe (T, T) Crossover(T parentA, T parentB)
        {
            T offspringA = default;
            T offspringB = default;          

            double* ptrA = (double*)&parentA;
            double* ptrB = (double*)&parentB;
            double* ofspA = (double*)&offspringA;
            double* ofspB = (double*)&offspringB;

            do
            {
                offspringA = parentA;
                offspringB = parentB;

                for(int i = 0; i < genes; i++)
                {
                    if(CrossoverFactor < rand.NextDouble())
                    {
                        ofspA[i] = ptrB[i];
                        ofspB[i] = ptrA[i];                        
                    }
                }

            }while(IChromosome.CheckEqualityUnsafe<T>(&parentA, &offspringA, genes)
                || IChromosome.CheckEqualityUnsafe<T>(&parentB, &offspringB, genes)
                || IChromosome.CheckEqualityUnsafe<T>(&parentA, &offspringB, genes)
                || IChromosome.CheckEqualityUnsafe<T>(&parentB, &offspringA, genes)
                || IChromosome.CheckEqualityUnsafe<T>(&offspringA, &offspringB, genes));

            return (offspringA, offspringB);
        }

    }    

    public class ArithmeticCrossover<T> : ICrossover<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        Random rand = new Random();
        T[] MatingPool {get; set;}
        public double crossFactor = 0.75;
        public double CrossFactor
        {
            get => crossFactor;
            set 
            {
                if(value <= 0 || value >= 1) throw new ArgumentException("value must be (0, 1)");
                crossFactor = value;
            }
        }

        public ArithmeticCrossover()
        {
            unsafe
            {
                size = sizeof(T);   
                genes = size / sizeof(double);                 
            }
        }


        public void RunStep(GeneticAlg<T> algorithm)
        {
            // one half of mating pool stores parents
            // while the other half stores offsprings; 
            T[] matingpool = (T[])algorithm.MatingPool;
            int mean = matingpool.Length / 2;
            Span<T> parents = new Span<T>(matingpool, 0, mean);
            Span<T> offsprings = new Span<T>(matingpool, mean, mean);

            for(int i = 0; i < parents.Length; i++)
            {
                int crossoverPoint = rand.Next(genes);
                (T offspringA, T offspringB) = Crossover(parents[i], parents[i + 1]);
                offsprings[i] = offspringA;
                offsprings[i + 1] = offspringB;
            }
        }

        private unsafe (T, T) Crossover(T parentA, T parentB)
        {
            T offspringA = parentA;
            T offspringB = parentB;          

            double* ptrA = (double*)&parentA;
            double* ptrB = (double*)&parentB;
            double* ofspA = (double*)&offspringA;
            double* ofspB = (double*)&offspringB;

            for(int i = 0; i < genes; i++)
            {
                ofspA[i] = CrossFactor * ptrA[i] + (1 - CrossFactor) * ptrB[i];
                ofspB[i] = (1 - CrossFactor) * ptrA[i] + CrossFactor * ptrB[i];
            }

            return (offspringA, offspringB);
        }
    }

}
