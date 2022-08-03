


namespace Optimization
{
    public class OnePointCrossover<T> : ICrossover<T> where T: unmanaged, IChromosome
    {
        int size;
        int genes;
        Random rand = new Random();

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
            T[] matingpool = (T[])algorithm.MatingPool;
            int mean = matingpool.Length / 2;
            Span<T> parents = new Span<T>(matingpool, 0, mean);
            Span<T> offsprings = new Span<T>(matingpool, mean, mean);

            if(parents.Length != offsprings.Length) throw new Exception("mating pool should split in half");

            for(int i = 0; i < parents.Length; i += 2)
            {    
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
                int point = rand.Next(1, genes - 1);
                
                for(int i = point; i < genes; i++)
                {
                    ofspA[i] = ptrB[i];
                    ofspB[i] = ptrA[i];
                }

            }while(IChromosome.CheckEqualityUnsafe<T>(&parentA, &offspringA, genes)
                || IChromosome.CheckEqualityUnsafe<T>(&parentB, &offspringB, genes)
                || IChromosome.CheckEqualityUnsafe<T>(&offspringA, &offspringB, genes));

            return (offspringA, offspringB);
        }

        [Test]
        internal unsafe static (bool, string) TestOnePointCrossover()
        {      
            var rand = new Random();            
            var crossover = new OnePointCrossover<Chromosome>();
            var msg = new StringBuilder(1000);
            var eq = true;
            
            for(int i = 0; i < 20; i++)
            {
                var A = Chromosome.InitRandom(rand);
                var B = Chromosome.InitRandom(rand);
                (Chromosome offA, Chromosome offB) = crossover.Crossover(A, B);
                
                if(Chromosome.CheckEqualityUnsafe(&A, &offA)
                    || Chromosome.CheckEqualityUnsafe(&A, &offB)
                    || Chromosome.CheckEqualityUnsafe(&B, &offA)
                    || Chromosome.CheckEqualityUnsafe(&B, &offB)
                    || Chromosome.CheckEqualityUnsafe(&offA, &offB)
                    || !offA.Constrained()
                    || !offB.Constrained())
                    {
                        eq = false;
                        msg.AppendLine("parentA: " + A.ToString());
                        msg.AppendLine("parentB: " + B.ToString());
                        msg.AppendLine("offspringA: " + offA.ToString());
                        msg.AppendLine("offspringB: " + offB.ToString());
                        msg.AppendLine();
                    }
            }

            return (eq, msg.ToString());
        }
    }

    public class MultiPointCrossover<T> : ICrossover<T> where T: unmanaged, IChromosome
    {
        public void RunStep(GeneticAlg<T> algorith)
        {
            throw new NotImplementedException();   
        }

        // [Test]
        // internal unsafe static (bool, string) TestMultiPointCrossover()
        // {      
        //     var rand = new Random();            
        //     var crossover = new MultiPointCrossover<Chromosome>();
        //     var msg = new StringBuilder(1000);
        //     var eq = true;
            
        //     for(int i = 0; i < 5; i++)
        //     {
        //         var A = Chromosome.InitRandom(rand);
        //         var B = Chromosome.InitRandom(rand);
        //         (Chromosome offA, Chromosome offB) = crossover.Crossover(A, B, rand.Next(Chromosome.Genes));
        //         msg.AppendLine("parentA: " + A.ToString());
        //         msg.AppendLine("parentB: " + B.ToString());
        //         msg.AppendLine("offspringA: " + offA.ToString());
        //         msg.AppendLine();
        //         msg.AppendLine("offspringB: " + offB.ToString());

        //         if(Chromosome.CheckEqualityUnsafe(&A, &offA)
        //             || Chromosome.CheckEqualityUnsafe(&A, &offB)
        //             || Chromosome.CheckEqualityUnsafe(&B, &offA)
        //             || Chromosome.CheckEqualityUnsafe(&B, &offB)
        //             || Chromosome.CheckEqualityUnsafe(&offA, &offB)) eq = false;
        //     }

        //     return (eq, msg.ToString());
        // }
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

        [Test]
        internal unsafe static (bool, string) TestUniformCrossover()
        {      
            var rand = new Random();            
            var crossover = new UniformCrossover<Chromosome>();
            var msg = new StringBuilder(1000);
            var eq = true;
            
            for(int i = 0; i < 20; i++)
            {
                var A = Chromosome.InitRandom(rand);
                var B = Chromosome.InitRandom(rand);
                (Chromosome offA, Chromosome offB) = crossover.Crossover(A, B);                

                if(Chromosome.CheckEqualityUnsafe(&A, &offA)
                    || Chromosome.CheckEqualityUnsafe(&A, &offB)
                    || Chromosome.CheckEqualityUnsafe(&B, &offA)
                    || Chromosome.CheckEqualityUnsafe(&B, &offB)
                    || Chromosome.CheckEqualityUnsafe(&offA, &offB)
                    || !offA.Constrained()
                    || !offB.Constrained()) 
                    {
                        eq = false;
                        msg.AppendLine("parentA: " + A.ToString());
                        msg.AppendLine("parentB: " + B.ToString());
                        msg.AppendLine("offspringA: " + offA.ToString());
                        msg.AppendLine("offspringB: " + offB.ToString());
                        msg.AppendLine();
                    }
            }

            return (eq, msg.ToString());
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

        [Test]
        internal unsafe static (bool, string) TestArithmeticCrossover()
        {      
            var rand = new Random();            
            var crossover = new ArithmeticCrossover<Chromosome>();
            var msg = new StringBuilder(1000);
            var eq = true;
            
            for(int i = 0; i < 20; i++)
            {
                var A = Chromosome.InitRandom(rand);
                var B = Chromosome.InitRandom(rand);
                (Chromosome offA, Chromosome offB) = crossover.Crossover(A, B);
                
                if(Chromosome.CheckEqualityUnsafe(&A, &offA)
                    || Chromosome.CheckEqualityUnsafe(&A, &offB)
                    || Chromosome.CheckEqualityUnsafe(&B, &offA)
                    || Chromosome.CheckEqualityUnsafe(&B, &offB)
                    || Chromosome.CheckEqualityUnsafe(&offA, &offB)
                    || !offA.Constrained()
                    || !offB.Constrained()) 
                    {
                        eq = false;
                        msg.AppendLine("parentA: " + A.ToString());
                        msg.AppendLine("parentB: " + B.ToString());
                        msg.AppendLine("offspringA: " + offA.ToString());
                        msg.AppendLine("offspringB: " + offB.ToString());
                        msg.AppendLine();
                    }
            }

            return (eq, msg.ToString());
        }    
    }

    ///<summary> simply copies matingpool to population, no complex selecting </summary>
    public class SetPopulation<T> : IUpdatePopulation<T> where T: IChromosome
    {
        public void RunStep(GeneticAlg<T> algorithm)
        {
            var population = algorithm.Population;
            var matingpool = algorithm.Matingpool;
            var generations = algorithm.PopulationGenerations;

            if(population.Length != matingpool.Length) 
                throw new Exception("population and matingpool must have same Length!!");

            Array.Copy(matingpool, 0, population, 0, population.Length);

            for(int i = 0; i < generations.Length; i++) generations[i] += 1;
        }
    }
}
