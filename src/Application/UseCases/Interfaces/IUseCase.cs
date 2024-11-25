namespace FIAP.TechChallenge.LambdaProduto.Application.UseCases.Interfaces
{
    public interface IUseCase<T1, T2>
    {
        T2 Execute(T1 request);
    }

    public interface IUseCase<T1>
    {
        T1 Execute();
    }
    
    public interface IUseCaseAsync<T1, T2>
    {
        Task<T2> Execute(T1 request);
    }

    public interface IUseCaseAsync<T1>
    {
        Task<T1> Execute();
    }
}
