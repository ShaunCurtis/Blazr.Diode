/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

/// <summary>
/// Delegate Declaration for a mutation
/// It defines a function that receives a TEntity object
/// And returns a new TEntity object with the mutation applied
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <param name="entity"></param>
/// <returns></returns>
public delegate Task<DiodeResult<T>> DiodeMutationDelegate<T>(DiodeMutationRequest<T> request) where T : class;
