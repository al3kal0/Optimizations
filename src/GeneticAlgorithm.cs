using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
// using NUnit.Framework;

namespace Optimization
{
    // [TestFixture]
    public class GeneticAlg<T> where T: unmanaged, IChromosome
    {
        T[] population;
        T[] matingpool;
        
        double[] population_fitness;
        double[] matingpool_fitness;
        
        int[] population_generations;
        int[] matingpool_generations;
        
        int population_count;
        int matingpool_count;
        int population_maxSize;
        int iterations;

        IPopulationInitialization<T> init_population;
        ITermination<T> termination_condition;
        Queue<IStep<T>> steps = new();

        public GeneticAlg(int max_size, IPopulationInitialization<T> init_population, ITermination<T> termination_condition)
        {
            population = new T[max_size];
            matingpool = new T[max_size];

            population_fitness = new double[max_size];
            matingpool_fitness = new double[max_size];

            population_generations = new int[max_size];
            matingpool_generations = new int[max_size];

            population_maxSize = max_size;
            iterations = 0;

            this.init_population = init_population;
            this.termination_condition = termination_condition;
        }

        #region properties
        public Span<T> Population => population.AsSpan(0, population_count);
        public Span<T> Matingpool => matingpool.AsSpan(0, matingpool_count);
        public Span<double> PopulationFitness => population_fitness.AsSpan(0, population_count);
        public Span<double> MatingpoolFitness => matingpool_fitness.AsSpan(0, matingpool_count);
        public Span<int> PopulationGenerations => population_generations.AsSpan(0, population_count);
        public Span<int> MatingpoolGenerations => matingpool_generations.AsSpan(0, matingpool_count);

        public int PopulationCount
        {
            get => population_count;
            set => population_count = (value <= population_maxSize) ? value : throw new Exception();
        }

        public int MatingpoolCount
        {
            get => matingpool_count;
            set => matingpool_count = (value <= population_maxSize) ? value : throw new Exception(); 
        }        
        #endregion

        public GeneticAlg<T> AddStep(IStep<T> step)
        {
            steps.Enqueue(step);

            return this;
        }

        public void Run()
        {
            init_population.InitPopulation(this);

            while(!termination_condition.Terminate(this))
            {
                foreach(var step in steps)
                {
                    step.RunStep(this);
                }
            }
        }

        public void Sort()
        {
            for(int i = 0; i < population_count - 1; i++)
            {
                for(int j = i + 1; j < population_count; j++)
                {
                    if(population_fitness[j] > population_fitness[i])
                    {
                        var temp = population[i];
                        population[i] = population[j];
                        population[j] = temp;

                        var fitness = population_fitness[i];
                        population_fitness[i] = population_fitness[j];
                        population_fitness[j] = fitness;

                        var generation = population_generations[i];
                        population_generations[i] = population_generations[j];
                        population_generations[j] = generation;
                    }
                }
            }
        }

        // [Test]
        public static void TestGeneticAlgorithm()
        {   
            
        }
    }
}
