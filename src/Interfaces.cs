


namespace Optimization
{
    public interface IChromosome
    {
        double Fitness();
        void RandomInit();
        void Repair();
        int Size { get; }
        int Genes { get; }
    }
    
    public interface IStep<T> where T: unmanaged, IChromosome
    {
        void RunStep(GeneticAlg<T> algorithm);
    }

    public interface IPopulationInitialization<T> where T: unmanaged, IChromosome
    {
        void InitPopulation(GeneticAlg<T> algorithm);    
    }    

    public interface ITermination<T> where T: unmanaged, IChromosome
    {
        bool Terminate(GeneticAlg<T> algorithm);
    }

    public interface IFitness<T> : IStep<T> where T: unmanaged, IChromosome {}
    
    public interface ISelection<T> : IStep<T> where T: unmanaged, IChromosome {}

    public interface ICrossover<T> : IStep<T> where T: unmanaged, IChromosome {}

    public interface IMutation<T>  : IStep<T> where T: unmanaged, IChromosome {}

    public interface IUpdatePopulation<T> : IStep<T> where T: unmanaged, IChromosome {}    
}
