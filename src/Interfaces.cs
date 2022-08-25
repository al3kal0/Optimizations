using System;


namespace Optimization
{
    public interface IChromosome<V> where V: INumeric
    {
        double Fitness();
        void RandomInit();
        void Repair();
        int Size { get; }
        int Genes { get; }
        V this[int index] { get; set; };
    }
    
    public interface IStep<T> where T: unmanaged, IChromosome<V>
    {
        void RunStep(GeneticAlg<T> algorithm);
    }

    public interface IPopulationInitialization<T> where T: unmanaged, IChromosome<V>
    {
        void InitPopulation(GeneticAlg<T> algorithm);    
    }    

    public interface ITermination<T> where T: unmanaged, IChromosome<V>
    {
        bool Terminate(GeneticAlg<T> algorithm);
    }

    public interface IFitness<T> : IStep<T> where T: unmanaged, IChromosome<V> {}
    
    public interface ISelection<T> : IStep<T> where T: unmanaged, IChromosome<V> {}

    public interface ICrossover<T> : IStep<T> where T: unmanaged, IChromosome<V> {}

    public interface IMutation<T>  : IStep<T> where T: unmanaged, IChromosome<V> {}

    public interface ISurvival<T> : IStep<T> where T: unmanaged, IChromosome<V> {}    
}
