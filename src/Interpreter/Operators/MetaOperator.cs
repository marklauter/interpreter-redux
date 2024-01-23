namespace Interpreter.Operators;

internal abstract class MetaOperator<TResult, TParam>
{
    public virtual int Precedence { get; }
    public virtual int OperandCount { get; }
    public virtual string Symbol { get; } = String.Empty;
    public abstract TResult Evaluate(params TParam[] args);
}
